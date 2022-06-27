using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using ShareInvest.Server.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;

namespace ShareInvest.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        public ForgotPasswordModel(UserManager<CoreUser> userManager, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.emailSender = emailSender;
        }
        [BindProperty, AllowNull]
        public InputModel Input
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
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(Input.Email);

                if (user == null || (await userManager.IsEmailConfirmedAsync(user)) is false)
                    return RedirectToPage("./ForgotPasswordConfirmation");

                var code = await userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page("/Account/ResetPassword", pageHandler: null, values: new
                {
                    code
                },
                protocol: Request.Scheme);

                await emailSender.SendEmailAsync(Input.Email, "Reset Password", $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }
            return Page();
        }
        readonly UserManager<CoreUser> userManager;
        readonly IEmailSender emailSender;
    }
}