using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Newtonsoft.Json;

using ShareInvest.Server.Data.Models;

using System.Text;

namespace ShareInvest.Server.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        public DownloadPersonalDataModel(UserManager<CoreUser> userManager, ILogger<DownloadPersonalDataModel> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

            logger.LogInformation("User with ID '{UserId}' asked for their personal data.", userManager.GetUserId(User));
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(CoreUser).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));

            foreach (var p in personalDataProps)
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");

            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");

            return new FileContentResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData)), "text/json");
        }
        readonly UserManager<CoreUser> userManager;
        readonly ILogger<DownloadPersonalDataModel> logger;
    }
}