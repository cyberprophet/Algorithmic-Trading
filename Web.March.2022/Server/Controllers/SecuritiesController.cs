using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Server.Data;

namespace ShareInvest.Server.Controllers
{
    [Route("api/[controller]"), Produces("application/json"), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status400BadRequest), ApiController]
    public class SecuritiesController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PostContextAsync([FromBody] Models.Securities securities)
        {
            try
            {
                if (context.Securities is not null)
                {
                    if (await context.Securities.FindAsync(securities.Key, securities.Company) is Models.Securities model)
                    {
                        if (string.IsNullOrEmpty(securities.Name) is false)
                            model.Name = securities.Name;

                        if (string.IsNullOrEmpty(securities.Id) is false)
                            model.Id = securities.Id;

                        model.Count = securities.Count;
                    }
                    else
                        context.Securities.Add(securities);

                    if (string.IsNullOrEmpty(securities.Key) is false &&
                        string.IsNullOrEmpty(securities.Id) is false)
                        return Ok(new Interface.Initialization
                        {
                            Changes = context.SaveChanges(),
                            Id = context.Securities.AsNoTracking().Single(o => securities.Company == o.Company && securities.Id.Equals(o.Id) && securities.Key.Equals(o.Key)).User
                        });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetContextAsync([FromQuery] string? key, [FromQuery] string? user, [FromQuery] string? id, [FromQuery] int company)
        {
            try
            {
                if (context.Securities is not null &&
                    context.Securities.AsNoTracking() is IQueryable<Models.Securities> models)
                {
                    if (company > 0 && string.IsNullOrEmpty(key) is false)
                    {
                        var admin = models.Single(o => key.Equals(o.Key) && o.Company == company);

                        return Ok(new Interface.Admin
                        {
                            IsAdministrator = admin.IsAdministrator,
                            Id = admin.Id
                        });
                    }
                    if (string.IsNullOrEmpty(key) is false && await models.AnyAsync(o => key.Equals(o.Key)))
                        return Ok(models.Single(o => key.Equals(o.Key)));

                    if (string.IsNullOrEmpty(user) is false && await models.AnyAsync(o => user.Equals(o.Id)))
                        return Ok(models.Single(o => user.Equals(o.Id)));

                    if (string.IsNullOrEmpty(id) is false && await models.AnyAsync(o => id.Equals(o.User)))
                        return Ok(from o in models where id.Equals(o.User) select o);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }
        public SecuritiesController(CoreContext context) => this.context = context;
        readonly CoreContext context;
    }
}