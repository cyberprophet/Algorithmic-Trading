using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Interface;
using ShareInvest.Server.Data;

using System.Globalization;

namespace ShareInvest.Server.Controllers
{
    [Route("api/[controller]"), Produces("application/json"), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status400BadRequest), ApiController]
    public class BalanceController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PostContextAsync([FromBody] Models.OpenAPI.Balance balance)
        {
            try
            {
                var now = DateTime.Now;
                balance.Date = now.ToString("d", new CultureInfo("ko-KR"));
                balance.Lookup = now.Ticks;

                if (context.Balances is not null)
                {
                    if (await context.Balances.FindAsync(balance.Key, balance.Code, balance.Date, balance.AccNo, balance.Company) is Models.OpenAPI.Balance model)
                    {
                        model.Amount = balance.Amount;
                        model.Average = balance.Average;
                        model.Current = balance.Current;
                        model.Evaluation = balance.Evaluation;
                        model.Loan = balance.Loan;
                        model.Lookup = balance.Lookup;
                        model.Name = balance.Name;
                        model.PaymentBalance = balance.PaymentBalance;
                        model.PreviousPurchaseQuantity = balance.PreviousPurchaseQuantity;
                        model.PreviousSalesQuantity = balance.PreviousSalesQuantity;
                        model.Purchase = balance.Purchase;
                        model.PurchaseQuantity = balance.PurchaseQuantity;
                        model.Quantity = balance.Quantity;
                        model.Rate = balance.Rate;
                        model.SalesQuantity = balance.SalesQuantity;
                    }
                    else
                        context.Balances.Add(balance);

                    if (context.SaveChanges() is int changes && changes > 0)
                    {
                        if (string.IsNullOrEmpty(balance.Key) is false &&
                            context.Securities is not null &&
                            context.Securities.AsNoTracking().Single(o => balance.Key.Equals(o.Key)).User is string user)
                            await hub.Clients.User(user).OnReceiveBalanceMessage(balance);

                        return Ok(new Initialization
                        {
                            Changes = changes,
                            Id = balance.Key
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }
        [HttpGet, Authorize]
        public async Task<IActionResult> GetContextAsync([FromQuery] string? key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return Ok(await GetContextAsync());

                else
                {

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }
        public BalanceController(CoreContext context, IHubContext<Hubs.Message, IHubs> hub)
        {
            this.context = context;
            this.hub = hub;
        }
        async Task<IEnumerable<Models.OpenAPI.Balance>> GetContextAsync()
        {
            if (context.Balances is not null)
            {
                var recent = await context.Balances.AsNoTracking().MaxAsync(o => o.Date);

                if (string.IsNullOrEmpty(recent) is false)
                    return from o in context.Balances.AsNoTracking()
                           where recent.Equals(o.Date)
                           orderby o.Lookup
                           select o;
            }
            return Array.Empty<Models.OpenAPI.Balance>();
        }
        readonly CoreContext context;
        readonly IHubContext<Hubs.Message, IHubs> hub;
    }
}