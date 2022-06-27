using AxKHOpenAPILib;

using ShareInvest.Event;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.Kiwoom
{
    class Opt10004 : Tr
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
            if (TR is not null && Ax is not null)
            {
                int i, j;
                string[] single = new string[TR.Single.Length], multi = new string[TR.Multiple.Length];

                if (TR.Single.Length > 0)
                    for (i = 0; i < TR.Single.Length; i++)
                        single[i] = Ax.GetCommData(e.sTrCode, e.sRQName, 0, TR.Single[i]).Trim();

                for (i = 0; i < Ax.GetRepeatCnt(e.sTrCode, e.sRQName); i++)
                    for (j = 0; j < TR.Multiple.Length; j++)
                        multi[j] = Ax.GetCommData(e.sTrCode, e.sRQName, i, TR.Multiple[j]).Trim();

                Send?.Invoke(this, new SecuritiesEventArgs(TR, single, multi));
            }
            return int.TryParse(e.sPrevNext, out int next) is false || next == 0;
        }
    }
}