using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using ShareInvest.Server.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ShareInvest.Server.Areas.Identity.Pages.Account.Manage
{
    public class SetPasswordModel : PageModel
    {
        public SetPasswordModel(UserManager<CoreUser> userManager, SignInManager<CoreUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [BindProperty, AllowNull]
        public InputModel Input
        {
            get; set;
        }
        [TempData]
        public string? StatusMessage
        {
            get; set;
        }
        public class InputModel
        {
            [Required, StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6), DataType(DataType.Password), Display(Name = "New password")]
            public string? NewPassword
            {
                get; set;
            }
            [DataType(DataType.Password), Display(Name = "Confirm new password"), Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string? ConfirmPassword
            {
                get; set;
            }
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

            var hasPassword = await userManager.HasPasswordAsync(user);

            if (hasPassword)
                return RedirectToPage("./ChangePassword");

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid is false)
                return Page();

            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

            var addPasswordResult = await userManager.AddPasswordAsync(user, Input.NewPassword);

            if (addPasswordResult.Succeeded is false)
            {
                foreach (var error in addPasswordResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }
            await signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your password has been set.";

            return RedirectToPage();
        }
        readonly UserManager<CoreUser> userManager;
        readonly SignInManager<CoreUser> signInManager;
    }
}