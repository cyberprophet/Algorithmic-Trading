namespace ShareInvest.Interface.OpenAPI
{
    public class OPW00004 : TR
    {
        public override string[]? Value
        {
            get; set;
        }
        public override string? RQName
        {
            set
            {

            }
            get => "계좌평가현황요청";
        }
        public override string TrCode => nameof(OPW00004);
        public override int PrevNext
        {
            get; set;
        }
        public override string ScreenNo => LookupScreenNo;
        public override string[] Id => new[]
        {
            "계좌번호",
            "비밀번호",
            "상장폐지조회구분",
            "비밀번호입력매체구분"
        };
        public override string[] Single => new[]
        {
            "계좌명",
            "지점명",
            "예수금",
            "D+2추정예수금",
            "유가잔고평가액",
            "예탁자산평가액",
            "총매입금액",
            "추정예탁자산",
            "매도담보대출금",
            "당일투자원금",
            "당월투자원금",
            "누적투자원금",
            "당일투자손익",
            "당월투자손익",
            "누적투자손익",
            "당일손익율",
            "당월손익율",
            "누적손익율",
            "출력건수"
        };
        public override string[] Multiple => new[]
        {
            "종목코드",
            "종목명",
            "보유수량",
            "평균단가",
            "현재가",
            "평가금액",
            "손익금액",
            "손익율",
            "대출일",
            "매입금액",
            "결제잔고",
            "전일매수수량",
            "전일매도수량",
            "금일매수수량",
            "금일매도수량"
        };
    }
}