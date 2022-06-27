namespace ShareInvest.Interface
{
    public struct Hermes
    {
        public Method Method
        {
            get; set;
        }
        public string Parameter
        {
            get; set;
        }
    }
    public enum Method
    {
        Account = 'A',
        Balance = 'B',
        Company = 'C',
        Error = 'E',
        LoginInfo = 'L',
        Message = 'M',
        Exit = 'T',
        OPW00004 = 'W' + 4,
        OPTKWFID = 'T' + 0x100,
        opt10004 = 't' + 0x100 + 4,
        opt10081 = 't' + 0x100 + 81,
        주식체결 = 0x1000 + 'C',
        주식시세 = 0x1000 + 'Q',
        주식우선호가 = 0x1000 + 'N',
        주식호가잔량 = 0x1000 + 'R',
        장시작시간 = 0x1000 + 'T'
    }
    public enum Operation
    {
        장시작전 = 0,
        장마감전_동시호가 = 2,
        장시작 = 3,
        장종료_예상지수종료 = 4,
        장마감 = 8,
        장종료_시간외종료 = 9,
        시간외_종가매매_시작 = 'a',
        시간외_종가매매_종료 = 'b',
        시간외_단일가_매매시작 = 'c',
        시간외_단일가_매매종료 = 'd',
        선옵_장마감전_동시호가_시작 = 's',
        선옵_장마감전_동시호가_종료 = 'e'
    }
    public enum Securities
    {
        Kiwoom = 'K'
    }
}