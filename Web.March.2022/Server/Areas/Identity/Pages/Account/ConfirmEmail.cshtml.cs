using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using ShareInvest.Server.Data.Models;

namespace ShareInvest.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        public ConfirmEmailModel(UserManager<CoreUser> userManager) => this.userManager = userManager;
        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
                return RedirectToPage("/");

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound($"Unable to load user with ID '{userId}'.");

            if ((await userManager.ConfirmEmailAsync(user, code)).Succeeded is false)
                throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");

            return Page();
        }
        readonly UserManager<CoreUser> userManager;
    }
}