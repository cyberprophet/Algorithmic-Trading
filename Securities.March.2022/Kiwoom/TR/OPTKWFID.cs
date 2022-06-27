using AxKHOpenAPILib;

using Newtonsoft.Json;

using ShareInvest.Event;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.Kiwoom
{
    class OPTKWFID : Tr
    {
        public override event EventHandler<SecuritiesEventArgs>? Send;
        internal override TR? TR
        {
            get; set;
        }
        internal override AxKHOpenAPI? Ax
        {
            get; set;
        }
        internal override bool OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if (TR?.Multiple is not null)
                for (int i = 0; i < Ax?.GetRepeatCnt(e.sTrCode, e.sRQName); i++)
                {
                    var dic = new Dictionary<string, string>();

                    for (int j = 0; j < TR.Multiple.Length; j++)
                        dic[TR.Multiple[j]] = Ax.GetCommData(e.sTrCode, e.sRQName, i, TR.Multiple[j]).Trim();

                    var code = dic[TR.Multiple[0]];
                    dic[nameof(Models.OpenAPI.Stock.State)] = Ax.GetMasterStockState(code);
                    dic[TR.Multiple[0x24]] = Ax.KOA_Functions(cnt, code);
                    dic[nameof(Models.OpenAPI.Stock.ConstructionSupervision)] = Ax.KOA_Functions(info, code).Replace(';', '+');
                    dic[nameof(Models.OpenAPI.Stock.InvestmentCaution)] = Ax.KOA_Functions(warning, code);
                    dic[nameof(Models.OpenAPI.Stock.ListingDate)] = Ax.GetMasterListedStockDate(code);

                    if (dic.Count > 0)
                        Send?.Invoke(this, new SecuritiesEventArgs(Interface.Method.OPTKWFID, JsonConvert.SerializeObject(dic)));
                }
            Send?.Invoke(this, new SecuritiesEventArgs(Interface.Method.OPTKWFID, string.Empty));

            return int.TryParse(e.sPrevNext, out int next) is false || next == 0;
        }
        const string info = "GetMasterStockInfo";
        const string cnt = "GetMasterListedStockCntEx";
        const string warning = "IsOrderWarningStock";
    }
}