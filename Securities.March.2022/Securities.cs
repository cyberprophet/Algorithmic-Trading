using Microsoft.AspNetCore.SignalR.Client;

using Newtonsoft.Json;

using ShareInvest.Event;
using ShareInvest.Kiwoom;

using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace ShareInvest
{
    partial class Securities : Form
    {
        internal Securities(Icon[] icons, string security)
        {
            stream = new PipeStream(nameof(Interface.Hermes));
            this.security = security;
            this.icons = icons;
            InitializeComponent();

            if (stream.Server is not null)
            {
                api = new OpenAPI(security, stream.Server);
                timer.Start();
            }
        }
        void PassingFromPipeToComponent(object? sender, SecuritiesEventArgs e) => Invoke(new Action(async () =>
        {
            string? ctor = null, name = null;
            string[]? value = null;

            switch (e.Convey)
            {
                case Tuple<Interface.Method, string> method:

                    switch (method.Item1)
                    {
                        case Interface.Method.OPTKWFID:

                            if (JsonConvert.DeserializeObject<Models.OpenAPI.Stock>(method.Item2) is Models.OpenAPI.Stock stock &&
                                await Condition.API.PostContextAsync(stock) is Interface.Initialization)
                            {
                                if (this.api is OpenAPI api && api.IsAdministrator)
                                    api.WriteLine(JsonConvert.SerializeObject(new Interface.Hermes
                                    {
                                        Parameter = method.Item2,
                                        Method = Interface.Method.OPTKWFID
                                    }));
                                return;
                            }
                            if (string.IsNullOrEmpty(method.Item2) &&
                                sender is OPTKWFID opt &&
                                opt.TR is Interface.OpenAPI.OPTKWFID kw)
                            {
                                name = kw.RQName;
                                value = kw.Value;

                                break;
                            }
                            return;

                        case Interface.Method.Account:

                            if (JsonConvert.DeserializeObject<Models.OpenAPI.Account>(method.Item2) is Models.OpenAPI.Account account)
                            {
                                account.Key = Info.Key;
                                account.Company = (int)Interface.Securities.Kiwoom;

                                if (await Condition.API.PostContextAsync(account) is Interface.Initialization)
                                    return;
                            }
                            if (string.IsNullOrEmpty(method.Item2) && sender is OPW00004 opw && opw.TR is Interface.OpenAPI.OPW00004 w0004)
                            {
                                name = w0004.RQName;
                                value = w0004.Value;

                                break;
                            }
                            return;

                        case Interface.Method.Balance when JsonConvert.DeserializeObject<Models.OpenAPI.Balance>(method.Item2) is Models.OpenAPI.Balance balance:
                            balance.Key = Info.Key;
                            balance.Company = (int)Interface.Securities.Kiwoom;

                            if (await Condition.API.PostContextAsync(balance) is Interface.Initialization)
                                notifyIcon.Text = balance.Name;

                            return;
                    }
                    ctor = sender?.GetType().Name;
                    break;

                case Tuple<Interface.OpenAPI.TR, Queue<Interface.Chart>> tc:

                    if (Hermes is not null)
                        await Hermes.SendAsync("SendChartAsync", tc.Item1.Value?[0], tc.Item2);

                    if (tc.Item1.PrevNext > 0)
                    {
                        (api as OpenAPI)?.CommRqData(0x259, tc.Item1);

                        return;
                    }
                    ctor = tc.Item1.TrCode;
                    name = tc.Item1.RQName;
                    value = tc.Item1.Value;
                    break;

                case Tuple<Interface.OpenAPI.TR, string[], string[]> tr when Hermes is not null:

                    switch (tr.Item1)
                    {
                        case Interface.OpenAPI.Opt10004:
                            await Hermes.SendAsync("SendBundleAsync", tr.Item1.Value?[0], tr.Item3);
                            break;
                    }
                    ctor = tr.Item1.TrCode;
                    name = tr.Item1.RQName;
                    value = tr.Item1.Value;
                    break;

                case StringBuilder sb when sender is OpenAPI api && sb.ToString() is string codes:
                    api.CommRqData(new Interface.OpenAPI.OPTKWFID
                    {
                        Value = new[] { codes },
                        PrevNext = codes.Split(';').Length
                    })
                    .Send += PassingFromPipeToComponent;
                    return;

                case Models.OpenAPI.Message message when await Condition.API.PostContextAsync(message) is Models.OpenAPI.Message model:

                    switch (model.Screen)
                    {
                        case "0106" or "0100":
                            Dispose(api as Control);
                            break;
                    }
                    var param = $"{new DateTime(model.Lookup):G}\n[{model.Code}] {model.Title}({model.Screen})";
                    notifyIcon.Text = param.Length < 0x40 ? param : $"[{model.Code}] {model.Title}({model.Screen})";
                    return;

                case Interface.LoginInfo info when await Condition.API.PostContextAsync(new Models.Securities
                {
                    Id = Security.Crypto.Encrypt(Encoding.UTF8.GetBytes(info.Key), info.USER_ID),
                    Company = (int)Interface.Securities.Kiwoom,
                    Name = info.USER_NAME,
                    Key = info.Key,
                    Count = info.ACCOUNT_CNT
                })
                    is Interface.Initialization:

                    foreach (var account in info.ACCLIST)
                        if (account.Length == 0xA && this.api is OpenAPI api)
                            switch (account[^2..].CompareTo("31"))
                            {
                                case < 0:
                                    try
                                    {
                                        api.CommRqData(new Interface.OpenAPI.OPW00004
                                        {
                                            PrevNext = 0,
                                            Value = new[] { account, string.Empty, "0", "00" }
                                        })
                                        .Send += PassingFromPipeToComponent;
                                    }
                                    catch (Exception ex)
                                    {
                                        notifyIcon.Text = ex.Message.Length < 0x40 ? ex.Message : ex.Message[..0x3F];
                                    }
                                    break;

                                case 0:

                                    break;
                            }
                    Info = info;
                    return;

                case Tuple<string, string, string> args:
                    notifyIcon.Text = string.Concat('[', args.Item3, ']', ' ', args.Item1, '-', args.Item2);
                    return;

                case Exception ex:
                    notifyIcon.Text = ex.Message.Length < 0x40 ? ex.Message : ex.Message[..0x3F];
                    return;

                case string message:
                    notifyIcon.Text = message.Length < 0x40 ? message : message[..0x3F];
                    return;

                case bool:
                    await Task.Delay(0x100 * 0xA);
                    Dispose(api as Control);
                    return;
            }
            if (value is not null &&
                string.IsNullOrEmpty(ctor) is false &&
                string.IsNullOrEmpty(name) is false &&
                (api as OpenAPI)?.CommRqData(ctor, name, value) is ICoreAPI<SecuritiesEventArgs> connect)
            {
                connect.Send -= PassingFromPipeToComponent;
                notifyIcon.Text = name;
            }
        }));
        void TimerTick(object sender, EventArgs e)
        {
            if (api is null)
            {
                timer.Stop();
                strip.ItemClicked -= StripItemClicked;
                Dispose();
            }
            else if (FormBorderStyle.Equals(FormBorderStyle.Sizable) && WindowState.Equals(FormWindowState.Minimized) is false)
            {
                Task.Run(async () =>
                {
                    Hermes = new HubConnectionBuilder().WithUrl(string.Concat(Condition.Url, $"/hubs/hermes?key={security}")).WithAutomaticReconnect().Build();
                    OnHermes = Hermes.On<Interface.Method, string>(nameof(Interface.IHubs.OnReceiveMethodMessage), (method, json) =>
                    {
                        if (this.api is OpenAPI api)
                        {
                            Interface.OpenAPI.TR tr;

                            switch (method)
                            {
                                case Interface.Method.OPW00004
                                when JsonConvert.DeserializeObject<Interface.OpenAPI.OPW00004>(json) is Interface.OpenAPI.OPW00004 opw00004:
                                    tr = opw00004;
                                    break;

                                case Interface.Method.opt10081
                                when JsonConvert.DeserializeObject<Interface.OpenAPI.Opt10081>(json) is Interface.OpenAPI.Opt10081 opt10081:
                                    tr = opt10081;
                                    break;

                                case Interface.Method.opt10004
                                when JsonConvert.DeserializeObject<Interface.OpenAPI.Opt10004>(json) is Interface.OpenAPI.Opt10004 opt10004:
                                    tr = opt10004;
                                    break;

                                default:

                                    return;
                            }
                            api.CommRqData(tr).Send += PassingFromPipeToComponent;
                        }
                    });
                    OnOperation = Hermes.On<Interface.Operation, string, string>(nameof(Interface.IHubs.OnReceiveOperationMessage), (operation, ft, rt) =>
                    {
                        switch (operation)
                        {
                            case Interface.Operation.장시작 or Interface.Operation.장마감 when this.api is OpenAPI api:

                                foreach (var acc in Array.FindAll(Info.ACCLIST, str => str.Length == 0xA && str[^2..].CompareTo("31") < 0))
                                    api.CommRqData(new Interface.OpenAPI.OPW00004
                                    {
                                        Value = new[] { acc, string.Empty, "0", "00" },
                                        PrevNext = 0
                                    })
                                    .Send += PassingFromPipeToComponent;

                                if (api.IsAdministrator && Interface.Operation.장마감.Equals(operation))
                                    api.Dispose();

                                break;

                            case Interface.Operation.장시작전:

                                break;
                        }
                    });
                    Hermes.Closed += async error =>
                    {
                        if (HubConnectionState.Disconnected.Equals(Hermes.State))
                        {
                            await Task.Delay(new Random().Next(5, 10) * 0x400);
                            await Hermes.StartAsync();
                        }
                        if (error is not null && Condition.IsDebug)
                            Debug.WriteLine(error.Message);
                    };
                    Hermes.Reconnecting += async error =>
                    {
                        if (Condition.IsDebug && error is not null)
                        {
                            await Task.Delay(0x100);
                            Debug.WriteLine(error.Message);
                        }
                    };
                    if (HubConnectionState.Disconnected.Equals(Hermes.State))
                    {
                        await Hermes.StartAsync();

                        while (HubConnectionState.Connected.Equals(Hermes.State))
                            try
                            {
                                if (this.api is OpenAPI api)
                                {
                                    if (IsConnected && api.ConnectState == 1)
                                        await Hermes.SendAsync("OnReceiveQueuedQuantity", security, api.Count);

                                    else
                                        await Task.Delay(2 * 0x200 * 2 * 0x20);
                                }
                            }
                            catch (Exception ex)
                            {
                                if (Condition.IsDebug)
                                    Debug.WriteLine(ex.Message);
                            }
                            finally
                            {
                                await Task.Delay(0x200 * 3);
                            }
                    }
                });
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                var now = DateTime.Now;

                if (IsConnected)
                {
                    notifyIcon.Icon = icons[DateTime.Now.Second % 4];

                    if (now.Hour == 8 && now.Minute == 1 && now.Second % 9 == 0 &&
                        (int)now.DayOfWeek > 0 && (int)now.DayOfWeek < 6 &&
                        this.api is OpenAPI api && api.IsAdministrator)
                        api.Dispose();
                }
                else
                    notifyIcon.Icon = icons[^1];

                if (now.Second == 0x3A && now.Minute % 2 == 0)
                {
                    switch (now.DayOfWeek)
                    {
                        case DayOfWeek.Sunday:

                            break;

                        case DayOfWeek.Saturday:

                            break;

                        default:

                            break;
                    }
                    if (now.Hour == 5 && now.Minute < 9)
                    {

                    }
                    else if (api is OpenAPI kiwoom && kiwoom.ConnectState == 0)
                        BeginInvoke(new Action(async () =>
                        {
                            try
                            {
                                IsConnected = await kiwoom.CommConnect();

                                if (IsConnected)
                                    api.Send += PassingFromPipeToComponent;
                            }
                            catch (Exception)
                            {
                                Dispose(api as OpenAPI);
                            }
                        }));
                }
            }
        }
        void StripItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name.Equals(reference.Name))
                BeginInvoke(new Action(async () =>
                {
                    if (IsConnected)
                        Process.Start(new ProcessStartInfo(@"http://shareinvest.net")
                        {
                            UseShellExecute = true
                        });
                    else
                    {
                        switch (api)
                        {
                            case OpenAPI ko:
                                IsConnected = await ko.CommConnect();
                                break;
                        }
                        if (IsConnected && api is not null)
                            api.Send += PassingFromPipeToComponent;
                    }
                }));
            else
                Close();
        }
        void SecuritiesResize(object sender, EventArgs e) => BeginInvoke(new Action(() =>
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                SuspendLayout();
                Visible = false;
                ShowIcon = false;
                notifyIcon.Visible = true;
                ResumeLayout();
                StartProgress(api as Control);
            }
        }));
        void JustBeforeFormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseReason.UserClosing.Equals(e.CloseReason) &&
                DialogResult.Cancel.Equals(MessageBox.Show(warning, Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)))
            {
                e.Cancel = true;

                return;
            }
            Dispose(api as OpenAPI);
        }
        void StartProgress(IComponent? component)
        {
            if (component is Control control)
            {
                Controls.Add(control);
                control.Dock = DockStyle.Fill;
                control.Show();
                FormBorderStyle = FormBorderStyle.None;
                CenterToScreen();
            }
            else
                Close();
        }
        void Dispose(IComponent? component)
        {
            if (component is Control control)
                control.Dispose();

            Dispose();
        }
        bool IsConnected
        {
            get; set;
        }
        Interface.LoginInfo Info
        {
            get; set;
        }
        IDisposable? OnHermes
        {
            get; set;
        }
        IDisposable? OnOperation
        {
            get; set;
        }
        HubConnection? Hermes
        {
            get; set;
        }
        readonly Icon[] icons;
        readonly PipeStream stream;
        readonly ICoreAPI<SecuritiesEventArgs>? api;
        readonly string security;
        const string warning = "It can be fatal to data when manually terminated.\n\nDo you really want to end it?";
    }
}