using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Server.Data;

namespace ShareInvest.Server.Controllers
{
    [Route("api/[controller]"), Produces("application/json"), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status400BadRequest), ApiController]
    public class StockController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetContextAsync([FromQuery] string page)
        {
            try
            {
                if (context.Stocks is not null)
                {
                    if (page.Length == 6)
                        return Ok(context.Stocks.AsNoTracking().Single(o => page.Equals(o.Code)));

                    if (context.Overviews is not null &&
                        await context.Stocks.AsNoTracking().MaxAsync(o => o.Date) is string today)
                        return Ok(page switch
                        {
                            "left" => from o in context.Stocks.AsNoTracking()
                                      where today.Equals(o.Date)
                                      join v in context.Overviews.AsNoTracking() on o.Code equals v.Code into grouping
                                      from v in grouping.DefaultIfEmpty()
                                      select new
                                      {
                                          Overview = v,
                                          o.Code,
                                          o.Name,
                                          o.Current,
                                          o.UpperLimit,
                                          o.LowerLimit,
                                          o.StartingPrice,
                                          o.HighPrice,
                                          o.LowPrice,
                                          o.CompareToPreviousSign,
                                          o.CompareToPreviousDay,
                                          o.Rate,
                                          o.Volume,
                                          o.TransactionAmount,
                                          o.State
                                      },
                            "stock" => from o in context.Stocks.AsNoTracking()
                                       where today.Equals(o.Date)
                                       select new
                                       {
                                           o.Code,
                                           o.Name,
                                           o.Current,
                                           o.Rate,
                                           o.CompareToPreviousDay,
                                           o.CompareToPreviousSign,
                                           o.Volume,
                                           o.TransactionAmount,
                                           o.State,
                                           o.InvestmentCaution
                                       },
                            "map" => from o in context.Stocks.AsNoTracking()
                                     where today.Equals(o.Date)
                                     join c in context.Overviews.AsNoTracking() on o.Code equals c.Code
                                     select new
                                     {
                                         Overview = c,
                                         o.Current,
                                         o.Code,
                                         o.Name,
                                         o.CompareToPreviousSign,
                                         o.CompareToPreviousDay,
                                         o.Rate,
                                         o.FasteningTime,
                                         o.Date,
                                         o.ListingDate,
                                         o.Price
                                     },
                            nameof(Models.CompanyOverview.Code) => from o in context.Stocks.AsNoTracking()
                                                                   where today.Equals(o.Date)
                                                                   select o.Code,
                            _ => Array.Empty<object>()
                        });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return NoContent();
        }
        [HttpPut]
        public async Task<IActionResult> PutContextAsync([FromBody] Models.CompanyOverview company)
        {
            try
            {
                if (context.Stocks is not null &&
                    context.Overviews is not null &&
                    await context.Stocks.FindAsync(company.Code) is Models.OpenAPI.Stock model)
                {
                    if (string.IsNullOrEmpty(company.Address))
                    {
                        if (context.Overviews.Find(company.Code) is Models.CompanyOverview co)
                        {
                            co.Date = DateTime.Now;
                            co.CorpCode = company.CorpCode;
                            co.CorpName = company.CorpName;
                            co.ModifyDate = company.ModifyDate;
                        }
                        else
                            model.Overview = company;
                    }
                    else
                    {
                        if (context.Overviews.Find(company.Code) is Models.CompanyOverview co &&
                            company.Address.Equals(co.Address) &&
                            co.Longitude > 0 &&
                            co.Latitude > 0)
                        {
                            if (context.Overviews.AsNoTracking().Count(o => co.Latitude == o.Latitude && co.Longitude == o.Longitude) > 1)
                            {
                                if (await Condition.Kakao.GetLocalAddressAsync(co.Address) is Interface.Kakao.LocalAddress address)
                                    switch (address.Document.Length)
                                    {
                                        case 1 when
                                        double.TryParse(address.Document[0].Latitude, out double latitude) &&
                                        double.TryParse(address.Document[0].Longitude, out double longitude):
                                            co.Status = nameof(Interface.Kakao);
                                            co.Message = address.Document[0].Road.Name;
                                            co.Latitude = latitude - 1e-4 * new Random().Next(-2, 3);
                                            co.Longitude = longitude + 1e-4 * new Random().Next(-2, 3);
                                            break;

                                        case > 1 when address.Document.Count(o => string.IsNullOrEmpty(o.Road.BuildingName) is false) == 1:
                                            var lo = address.Document.Single(o => string.IsNullOrEmpty(o.Road.BuildingName) is false);

                                            if (double.TryParse(lo.Latitude, out double lati) && double.TryParse(lo.Longitude, out double longi))
                                            {
                                                co.Status = nameof(Interface.Kakao);
                                                co.Message = lo.Road.Name;
                                                co.Latitude = lati + 1e-4 * new Random().Next(-2, 3);
                                                co.Longitude = longi - 1e-4 * new Random().Next(-2, 3);
                                            }
                                            break;

                                        default:
                                            co.Latitude += 1e-4 * new Random().Next(-2, 3);
                                            co.Longitude -= 1e-4 * new Random().Next(-2, 3);
                                            break;
                                    }
                                else
                                {
                                    co.Latitude += 1e-4 * new Random().Next(-2, 3);
                                    co.Longitude -= 1e-4 * new Random().Next(-2, 3);
                                }
                            }
                            co.Date = DateTime.Now;
                            co.CorpCode = company.CorpCode;
                            co.CorpName = company.CorpName;
                            co.CorpEngName = company.CorpEngName;
                            co.Name = company.Name;
                            co.CEO = company.CEO;
                            co.Classification = company.Classification;
                            co.LegalRegistrationNumber = company.LegalRegistrationNumber;
                            co.CorporateRegistrationNumber = company.CorporateRegistrationNumber;
                            co.Url = company.Url;
                            co.IR = company.IR;
                            co.Phone = company.Phone;
                            co.Fax = company.Fax;
                            co.IndutyCode = company.IndutyCode;
                            co.FoundingDate = company.FoundingDate;
                            co.SettlementMonth = company.SettlementMonth;
                            co.ModifyDate = company.ModifyDate;
                        }
                        else
                        {
                            var geo = await Geo.GetLocation(company.Address);

                            if (geo.Item3 == 0D && geo.Item4 == 0D || double.IsNaN(geo.Item3) && double.IsNaN(geo.Item4))
                                geo = await Geo.GetLocation(company.Address.Split('(')[0]);

                            if (double.IsNaN(geo.Item3) is false && double.IsNaN(geo.Item4) is false)
                            {
                                if (context.Overviews.AsNoTracking().Count(o => geo.Item3 == o.Latitude && geo.Item4 == o.Longitude) > 1)
                                {
                                    company.Latitude = geo.Item3 + 1e-4 * new Random().Next(-2, 3);
                                    company.Longitude = geo.Item4 - 1e-4 * new Random().Next(-2, 3);
                                }
                                else
                                {
                                    company.Latitude = geo.Item3;
                                    company.Longitude = geo.Item4;
                                }
                                company.Status = geo.Item1;
                                company.Message = geo.Item2;
                            }
                            company.Date = DateTime.Now;
                            model.Overview = company;
                        }
                    }
                    return Ok(context.SaveChanges());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> PostContextAsync([FromBody] Models.OpenAPI.Stock stock)
        {
            try
            {
                if (context.Stocks is not null)
                {
                    if (await context.Stocks.FindAsync(stock.Code) is Models.OpenAPI.Stock model)
                    {
                        model.Bid = stock.Bid;
                        model.BidAlpha = stock.BidAlpha;
                        model.BidBeta = stock.BidBeta;
                        model.BidDelta = stock.BidDelta;
                        model.BidEpsilon = stock.BidEpsilon;
                        model.BidGamma = stock.BidGamma;
                        model.BreakEven = stock.BreakEven;
                        model.Capital = stock.Capital;
                        model.CapitalSupport = stock.CapitalSupport;
                        model.ClosingPrice = stock.ClosingPrice;
                        model.ComparePreviousOutstanding = stock.ComparePreviousOutstanding;
                        model.CompareToPreviousDay = stock.CompareToPreviousDay;
                        model.CompareToPreviousSign = stock.CompareToPreviousSign;
                        model.CompareToPreviousVolume = stock.CompareToPreviousVolume;
                        model.ConstructionSupervision = stock.ConstructionSupervision;
                        model.ContractAmount = stock.ContractAmount;
                        model.ConversionRate = stock.ConversionRate;
                        model.Current = stock.Current;
                        model.Date = stock.Date;
                        model.Delta = stock.Delta;
                        model.ELWDueDate = stock.ELWDueDate;
                        model.ELWEventPrice = stock.ELWEventPrice;
                        model.EstimatedContractVolume = stock.EstimatedContractVolume;
                        model.ExpectedPrice = stock.ExpectedPrice;
                        model.FaceValue = stock.FaceValue;
                        model.FasteningStrength = stock.FasteningStrength;
                        model.FasteningTime = stock.FasteningTime;
                        model.Gamma = stock.Gamma;
                        model.Gearing = stock.Gearing;
                        model.HighPrice = stock.HighPrice;
                        model.ImpliedVolatility = stock.ImpliedVolatility;
                        model.InvestmentCaution = stock.InvestmentCaution;
                        model.ListingDate = stock.ListingDate;
                        model.LowerLimit = stock.LowerLimit;
                        model.LowPrice = stock.LowPrice;
                        model.MarketCap = stock.MarketCap;
                        model.Name = stock.Name;
                        model.NumberOfListedStocks = stock.NumberOfListedStocks;
                        model.NumberOfPreferentialBid = stock.NumberOfPreferentialBid;
                        model.NumberOfPreferentialOffer = stock.NumberOfPreferentialOffer;
                        model.NumberOfTotalBid = stock.NumberOfTotalBid;
                        model.NumberOfTotalOffer = stock.NumberOfTotalOffer;
                        model.Offer = stock.Offer;
                        model.OfferAlpha = stock.OfferAlpha;
                        model.OfferBeta = stock.OfferBeta;
                        model.OfferDelta = stock.OfferDelta;
                        model.OfferEpsilon = stock.OfferEpsilon;
                        model.OfferGamma = stock.OfferGamma;
                        model.OpenInterest = stock.OpenInterest;
                        model.Parity = stock.Parity;
                        model.PreferredBidRemaining = stock.PreferredBidRemaining;
                        model.PreferredOfferRemaining = stock.PreferredOfferRemaining;
                        model.Price = stock.Price;
                        model.QuoteTime = stock.QuoteTime;
                        model.Rate = stock.Rate;
                        model.Rho = stock.Rho;
                        model.StartingPrice = stock.StartingPrice;
                        model.State = stock.State;
                        model.TheoreticalPrice = stock.TheoreticalPrice;
                        model.Theta = stock.Theta;
                        model.TotalBidRemaining = stock.TotalBidRemaining;
                        model.TotalOfferRemaining = stock.TotalOfferRemaining;
                        model.TransactionAmount = stock.TransactionAmount;
                        model.UpperLimit = stock.UpperLimit;
                        model.Vega = stock.Vega;
                        model.Volume = stock.Volume;
                    }
                    else
                        context.Stocks.Add(stock);

                    return Ok(new Interface.Initialization
                    {
                        Changes = context.SaveChanges(),
                        Id = nameof(StockController)
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }
        public StockController(CoreContext context) => this.context = context;
        readonly CoreContext context;
    }
}