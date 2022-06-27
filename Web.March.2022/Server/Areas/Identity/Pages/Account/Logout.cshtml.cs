using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using ShareInvest.Server.Data.Models;

namespace ShareInvest.Server.Areas.Identity.Pages.Account
{
    [Authorize, IgnoreAntiforgeryToken]
    public partial class LogoutModel : PageModel
    {
        public LogoutModel(SignInManager<CoreUser> signInManager, ILogger<LogoutModel> logger)
        {
            this.signInManager = signInManager;
            this.logger = logger;
        }
        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPost()
        {
            if (signInManager.IsSignedIn(User))
            {
                await signInManager.SignOutAsync();
                logger.LogInformation("User logged out.");
            }
            return Redirect("~/");
        }
        readonly SignInManager<CoreUser> signInManager;
        readonly ILogger<LogoutModel> logger;
    }
}