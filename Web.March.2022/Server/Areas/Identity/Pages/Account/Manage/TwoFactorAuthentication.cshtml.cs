using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using ShareInvest.Server.Data.Models;

namespace ShareInvest.Server.Areas.Identity.Pages.Account.Manage
{
    public class TwoFactorAuthenticationModel : PageModel
    {
        public TwoFactorAuthenticationModel(UserManager<CoreUser> userManager, SignInManager<CoreUser> signInManager, ILogger<TwoFactorAuthenticationModel> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }
        public bool HasAuthenticator
        {
            get; set;
        }
        public int RecoveryCodesLeft
        {
            get; set;
        }
        [BindProperty]
        public bool Is2faEnabled
        {
            get; set;
        }
        public bool IsMachineRemembered
        {
            get; set;
        }
        [TempData]
        public string? StatusMessage
        {
            get; set;
        }
        public async Task<IActionResult> OnGet()
        {
            var user = await userManager.GetUserAsync(User);

            if (user is null)
            {
                logger.LogInformation(nameof(OnGet), User.Identity?.Name);

                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }
            HasAuthenticator = await userManager.GetAuthenticatorKeyAsync(user) is not null;
            Is2faEnabled = await userManager.GetTwoFactorEnabledAsync(user);
            IsMachineRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user);
            RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user);

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            var user = await userManager.GetUserAsync(User);

            if (user is null)
            {
                logger.LogInformation(nameof(OnPost), User.Identity?.Name);

                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }
            await signInManager.ForgetTwoFactorClientAsync();
            StatusMessage = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";

            return RedirectToPage();
        }
        readonly UserManager<CoreUser> userManager;
        readonly SignInManager<CoreUser> signInManager;
        readonly ILogger<TwoFactorAuthenticationModel> logger;
        const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}";
    }
}