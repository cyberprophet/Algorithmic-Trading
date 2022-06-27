using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Newtonsoft.Json;

using ShareInvest.Server.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace ShareInvest.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        public ExternalLoginModel(SignInManager<CoreUser> signInManager, UserManager<CoreUser> userManager, ILogger<ExternalLoginModel> logger)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
        }
        [BindProperty, AllowNull]
        public InputModel Input
        {
            get; set;
        }
        public string? LoginProvider
        {
            get; set;
        }
        public string? ReturnUrl
        {
            get; set;
        }
        [TempData]
        public string? ErrorMessage
        {
            get; set;
        }
        public class InputModel
        {
            [Required, EmailAddress]
            public string? Email
            {
                get; set;
            }
        }
        public IActionResult OnGetAsync() => RedirectToPage("./Login");
        public IActionResult OnPost(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new
            {
                returnUrl
            });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }
        public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";

                return RedirectToPage("./Login", new
                {
                    ReturnUrl = returnUrl
                });
            }
            var info = await signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";

                return RedirectToPage("./Login", new
                {
                    ReturnUrl = returnUrl
                });
            }
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded && (await signInManager.UpdateExternalAuthenticationTokensAsync(info)).Succeeded)
            {
                var user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                var props = new AuthenticationProperties();

                try
                {
                    if (info.AuthenticationTokens.Single(o => "auth_info".Equals(o.Name))?.Value is string value)
                    {
                        var talk = JsonConvert.DeserializeObject<KakaoTalk>(value);
                        user.NickName = talk.Property.NickName;
                        user.ThumbnailImage = talk.Property.ThumbnailImage;
                        user.ProfileImage = talk.Property.ProfileImage;

                        if ((await userManager.UpdateAsync(user)).Succeeded)
                            logger.LogInformation("update kakaotalk infomation");
                    }
                }
                catch (Exception ex)
                {
                    var tokens = info.AuthenticationTokens.ToList();
                    tokens.Add(new AuthenticationToken
                    {
                        Name = "update_exception",
                        Value = ex.Message
                    });
                    info.AuthenticationProperties.StoreTokens(tokens);
                }
                props.StoreTokens(info.AuthenticationTokens);
                await signInManager.SignInAsync(user, props, info.LoginProvider);
                logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity?.Name, info.LoginProvider);

                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
                return RedirectToPage("./Lockout");

            else
            {
                ReturnUrl = returnUrl;
                LoginProvider = info.LoginProvider;

                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                return Page();
            }
        }
        public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            var info = await signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";

                return RedirectToPage("./Login", new
                {
                    ReturnUrl = returnUrl
                });
            }
            if (ModelState.IsValid)
            {
                var user = new CoreUser
                {
                    UserName = Input.Email,
                    Email = Input.Email
                };
                var result = await userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(user, info);

                    if (result.Succeeded)
                    {
                        if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                            await userManager.AddClaimAsync(user, info.Principal.FindFirst(ClaimTypes.GivenName));

                        if (info.Principal.HasClaim(c => c.Type == "urn:google:locale"))
                            await userManager.AddClaimAsync(user, info.Principal.FindFirst("urn:google:locale"));

                        if (info.Principal.HasClaim(c => c.Type == "urn:google:picture"))
                            await userManager.AddClaimAsync(user, info.Principal.FindFirst("urn:google:picture"));

                        var props = new AuthenticationProperties();
                        props.StoreTokens(info.AuthenticationTokens);
                        props.IsPersistent = true;
                        await signInManager.SignInAsync(user, props);
                        logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;

            return Page();
        }
        readonly SignInManager<CoreUser> signInManager;
        readonly UserManager<CoreUser> userManager;
        readonly ILogger<ExternalLoginModel> logger;
    }
}