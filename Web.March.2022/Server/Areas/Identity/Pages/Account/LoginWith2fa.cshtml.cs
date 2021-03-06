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
    public class LoginWith2faModel : PageModel
    {
        public LoginWith2faModel(SignInManager<CoreUser> signInManager, ILogger<LoginWith2faModel> logger)
        {
            this.signInManager = signInManager;
            this.logger = logger;
        }
        [BindProperty, AllowNull]
        public InputModel Input
        {
            get; set;
        }
        public bool RememberMe
        {
            get; set;
        }
        public string? ReturnUrl
        {
            get; set;
        }
        public class InputModel
        {
            [Required, StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6), DataType(DataType.Text), Display(Name = "Authenticator code")]
            public string? TwoFactorCode
            {
                get; set;
            }
            [Display(Name = "Remember this machine")]
            public bool RememberMachine
            {
                get; set;
            }
        }
        public async Task<IActionResult> OnGetAsync(bool rememberMe, string? returnUrl = null)
        {
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");

            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(bool rememberMe, string? returnUrl = null)
        {
            if (ModelState.IsValid is false)
                return Page();

            returnUrl ??= Url.Content("~/");
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");

            var authenticatorCode = Input.TwoFactorCode?.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result = await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);

            if (result.Succeeded)
            {
                logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);

                return LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);

                return RedirectToPage("./Lockout");
            }
            else
            {
                logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");

                return Page();
            }
        }
        readonly SignInManager<CoreUser> signInManager;
        readonly ILogger<LoginWith2faModel> logger;
    }
}