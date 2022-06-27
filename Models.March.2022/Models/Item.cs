namespace ShareInvest.Models
{
    public enum Dart
    {
        corpCode_xml = 'C',
        company_json = 'c'
    }
    public enum Sign
    {
        arrow_thick_top = 1,
        caret_top = 2,
        _ = 3,
        arrow_thick_bottom = 4,
        caret_bottom = 5
    }
    public enum InvestCaution
    {
        해당없음 = 0,
        정리매매 = 2,
        단기과열 = 3,
        투자위험 = 4,
        투자경고 = 5
    }
    public enum Market
    {
        장내 = 0,
        코스닥 = 10,
        ELW = 3,
        ETF = 8,
        KONEX = 50,
        뮤추얼펀드 = 4,
        신주인수권 = 5,
        리츠 = 6,
        하이얼펀드 = 9,
        K_OTC = 30
    }
}