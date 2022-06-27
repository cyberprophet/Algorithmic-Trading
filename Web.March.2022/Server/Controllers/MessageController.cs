using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Interface;
using ShareInvest.Server.Data;

namespace ShareInvest.Server.Controllers
{
    [Route("api/[controller]"), Produces("application/json"), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status400BadRequest), ApiController]
    public class MessageController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PostContextAsync([FromBody] Models.OpenAPI.Message message)
        {
            try
            {
                if (context.Messages is not null)
                {
                    var now = DateTime.Now;
                    message.Lookup = now.Ticks;

                    if (await context.Messages.FindAsync(message.Key, message.Lookup, message.Company) is null)
                        context.Messages.Add(message);

                    if (context.SaveChanges() > 0)
                    {
                        if (string.IsNullOrEmpty(message.Key) is false)
                        {
                            var user = context?.Securities?.AsNoTracking().Single(o => o.Company == message.Company && message.Key.Equals(o.Key)).User;

                            if (string.IsNullOrEmpty(user) is false)
                                await hub.Clients.User(user).OnReceiveStringMessage($"{now:T}  [{message.Code}] {message.Title}");
                        }
                        return Ok(message);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }
        public MessageController(CoreContext context, IHubContext<Hubs.Message, IHubs> hub)
        {
            this.context = context;
            this.hub = hub;
        }
        readonly IHubContext<Hubs.Message, IHubs> hub;
        readonly CoreContext context;
    }
}