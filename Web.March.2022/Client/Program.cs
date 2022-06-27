using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using ShareInvest;
using ShareInvest.Client;

if (WebAssemblyHostBuilder.CreateDefault(args) is WebAssemblyHostBuilder builder)
{
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
    builder.Services
        .AddHttpClient("Server", o => o.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
        .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
    builder.Services.AddScoped(o => o.GetRequiredService<IHttpClientFactory>().CreateClient("Server"));
    builder.Services.AddApiAuthorization();

    if (builder.Build() is WebAssemblyHost host)
    {
        Condition.SetDebug();
        await host.RunAsync();
    }
}