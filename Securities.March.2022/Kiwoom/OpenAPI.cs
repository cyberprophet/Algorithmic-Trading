using AxKHOpenAPILib;

using Newtonsoft.Json;

using ShareInvest.Event;

using System.IO.Pipes;
using System.Text;

namespace ShareInvest.Kiwoom
{
    partial class OpenAPI : UserControl, ICoreAPI<SecuritiesEventArgs>
    {
        public event EventHandler<SecuritiesEventArgs>? Send;
        internal new void Dispose() => Send?.Invoke(this, new SecuritiesEventArgs(Condition.IsDebug));
        internal bool IsAdministrator
        {
            get; private set;
        }
        internal int Count
        {
            get; private set;
        }
        internal int ConnectState => axAPI.GetConnectState();
        internal OpenAPI(string key, NamedPipeServerStream server)
        {
            this.key = key;
            this.server = server;
            InitializeComponent();
            tr = new HashSet<Tr>();
        }
        internal async Task<bool> CommConnect()
        {
            axAPI.OnReceiveChejanData += OnReceiveChejanData;
            axAPI.OnReceiveRealData += OnReceiveRealData;
            axAPI.OnReceiveTrData += OnReceiveTrData;
            axAPI.OnEventConnect += OnEventConnect;
            axAPI.OnReceiveMsg += OnReceiveMessage;
            axAPI.OnReceiveConditionVer += OnReceiveConditionVersion;
            axAPI.OnReceiveRealCondition += OnReceiveRealCondition;
            axAPI.OnReceiveTrCondition += OnReceiveTrCondition;
            await server.WaitForConnectionAsync();
            Pipe = new StreamWriter(server)
            {
                AutoFlush = true
            };
            return axAPI.CommConnect() == 0;
        }
        internal ICoreAPI<SecuritiesEventArgs>? CommRqData(string ctor, string name, string[] value)
        {
            try
            {
                var tr = this.tr.First(o => ctor.Equals(o.TR?.TrCode) && value.Equals(o.TR?.Value) && name.Equals(o.TR?.RQName));

                if (this.tr.Remove(tr))
                {
                    Pipe?.WriteLine(JsonConvert.SerializeObject(new Interface.Hermes
                    {
                        Parameter = value[0],
                        Method = Interface.Method.Message
                    }));
                    return tr;
                }
            }
            catch (Exception ex)
            {
                Send?.Invoke(this, new SecuritiesEventArgs(ex));
            }
            return null;
        }
        internal ICoreAPI<SecuritiesEventArgs> CommRqData(Interface.OpenAPI.TR tr)
        {
            ICoreAPI<SecuritiesEventArgs> ctor = tr switch
            {
                Interface.OpenAPI.Opt10004 => new Opt10004
                {
                    TR = tr,
                    Ax = axAPI
                },
                Interface.OpenAPI.Opt10081 => new Opt10081
                {
                    TR = tr,
                    Ax = axAPI
                },
                Interface.OpenAPI.OPW00004 => new OPW00004
                {
                    TR = tr,
                    Ax = axAPI
                },
                Interface.OpenAPI.OPTKWFID or _ => new OPTKWFID
                {
                    Ax = axAPI,
                    TR = tr
                }
            };
            if (ctor is Tr op && this.tr.Add(op))
            {
                if (tr is Interface.OpenAPI.OPTKWFID)
                {
                    var nCodeCount = tr.PrevNext;
                    tr.PrevNext = 0;

                    if (tr.Value is not null)
                    {
                        Delay.GetInstance(0x259).RequestTheMission(new Task(() => OnReceiveErrorMessage(axAPI.CommKwRqData(tr.Value[0], tr.PrevNext, nCodeCount, 0, tr.RQName, tr.ScreenNo), tr.RQName)));
                        Count++;
                    }
                }
                else
                {
                    Delay.GetInstance(0x259).RequestTheMission(new Task(() =>
                    {
                        for (int i = 0; i < tr.Id.Length; i++)
                            axAPI.SetInputValue(tr.Id[i], tr.Value?[i]);

                        OnReceiveErrorMessage(axAPI.CommRqData(tr.RQName, tr.TrCode, tr.PrevNext, tr.ScreenNo), tr.RQName);
                    }));
                    Count++;
                }
            }
            return ctor;
        }
        internal void CommRqData(int milliseconds, Interface.OpenAPI.TR tr)
        {
            Delay.GetInstance(milliseconds).RequestTheMission(new Task(() =>
            {
                for (int i = 0; i < tr.Id.Length; i++)
                    axAPI.SetInputValue(tr.Id[i], tr.Value?[i]);

                OnReceiveErrorMessage(axAPI.CommRqData(tr.RQName, tr.TrCode, tr.PrevNext, tr.ScreenNo), tr.RQName);
            }));
            Count++;
        }
        internal void WriteLine(string json) => Pipe?.WriteLine(json);
        void OnReceiveErrorMessage(int error, string? sRQName)
        {
            if (error < 0)
            {
                Pipe?.WriteLine(JsonConvert.SerializeObject(new Interface.Hermes
                {
                    Parameter = JsonConvert.SerializeObject(new Models.OpenAPI.Message
                    {
                        Title = Condition.Error[error],
                        Screen = Math.Abs(error).ToString("D4"),
                        Code = sRQName,
                        Key = key,
                        Company = (int)Interface.Securities.Kiwoom
                    }),
                    Method = Interface.Method.Error
                }));
                if (Array.Exists(new[] { -0x64, -0x6A }, o => o == error))
                    Send?.Invoke(this, new SecuritiesEventArgs(Condition.IsDebug));
            }
        }
        void OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            if (Enum.TryParse(e.sRealType, out Interface.Method method))
                Pipe?.WriteLine(JsonConvert.SerializeObject(new Interface.Hermes
                {
                    Parameter = JsonConvert.SerializeObject(new Interface.Real
                    {
                        Code = e.sRealKey,
                        Data = e.sRealData
                    }),
                    Method = method
                }));
        }
        void OnReceiveMessage(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e) => Send?.Invoke(this, new SecuritiesEventArgs(new Models.OpenAPI.Message
        {
            Title = e.sMsg[9..],
            Code = e.sRQName,
            Screen = e.sScrNo,
            Key = key,
            Company = (int)Interface.Securities.Kiwoom
        }));
        void OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {

        }
        void OnReceiveTrData(object sender, _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            try
            {
                if (tr.First(o => e.sTrCode.Equals(o.TR?.TrCode) && e.sRQName.Equals(o.TR.RQName)).OnReceiveTrData(e))
                    Send?.Invoke(this, new SecuritiesEventArgs(e.sTrCode, e.sRQName, e.sScrNo));
            }
            catch (Exception ex)
            {
                Send?.Invoke(this, new SecuritiesEventArgs(ex));
            }
        }
        void OnReceiveConditionVersion(object sender, _DKHOpenAPIEvents_OnReceiveConditionVerEvent e)
        {

        }
        void OnReceiveTrCondition(object sender, _DKHOpenAPIEvents_OnReceiveTrConditionEvent e)
        {

        }
        void OnReceiveRealCondition(object sender, _DKHOpenAPIEvents_OnReceiveRealConditionEvent e)
        {

        }
        void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e) => Invoke(new Action(async () =>
        {
            if (e.nErrCode == 0)
            {
                if (await Condition.API.GetContextAsync($"securities?key={key}&company={(int)Interface.Securities.Kiwoom}") is Interface.Admin admin)
                {
                    if (admin.IsAdministrator)
                        Pipe?.WriteLine(JsonConvert.SerializeObject(new Interface.Hermes
                        {
                            Method = Interface.Method.Message,
                            Parameter = JsonConvert.SerializeObject(admin)
                        }));
                    IsAdministrator = admin.IsAdministrator;
                }
                Delay.GetInstance(0x259).Run();
                var server = axAPI.GetLoginInfo(nameof(Interface.LoginInfo.GetServerGubun));
                Send?.Invoke(this, new SecuritiesEventArgs(new Interface.LoginInfo
                {
                    Key = key,
                    ACCOUNT_CNT = int.TryParse(axAPI.GetLoginInfo(nameof(Interface.LoginInfo.ACCOUNT_CNT)), out int cnt) ? cnt : 0,
                    ACCLIST = axAPI.GetLoginInfo(nameof(Interface.LoginInfo.ACCLIST)).Split(';'),
                    USER_ID = axAPI.GetLoginInfo(nameof(Interface.LoginInfo.USER_ID)),
                    USER_NAME = axAPI.GetLoginInfo(nameof(Interface.LoginInfo.USER_NAME)),
                    GetServerGubun = string.IsNullOrEmpty(server) || int.TryParse(server, out int mock) && mock is not 1
                }));
                if (IsAdministrator || Condition.IsDebug)
                    Pipe?.WriteLine(JsonConvert.SerializeObject(new Interface.Hermes
                    {
                        Method = Interface.Method.Company
                    }));
                var list = new List<string>(axAPI.GetCodeListByMarket("0").Split(';').OrderBy(o => Guid.NewGuid()));
                list.AddRange(axAPI.GetCodeListByMarket("10").Split(';').OrderBy(o => Guid.NewGuid()));
                GetInformationOfCode(list.OrderBy(o => Guid.NewGuid()));
            }
            else
                OnReceiveErrorMessage(e.nErrCode, sender.GetType().Name);
        }));
        void GetInformationOfCode(IEnumerable<string> market)
        {
            int index = 0;
            var sb = new StringBuilder(0x100);
            var stack = new Stack<StringBuilder>(0x10);

            foreach (var code in market)
                if (string.IsNullOrEmpty(code) is false)
                {
                    if (index++ % 0x63 == 0x62)
                    {
                        stack.Push(sb.Append(code));
                        sb = new StringBuilder();
                    }
                    sb.Append(code).Append(';');
                }
            stack.Push(sb.Remove(sb.Length - 1, 1));

            while (stack.TryPop(out StringBuilder? pop))
                if (pop is not null && pop.Length > 5)
                    Send?.Invoke(this, new SecuritiesEventArgs(pop));
        }
        StreamWriter? Pipe
        {
            get; set;
        }
        readonly string key;
        readonly NamedPipeServerStream server;
        readonly HashSet<Tr> tr;
    }
}