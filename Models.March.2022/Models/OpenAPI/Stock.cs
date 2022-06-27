using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ShareInvest.Models.OpenAPI
{
    public class Stock
    {
        [ForeignKey(nameof(Code))]
        public virtual CompanyOverview? Overview
        {
            get; set;
        }
        [DataMember, JsonProperty("종목코드"), Key, StringLength(6)]
        public string? Code
        {
            get; set;
        }
        [DataMember, JsonProperty("종목명"), StringLength(0x40)]
        public string? Name
        {
            get; set;
        }
        [DataMember, JsonProperty("현재가"), StringLength(0x10)]
        public string? Current
        {
            get; set;
        }
        [DataMember, JsonProperty("기준가"), StringLength(0x10)]
        public string? Price
        {
            get; set;
        }
        [DataMember, JsonProperty("전일대비"), StringLength(0x10)]
        public string? CompareToPreviousDay
        {
            get; set;
        }
        [DataMember, JsonProperty("전일대비기호"), StringLength(0x10)]
        public string? CompareToPreviousSign
        {
            get; set;
        }
        [DataMember, JsonProperty("등락율"), StringLength(0x10)]
        public string? Rate
        {
            get; set;
        }
        [DataMember, JsonProperty("거래량"), StringLength(0x10)]
        public string? Volume
        {
            get; set;
        }
        [DataMember, JsonProperty("거래대금"), StringLength(0x10)]
        public string? TransactionAmount
        {
            get; set;
        }
        [DataMember, JsonProperty("체결량"), StringLength(0x10)]
        public string? ContractAmount
        {
            get; set;
        }
        [DataMember, JsonProperty("체결강도"), StringLength(0x10)]
        public string? FasteningStrength
        {
            get; set;
        }
        [DataMember, JsonProperty("전일거래량대비"), StringLength(0x10)]
        public string? CompareToPreviousVolume
        {
            get; set;
        }
        [DataMember, JsonProperty("매도호가"), StringLength(0x10)]
        public string? Offer
        {
            get; set;
        }
        [DataMember, JsonProperty("매수호가"), StringLength(0x10)]
        public string? Bid
        {
            get; set;
        }
        [DataMember, JsonProperty("매도1차호가"), StringLength(0x10)]
        public string? OfferAlpha
        {
            get; set;
        }
        [DataMember, JsonProperty("매도2차호가"), StringLength(0x10)]
        public string? OfferBeta
        {
            get; set;
        }
        [DataMember, JsonProperty("매도3차호가"), StringLength(0x10)]
        public string? OfferGamma
        {
            get; set;
        }
        [DataMember, JsonProperty("매도4차호가"), StringLength(0x10)]
        public string? OfferDelta
        {
            get; set;
        }
        [DataMember, JsonProperty("매도5차호가"), StringLength(0x10)]
        public string? OfferEpsilon
        {
            get; set;
        }
        [DataMember, JsonProperty("매수1차호가"), StringLength(0x10)]
        public string? BidAlpha
        {
            get; set;
        }
        [DataMember, JsonProperty("매수2차호가"), StringLength(0x10)]
        public string? BidBeta
        {
            get; set;
        }
        [DataMember, JsonProperty("매수3차호가"), StringLength(0x10)]
        public string? BidGamma
        {
            get; set;
        }
        [DataMember, JsonProperty("매수4차호가"), StringLength(0x10)]
        public string? BidDelta
        {
            get; set;
        }
        [DataMember, JsonProperty("매수5차호가"), StringLength(0x10)]
        public string? BidEpsilon
        {
            get; set;
        }
        [DataMember, JsonProperty("상한가"), StringLength(0x10)]
        public string? UpperLimit
        {
            get; set;
        }
        [DataMember, JsonProperty("하한가"), StringLength(0x10)]
        public string? LowerLimit
        {
            get; set;
        }
        [DataMember, JsonProperty("시가"), StringLength(0x10)]
        public string? StartingPrice
        {
            get; set;
        }
        [DataMember, JsonProperty("고가"), StringLength(0x10)]
        public string? HighPrice
        {
            get; set;
        }
        [DataMember, JsonProperty("저가"), StringLength(0x10)]
        public string? LowPrice
        {
            get; set;
        }
        [DataMember, JsonProperty("종가"), StringLength(0x10)]
        public string? ClosingPrice
        {
            get; set;
        }
        [DataMember, JsonProperty("체결시간"), StringLength(0x10)]
        public string? FasteningTime
        {
            get; set;
        }
        [DataMember, JsonProperty("예상체결가"), StringLength(0x10)]
        public string? ExpectedPrice
        {
            get; set;
        }
        [DataMember, JsonProperty("예상체결량"), StringLength(0x10)]
        public string? EstimatedContractVolume
        {
            get; set;
        }
        [DataMember, JsonProperty("자본금"), StringLength(0x10)]
        public string? Capital
        {
            get; set;
        }
        [DataMember, JsonProperty("액면가"), StringLength(0x10)]
        public string? FaceValue
        {
            get; set;
        }
        [DataMember, JsonProperty("시가총액"), StringLength(0x10)]
        public string? MarketCap
        {
            get; set;
        }
        [DataMember, JsonProperty("주식수"), StringLength(0x10)]
        public string? NumberOfListedStocks
        {
            get; set;
        }
        [DataMember, JsonProperty("호가시간"), StringLength(0x10)]
        public string? QuoteTime
        {
            get; set;
        }
        [DataMember, JsonProperty("일자"), StringLength(0x10)]
        public string? Date
        {
            get; set;
        }
        [DataMember, JsonProperty("우선매도잔량"), StringLength(0x10)]
        public string? PreferredOfferRemaining
        {
            get; set;
        }
        [DataMember, JsonProperty("우선매수잔량"), StringLength(0x10)]
        public string? PreferredBidRemaining
        {
            get; set;
        }
        [DataMember, JsonProperty("우선매도건수"), StringLength(0x10)]
        public string? NumberOfPreferentialOffer
        {
            get; set;
        }
        [DataMember, JsonProperty("우선매수건수"), StringLength(0x10)]
        public string? NumberOfPreferentialBid
        {
            get; set;
        }
        [DataMember, JsonProperty("총매도잔량"), StringLength(0x10)]
        public string? TotalOfferRemaining
        {
            get; set;
        }
        [DataMember, JsonProperty("총매수잔량"), StringLength(0x10)]
        public string? TotalBidRemaining
        {
            get; set;
        }
        [DataMember, JsonProperty("총매도건수"), StringLength(0x10)]
        public string? NumberOfTotalOffer
        {
            get; set;
        }
        [DataMember, JsonProperty("총매수건수"), StringLength(0x10)]
        public string? NumberOfTotalBid
        {
            get; set;
        }
        [DataMember, JsonProperty("패리티"), StringLength(0x10)]
        public string? Parity
        {
            get; set;
        }
        [DataMember, JsonProperty("기어링"), StringLength(0x10)]
        public string? Gearing
        {
            get; set;
        }
        [DataMember, JsonProperty("손익분기"), StringLength(0x10)]
        public string? BreakEven
        {
            get; set;
        }
        [DataMember, JsonProperty("자본지지"), StringLength(0x10)]
        public string? CapitalSupport
        {
            get; set;
        }
        [DataMember, JsonProperty("ELW행사가"), StringLength(0x10)]
        public string? ELWEventPrice
        {
            get; set;
        }
        [DataMember, JsonProperty("전환비율"), StringLength(0x10)]
        public string? ConversionRate
        {
            get; set;
        }
        [DataMember, JsonProperty("ELW만기일"), StringLength(0x10)]
        public string? ELWDueDate
        {
            get; set;
        }
        [DataMember, JsonProperty("미결제약정"), StringLength(0x10)]
        public string? OpenInterest
        {
            get; set;
        }
        [DataMember, JsonProperty("미결제전일대비"), StringLength(0x10)]
        public string? ComparePreviousOutstanding
        {
            get; set;
        }
        [DataMember, JsonProperty("이론가"), StringLength(0x10)]
        public string? TheoreticalPrice
        {
            get; set;
        }
        [DataMember, JsonProperty("내재변동성"), StringLength(0x10)]
        public string? ImpliedVolatility
        {
            get; set;
        }
        [DataMember, JsonProperty("델타"), StringLength(0x10)]
        public string? Delta
        {
            get; set;
        }
        [DataMember, JsonProperty("감마"), StringLength(0x10)]
        public string? Gamma
        {
            get; set;
        }
        [DataMember, JsonProperty("쎄타"), StringLength(0x10)]
        public string? Theta
        {
            get; set;
        }
        [DataMember, JsonProperty("베가"), StringLength(0x10)]
        public string? Vega
        {
            get; set;
        }
        [DataMember, JsonProperty("로"), StringLength(0x10)]
        public string? Rho
        {
            get; set;
        }
        [StringLength(0x40)]
        public string? State
        {
            get; set;
        }
        [StringLength(0x80)]
        public string? ConstructionSupervision
        {
            get; set;
        }
        [StringLength(0x10)]
        public string? InvestmentCaution
        {
            get; set;
        }
        [StringLength(8)]
        public string? ListingDate
        {
            get; set;
        }
        [NotMapped]
        public bool IsShown
        {
            get; set;
        }
    }
}