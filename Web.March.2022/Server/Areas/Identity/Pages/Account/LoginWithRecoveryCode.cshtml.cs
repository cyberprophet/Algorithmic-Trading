using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using ShareInvest.Server.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ShareInvest.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginWithRecoveryCodeModel : PageModel
    {
        public LoginWithRecoveryCodeModel(SignInManager<CoreUser> signInManager, ILogger<LoginWithRecoveryCodeModel> logger)
        {
            this.signInManager = signInManager;
            this.logger = logger;
        }
        [BindProperty, AllowNull]
        public InputModel Input
        {
            get; set;
        }
        public string? ReturnUrl
        {
            get; set;
        }
        public class InputModel
        {
            [BindProperty, Required, DataType(DataType.Text), Display(Name = "Recovery Code")]
            public string? RecoveryCode
            {
                get; set;
            }
        }
        public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
        {
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");

            ReturnUrl = returnUrl;

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (ModelState.IsValid is false)
                return Page();

            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");

            var recoveryCode = Input.RecoveryCode?.Replace(" ", string.Empty);
            var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.Id);

                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            if (result.IsLockedOut)
            {
                logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);

                return RedirectToPage("./Lockout");
            }
            else
            {
                logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");

                return Page();
            }
        }
        readonly SignInManager<CoreUser> signInManager;
        readonly ILogger<LoginWithRecoveryCodeModel> logger;
    }
}