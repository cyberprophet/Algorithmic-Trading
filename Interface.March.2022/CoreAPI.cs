using Newtonsoft.Json;

using ShareInvest.Event;

using System.Diagnostics;

namespace ShareInvest
{
    partial class CoreAPI : Form
    {
        internal CoreAPI(Interface.API api, Icon[] icons)
        {
            this.api = api;
            this.icons = icons;
            InitializeComponent();
            timer.Start();
        }
        void TimerTick(object sender, EventArgs e)
        {
            if (FormBorderStyle.Equals(FormBorderStyle.Sizable) && WindowState.Equals(FormWindowState.Minimized) is false)
            {
                if (Client is null)
                {
                    Client = new PipeStream(".", nameof(Interface.Hermes));
                    Client.Send += PassingFromPipeToComponent;
                    Client.StartServerProgress();
                }
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                if (Client is not null && Client.IsConnected)
                    notifyIcon.Icon = icons[DateTime.Now.Second % 2];

                else
                    notifyIcon.Icon = icons[^1];
            }
        }
        void PassingFromPipeToComponent(object? sender, SecuritiesEventArgs e) => Invoke(new Action(async () =>
        {
            switch (e.Convey)
            {
                case Tuple<Interface.Method, string, string[]> r:

                    switch (r.Item1)
                    {
                        case Interface.Method.����۽ð� when r.Item3[0].Length == 1 && Condition.GetOperation(r.Item3[0]) is Interface.Operation operation:

                            switch (operation)
                            {
                                case Interface.Operation.�������:

                                    break;

                                case Interface.Operation.�����:

                                    break;

                                case Interface.Operation.�帶����_����ȣ��:

                                    break;

                                case Interface.Operation.�帶��:

                                    break;

                                case Interface.Operation.������_�ð�������:

                                    break;

                                case Interface.Operation.������_������������:

                                    break;

                                case Interface.Operation.����_�帶����_����ȣ��_����:

                                    break;

                                case Interface.Operation.����_�帶����_����ȣ��_����:

                                    break;

                                case Interface.Operation.�ð���_���ϰ�_�ŸŽ���:

                                    break;

                                case Interface.Operation.�ð���_���ϰ�_�Ÿ�����:

                                    break;

                                case Interface.Operation.�ð���_�����Ÿ�_����:

                                    break;

                                case Interface.Operation.�ð���_�����Ÿ�_����:

                                    break;
                            }
                            notifyIcon.Text = Enum.GetName(operation);
                            break;
                    }
                    return;

                case Models.OpenAPI.Balance balance when await api.PostContextAsync(balance) is Interface.Initialization initialization:

                    if (initialization.Changes > 0)
                        notifyIcon.Text = initialization.Id;

                    return;

                case Models.OpenAPI.Account account when await api.PostContextAsync(account) is Interface.Initialization initialization:

                    if (initialization.Changes > 0)
                        notifyIcon.Text = initialization.Id;

                    return;

                case Models.Securities init when await api.PostContextAsync(init) is Interface.Initialization initialization:

                    if (initialization.Changes > 0)
                        notifyIcon.Text = initialization.Id;

                    return;

                case bool:

                    while (Process.GetProcessesByName("Securities").Length > 0)
                        await Task.Delay(0x100);

                    break;

                default:

                    if (Condition.IsDebug && sender is not null)
                        Debug.WriteLine(JsonConvert.SerializeObject(new
                        {
                            sender = sender.GetType(),
                            convey = e.Convey.GetType().Name
                        },
                        Formatting.Indented));
                    return;
            }
            SuspendLayout();
            Visible = true;
            ShowIcon = true;
            notifyIcon.Visible = false;
            ResumeLayout();
            WindowState = FormWindowState.Normal;
            FormBorderStyle = FormBorderStyle.Sizable;
        }));
        void SecuritiesResize(object sender, EventArgs e) => BeginInvoke(new Action(() =>
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                SuspendLayout();
                Visible = false;
                ShowIcon = false;
                notifyIcon.Visible = true;
                ResumeLayout();
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
            Dispose();
        }
        void StripItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name.Equals(reference.Name))
                BeginInvoke(new Action(() => Process.Start(new ProcessStartInfo(@"http://shareinvest.net")
                {
                    UseShellExecute = true
                })));
            else
                Close();
        }
        PipeStream? Client
        {
            get; set;
        }
        readonly Interface.API api;
        readonly Icon[] icons;
        const string warning = "It can be fatal to data when manually terminated.\n\nDo you really want to end it?";
    }
}