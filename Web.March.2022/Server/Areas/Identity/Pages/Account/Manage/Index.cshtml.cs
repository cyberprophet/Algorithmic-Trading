using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using ShareInvest.Server.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;

namespace ShareInvest.Server.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        public IndexModel(UserManager<CoreUser> userManager, SignInManager<CoreUser> signInManager, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
        }
        public string? Username
        {
            get; set;
        }
        public bool IsEmailConfirmed
        {
            get; set;
        }
        [TempData]
        public string? StatusMessage
        {
            get; set;
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
            [Phone, Display(Name = "Phone number")]
            public string? PhoneNumber
            {
                get; set;
            }
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

            var userName = await userManager.GetUserNameAsync(user);
            var email = await userManager.GetEmailAsync(user);
            var phoneNumber = await userManager.GetPhoneNumberAsync(user);
            Username = userName;
            Input = new InputModel
            {
                Email = email,
                PhoneNumber = phoneNumber
            };
            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid is false)
                return Page();

            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

            var email = await userManager.GetEmailAsync(user);

            if (Input.Email != email)
            {
                var setEmailResult = await userManager.SetEmailAsync(user, Input.Email);

                if (setEmailResult.Succeeded is false)
                {
                    var userId = await userManager.GetUserIdAsync(user);

                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                }
            }
            var phoneNumber = await userManager.GetPhoneNumberAsync(user);

            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);

                if (setPhoneResult.Succeeded is false)
                {
                    var userId = await userManager.GetUserIdAsync(user);

                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }
            await signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (ModelState.IsValid is false)
                return Page();

            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

            var userId = await userManager.GetUserIdAsync(user);
            var email = await userManager.GetEmailAsync(user);
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new
            {
                userId,
                code
            },
            protocol: Request.Scheme);

            await emailSender.SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");
            StatusMessage = "Verification email sent. Please check your email.";

            return RedirectToPage();
        }
        readonly UserManager<CoreUser> userManager;
        readonly SignInManager<CoreUser> signInManager;
        readonly IEmailSender emailSender;
    }
}