using Microsoft.AspNetCore.Authentication;
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
    public class LogInModel : PageModel
    {
        public LogInModel(SignInManager<CoreUser> manager, ILogger<LogInModel> logger)
        {
            this.manager = manager;
            this.logger = logger;
        }
        [BindProperty, AllowNull]
        public InputModel Input
        {
            get; set;
        }
        [AllowNull]
        public IList<AuthenticationScheme> ExternalLogins
        {
            get; set;
        }
        public string? ReturnUrl
        {
            get; set;
        }
        [TempData, AllowNull]
        public string ErrorMessage
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
            [Required, DataType(DataType.Password)]
            public string? Password
            {
                get; set;
            }
            [Display(Name = "Remember me?")]
            public bool RememberMe
            {
                get; set;
            }
        }
        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(ErrorMessage) is false)
                ModelState.AddModelError(string.Empty, ErrorMessage);

            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ExternalLogins = (await manager.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await manager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    logger.LogInformation("User logged in.");

                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                    return RedirectToPage("./LoginWith2fa", new
                    {
                        ReturnUrl = returnUrl,
                        Input.RememberMe
                    });
                if (result.IsLockedOut)
                {
                    logger.LogWarning("User account locked out.");

                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                    return Page();
                }
            }
            return Page();
        }
        readonly SignInManager<CoreUser> manager;
        readonly ILogger<LogInModel> logger;
    }
}