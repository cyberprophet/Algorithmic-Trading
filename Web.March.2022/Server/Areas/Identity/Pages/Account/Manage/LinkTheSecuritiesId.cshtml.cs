using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Server.Data;
using ShareInvest.Server.Data.Models;
using ShareInvest.Security;

using System.Text;

namespace ShareInvest.Server.Areas.Identity.Pages.Account.Manage
{
    public partial class LinkTheSecuritiesIdModel : PageModel
    {
        public LinkTheSecuritiesIdModel(UserManager<CoreUser> manager, ILogger<LinkTheSecuritiesIdModel> logger, CoreContext context)
        {
            this.context = context;
            this.manager = manager;
            this.logger = logger;
        }
        public async Task<IActionResult> OnPostAsync([FromForm] int securities, [FromForm] string id)
        {
            try
            {
                if (securities > 0 && string.IsNullOrEmpty(id) is false && context.Securities is not null)
                {
                    foreach (var cf in from o in context.Securities.AsNoTracking()
                                       where string.IsNullOrEmpty(o.User) && o.Company == securities
                                       select new
                                       {
                                           o.Id,
                                           o.Key,
                                           o.Company
                                       })
                        if (cf.Id.Equals(Crypto.Encrypt(Encoding.UTF8.GetBytes(cf.Key), id)))
                            (await context.Securities.FindAsync(cf.Key, cf.Company))!.User = manager.GetUserId(User);

                    if (context.SaveChanges() > 0)
                        return Redirect("~/terms");
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(nameof(OnPostAsync), ex.Message);
            }
            return Page();
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                if (context.Securities is not null &&
                    await context.Securities.AsNoTracking().AnyAsync(o => manager.GetUserId(User).Equals(o.User)))
                    Securities = new List<Models.Securities>(from o in context.Securities.AsNoTracking()
                                                             where manager.GetUserId(User).Equals(o.User)
                                                             select o);
            }
            catch (Exception ex)
            {
                logger.LogInformation(nameof(OnGetAsync), ex.Message);
            }
            return Page();
        }
        public List<Models.Securities>? Securities
        {
            get; private set;
        }
        readonly CoreContext context;
        readonly UserManager<CoreUser> manager;
        readonly ILogger<LinkTheSecuritiesIdModel> logger;
    }
}