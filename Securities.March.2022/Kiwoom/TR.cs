using AxKHOpenAPILib;

using ShareInvest.Event;

namespace ShareInvest.Kiwoom
{
    abstract class Tr : ICoreAPI<SecuritiesEventArgs>
    {
        public abstract event EventHandler<SecuritiesEventArgs> Send;
        internal abstract Interface.OpenAPI.TR? TR
        {
            get; set;
        }
        internal abstract AxKHOpenAPI? Ax
        {
            get; set;
        }
        internal abstract bool OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e);
    }
}