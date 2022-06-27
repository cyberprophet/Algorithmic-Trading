using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using ShareInvest.Server.Data.Models;

namespace ShareInvest.Server.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        public PersonalDataModel(UserManager<CoreUser> userManager, ILogger<PersonalDataModel> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }
        public async Task<IActionResult> OnGet()
        {
            var user = await userManager.GetUserAsync(User);

            if (user is not null)
            {
                logger.LogInformation(nameof(OnGet), User.Identity?.Name);

                return Page();
            }
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }
        readonly UserManager<CoreUser> userManager;
        readonly ILogger<PersonalDataModel> logger;
    }
}