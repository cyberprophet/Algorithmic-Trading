using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Server.Data;

namespace ShareInvest.Server.Controllers
{
    [Route("api/[controller]"), Produces("application/json"), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status400BadRequest), ApiController]
    public class ChartController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetContextAsync([FromQuery] string account)
        {
            try
            {
                if (context.Accounts is not null &&
                    context.Balances is not null &&
                    context.Accounts.AsNoTracking() is IQueryable<Models.OpenAPI.Account> models &&
                    await models.AnyAsync(o => account.Equals(o.AccNo)))
                {
                    var res = new Stack<Models.OpenAPI.Account>();

                    foreach (var acc in from o in models
                                        where account.Equals(o.AccNo)
                                        select o)
                    {
                        if (string.IsNullOrEmpty(acc.Date) is false)
                            acc.Balances = (from o in context.Balances.AsNoTracking()
                                            where account.Equals(o.AccNo) && acc.Date.Equals(o.Date)
                                            select o)
                                            .ToArray();
                        res.Push(acc);
                    }
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return NoContent();
        }
        public ChartController(CoreContext context) => this.context = context;
        readonly CoreContext context;
    }
}