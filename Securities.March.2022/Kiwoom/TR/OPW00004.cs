using AxKHOpenAPILib;

using Newtonsoft.Json;

using ShareInvest.Event;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.Kiwoom
{
    class OPW00004 : Tr
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
            if (TR?.Value is not null)
            {
                if (TR?.Single is not null)
                {
                    var dic = new Dictionary<string, string>
                    {
                        { TR.Id[0], TR.Value[0] }
                    };
                    for (int i = 0; i < TR.Single.Length; i++)
                        dic[TR.Single[i]] = Ax is not null ? Ax.GetCommData(e.sTrCode, e.sRQName, 0, TR.Single[i]).Trim() : string.Empty;

                    if (dic.Count > 0)
                        Send?.Invoke(this, new SecuritiesEventArgs(Interface.Method.Account, JsonConvert.SerializeObject(dic)));
                }
                if (TR?.Multiple is not null)
                    for (int i = 0; i < Ax?.GetRepeatCnt(e.sTrCode, e.sRQName); i++)
                    {
                        var dic = new Dictionary<string, string>
                        {
                            { TR.Id[0], TR.Value[0] }
                        };
                        for (int j = 0; j < TR.Multiple.Length; j++)
                            dic[TR.Multiple[j]] = Ax.GetCommData(e.sTrCode, e.sRQName, i, TR.Multiple[j]).Trim();

                        if (dic.Count > 0)
                            Delay.GetInstance(0x259)
                                 .RequestTheMission(new Task(() => Send?.Invoke(this, new SecuritiesEventArgs(Interface.Method.Balance, JsonConvert.SerializeObject(dic)))));
                    }
                Delay.GetInstance(0x259)
                     .RequestTheMission(new Task(() => Send?.Invoke(this, new SecuritiesEventArgs(Interface.Method.Account, string.Empty))));
            }
            return int.TryParse(e.sPrevNext, out int next) is false || next == 0;
        }
    }
}