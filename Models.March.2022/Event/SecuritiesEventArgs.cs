using System.Text;

namespace ShareInvest.Event
{
    public class SecuritiesEventArgs : EventArgs
    {
        public object Convey
        {
            get; private set;
        }
        public SecuritiesEventArgs(Dictionary<string, string> pairs) => Convey = pairs;
        public SecuritiesEventArgs(Exception exception) => Convey = exception;
        public SecuritiesEventArgs(StringBuilder sb) => Convey = sb;
        public SecuritiesEventArgs(Interface.OpenAPI.TR tr) => Convey = tr;
        public SecuritiesEventArgs(Interface.OpenAPI.TR tr, string[] single, string[] multi) => Convey = new Tuple<Interface.OpenAPI.TR, string[], string[]>(tr, single, multi);
        public SecuritiesEventArgs(Interface.OpenAPI.TR tr, int next, Queue<Interface.Chart> stack)
        {
            tr.PrevNext = next;
            Convey = new Tuple<Interface.OpenAPI.TR, Queue<Interface.Chart>>(tr, stack);
        }
        public SecuritiesEventArgs(Models.OpenAPI.Account account) => Convey = account;
        public SecuritiesEventArgs(Models.OpenAPI.Balance balance) => Convey = balance;
        public SecuritiesEventArgs(Models.OpenAPI.Message message) => Convey = message;
        public SecuritiesEventArgs(Models.OpenAPI.Stock stock) => Convey = stock;
        public SecuritiesEventArgs(Models.Securities securities) => Convey = securities;
        public SecuritiesEventArgs(Interface.LoginInfo info) => Convey = info;
        public SecuritiesEventArgs(Interface.Method method, string data) => Convey = new Tuple<Interface.Method, string>(method, data);
        public SecuritiesEventArgs(Interface.Method method, Interface.Real real) => Convey = new Tuple<Interface.Method, string, string[]>(method, real.Code, real.Data.Split('\t'));
        public SecuritiesEventArgs(string sTrCode, string sRQName, string sScrNo) => Convey = new Tuple<string, string, string>(sTrCode, sRQName, sScrNo);
        public SecuritiesEventArgs(string message) => Convey = message;
        public SecuritiesEventArgs(bool debug) => Convey = debug;
    }
}