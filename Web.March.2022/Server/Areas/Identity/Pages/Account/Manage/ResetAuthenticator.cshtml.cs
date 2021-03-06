using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using ShareInvest.Server.Data.Models;

namespace ShareInvest.Server.Areas.Identity.Pages.Account.Manage
{
    public class ResetAuthenticatorModel : PageModel
    {
        public ResetAuthenticatorModel(UserManager<CoreUser> userManager, SignInManager<CoreUser> signInManager, ILogger<ResetAuthenticatorModel> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }
        [TempData]
        public string? StatusMessage
        {
            get; set;
        }
        public async Task<IActionResult> OnGet()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

            await userManager.SetTwoFactorEnabledAsync(user, false);
            await userManager.ResetAuthenticatorKeyAsync(user);
            logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);
            await signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

            return RedirectToPage("./EnableAuthenticator");
        }
        readonly UserManager<CoreUser> userManager;
        readonly SignInManager<CoreUser> signInManager;
        readonly ILogger<ResetAuthenticatorModel> logger;
    }
}