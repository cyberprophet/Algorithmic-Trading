using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Server.Data;
using ShareInvest.Server.Data.Models;

namespace ShareInvest.Server.Areas.Identity.Pages.Account.Manage
{
    public class GrantAdminPrivilegesModel : PageModel
    {
        public GrantAdminPrivilegesModel(CoreContext context, UserManager<CoreUser> manager, ILogger<GrantAdminPrivilegesModel> logger)
        {
            this.context = context;
            this.manager = manager;
            this.logger = logger;
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
        public async Task<IActionResult> OnPostAsync([FromForm] string admin)
        {
            try
            {
                if (context.Securities is not null &&
                    await context.Securities.AsNoTracking().AnyAsync(o => manager.GetUserId(User).Equals(o.User)))
                {
                    foreach (var sa in from o in context.Securities.AsNoTracking()
                                       where manager.GetUserId(User).Equals(o.User)
                                       select new
                                       {
                                           o.Company,
                                           o.Key
                                       })
                        if (context.Securities.Find(sa.Key, sa.Company) is Models.Securities model)
                            model.IsAdministrator = admin.Equals(sa.Key);

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
        public List<Models.Securities>? Securities
        {
            get; private set;
        }
        readonly CoreContext context;
        readonly UserManager<CoreUser> manager;
        readonly ILogger<GrantAdminPrivilegesModel> logger;
    }
}