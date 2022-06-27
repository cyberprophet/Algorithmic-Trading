namespace ShareInvest.Interface.OpenAPI
{
    public class Opt10081 : TR
    {
        public const string rqName = "주식일봉차트조회요청";
        public override string[] Id => new[]
        {
            "종목코드",
            "기준일자",
            "수정주가구분"
        };
        public override string[]? Value
        {
            get; set;
        }
        public override string? RQName
        {
            get; set;
        }
        public override string TrCode => nameof(Opt10081).ToLower();
        public override int PrevNext
        {
            get; set;
        }
        public override string ScreenNo => LookupScreenNo;
        public override string[] Multiple => new[]
        {
            "종목코드",
            "현재가",
            "거래량",
            "거래대금",
            "일자",
            "시가",
            "고가",
            "저가",
            "수정주가구분",
            "수정비율",
            "대업종구분",
            "소업종구분",
            "종목정보",
            "수정주가이벤트",
            "전일종가"
        };
        public override string[] Single => new[] { "종목코드" };
    }
}