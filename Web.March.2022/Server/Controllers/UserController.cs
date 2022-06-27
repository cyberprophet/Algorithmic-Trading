using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Server.Data;
using ShareInvest.Server.Data.Models;

namespace ShareInvest.Server.Controllers
{
    [Route("api/[controller]"), Produces("application/json"), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status400BadRequest), ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetContextAsync([FromQuery] string? name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {

                }
                else
                {
                    if (context.Users.AsNoTracking() is IQueryable<CoreUser> models &&
                        await models.AnyAsync(o => name.Equals(o.UserName)) &&
                        context.Securities is not null)
                        return Ok(from o in context.Securities.AsNoTracking()
                                  where models.Single(o => name.Equals(o.UserName)).Id.Equals(o.User)
                                  select new Interface.Initialization
                                  {
                                      Id = o.Key,
                                      Changes = o.Company
                                  });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }
        public UserController(CoreContext context) => this.context = context;
        readonly CoreContext context;
    }
}