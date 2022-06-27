using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using ShareInvest;
using ShareInvest.Server;
using ShareInvest.Server.Data;
using ShareInvest.Server.Data.Models;
using ShareInvest.Server.Hubs;
using ShareInvest.Server.Properties;

using System.Security.Cryptography.X509Certificates;

Condition.SetDebug();

if (WebApplication.CreateBuilder(args) is WebApplicationBuilder builder)
    using (var stream = new PipeStream(".", nameof(ShareInvest.Interface.Hermes)))
    {
        builder.Services
            .AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            })
            .AddHubOptions<Hermes>(o =>
            {
                o.MaximumReceiveMessageSize = null;
                o.EnableDetailedErrors = true;
            })
            .AddHubOptions<Message>(o =>
            {
                o.MaximumReceiveMessageSize = null;
                o.EnableDetailedErrors = true;
            });
        builder.Services
            .AddDbContext<CoreContext>(o =>
            {
                o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            })
            .AddDatabaseDeveloperPageExceptionFilter()
            .AddResponseCompression(o =>
            {
                o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
                o.EnableForHttps = true;
            })
            .AddSingleton<Hermes>()
            .Configure<KestrelServerOptions>(o =>
            {
                o.ListenAnyIP(0x23BF, o =>
                {
                    o.UseHttps(StoreName.My, "", true).UseConnectionLogging();
                });
                o.Limits.MaxRequestBodySize = null;
            })
            .AddDefaultIdentity<CoreUser>(o =>
            {
                o.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<CoreContext>();

        builder.Services
            .AddIdentityServer(o =>
            {
                o.LicenseKey = Resources.license_key;
                o.KeyManagement.Enabled = false;
            })
            .AddApiAuthorization<CoreUser, CoreContext>();

        builder.Services
            .AddAuthentication()
            .AddKakaoTalk(o =>
            {
                o.ClientId = builder.Configuration["KakaoTalk:ClientId"];
                o.ClientSecret = builder.Configuration["KakaoTalk:ClientSecret"];
                o.SaveTokens = true;
                o.Events.OnCreatingTicket = o =>
                {
                    var tokens = o.Properties.GetTokens().ToList();
                    tokens.Add(new AuthenticationToken
                    {
                        Name = "auth_info",
                        Value = o.User.GetRawText()
                    });
                    o.Properties.StoreTokens(tokens);

                    return Task.CompletedTask;
                };
            })
            .AddIdentityServerJwt();

        builder.Services
            .TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>());

        builder.Services
            .AddControllersWithViews()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.WriteIndented = true;
            });
        builder.Services
            .AddRazorPages();

        if (builder.Build() is WebApplication app)
        {
            if (app.Environment.IsDevelopment())
                app.UseMigrationsEndPoint().UseWebAssemblyDebugging();

            else
                app.UseExceptionHandler("/Error").UseHsts();

            app.UseHttpsRedirection()
               .UseResponseCompression()
               .UseBlazorFrameworkFiles()
               .UseStaticFiles()
               .UseRouting()
               .UseIdentityServer()
               .UseAuthentication()
               .UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");
            app.MapHub<Hermes>("/hubs/hermes", o =>
            {
                o.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
            });
            app.MapHub<Message>("/hubs/message", o =>
            {
                o.CloseOnAuthenticationExpiration = true;
                o.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
            });
            if (app.Services.GetService<Hermes>() is Hermes hermes)
            {
                stream.Send += hermes.OnReceiveMessage;
                stream.StartServerProgress();
                GC.Collect();
                app.Run();
            }
        }
    }