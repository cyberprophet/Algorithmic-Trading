using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Interface;
using ShareInvest.Server.Data;

using System.Globalization;

namespace ShareInvest.Server.Controllers
{
    [Route("api/[controller]"), Produces("application/json"), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status400BadRequest), ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PostContextAsync([FromBody] Models.OpenAPI.Account account)
        {
            try
            {
                var now = DateTime.Now;
                account.Date = now.ToString("d", new CultureInfo("ko-KR"));
                account.Lookup = now.Ticks;

                if (context.Accounts is not null)
                {
                    if (await context.Accounts.FindAsync(account.Key, account.Date, account.AccNo, account.Company) is Models.OpenAPI.Account model)
                    {
                        model.AccumulatedInvestment = account.AccumulatedInvestment;
                        model.AccumulatedProfitAndLoss = account.AccumulatedProfitAndLoss;
                        model.Asset = account.Asset;
                        model.Balance = account.Balance;
                        model.Branch = account.Branch;
                        model.CumulativeProfitPercentage = account.CumulativeProfitPercentage;
                        model.CurrentMonthInvestment = account.CurrentMonthInvestment;
                        model.CurrentMonthProfitAndLoss = account.CurrentMonthProfitAndLoss;
                        model.Deposit = account.Deposit;
                        model.MortgageLoan = account.MortgageLoan;
                        model.Name = account.Name;
                        model.NumberOfPrints = account.NumberOfPrints;
                        model.PresumeAsset = account.PresumeAsset;
                        model.PresumeDeposit = account.PresumeDeposit;
                        model.ProfitAndLossPercentageForTheCurrentMonth = account.ProfitAndLossPercentageForTheCurrentMonth;
                        model.ProfitAndLossPercentageForTheDay = account.ProfitAndLossPercentageForTheDay;
                        model.SameDayInvestment = account.SameDayInvestment;
                        model.SameDayProfitAndLoss = account.SameDayProfitAndLoss;
                        model.TotalPurchaseAmount = account.TotalPurchaseAmount;
                        model.Lookup = account.Lookup;
                    }
                    else
                        context.Accounts.Add(account);

                    if (context.SaveChanges() is int changes && changes > 0 && context.Securities is not null)
                    {
                        if (string.IsNullOrEmpty(account.Key) is false &&
                            context.Securities.AsNoTracking().Single(o => account.Key.Equals(o.Key)).User is string user)
                            await hub.Clients.User(user).OnReceiveAccountMessage(account);

                        return Ok(new Initialization
                        {
                            Changes = changes,
                            Id = account.Key
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
        [HttpGet]
        public async Task<IActionResult> GetContextAsync([FromQuery] string? key)
        {
            try
            {
                if (string.IsNullOrEmpty(key) is false &&
                    context.Accounts is not null)
                {
                    if (await context.Accounts.AsNoTracking().AnyAsync(o => key.Equals(o.Key)) &&
                        context.Balances is not null)
                    {
                        var stack = new Stack<Models.OpenAPI.Account>();
                        var date = string.Empty;

                        foreach (var acc in from o in context.Accounts.AsNoTracking()
                                            where key.Equals(o.Key)
                                            orderby o.Lookup descending
                                            select o)
                        {
                            if ((string.IsNullOrEmpty(date) || date.Equals(acc.Date)) &&
                                string.IsNullOrEmpty(acc.AccNo) is false &&
                                string.IsNullOrEmpty(acc.Date) is false)
                            {
                                date = acc.Date;
                                acc.Balances = (from o in context.Balances.AsNoTracking()
                                                where acc.AccNo.Equals(o.AccNo) && acc.Date.Equals(o.Date)
                                                select o).ToArray();
                                acc.Acc = acc.AccNo[..^2].Insert(4, "-");
                                stack.Push(acc);

                                continue;
                            }
                            break;
                        }
                        return Ok(stack);
                    }
                    else
                        return Ok(Array.Empty<Models.OpenAPI.Account>());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }
        public AccountController(CoreContext context, IHubContext<Hubs.Message, IHubs> hub)
        {
            this.hub = hub;
            this.context = context;
        }
        readonly IHubContext<Hubs.Message, IHubs> hub;
        readonly CoreContext context;
    }
}