using Microsoft.Win32;

using System.Diagnostics;

namespace ShareInvest
{
    partial class Server : Form
    {
        internal Server(Icon[] icons)
        {
            this.icons = icons;
            InitializeComponent();
            OpenAPI = Interface.Securities.Kiwoom;
            progress.Style = ProgressBarStyle.Continuous;
            progress.Maximum = 300;
            timer.Start();
        }
        void TimerTick(object sender, EventArgs e)
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                var seconds = DateTime.Now.Second;

                if (seconds % 60 == 1)
                {
                    if (Process.GetProcessesByName("Server").Length < 1)
                    {
                        using (var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                UseShellExecute = true,
                                FileName = "Server.exe",
                                WorkingDirectory = @"C:\Server",
                                Verb = "runas"
                            }
                        })
                            if (process.Start())
                                notifyIcon.Text = process.ProcessName;
                    }
                    var processes = Process.GetProcessesByName("Securities");

                    if (processes.Length < 1)
                    {
                        using (var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                UseShellExecute = true,
                                FileName = "Securities.exe",
                                WorkingDirectory = @"C:\Program Files (x86)\Algorithmic Trading",
                                Verb = "runas"
                            }
                        })
                            if (process.Start())
                                notifyIcon.Text = process.ProcessName;
                    }
                    else if (processes.Length > 1)
                        foreach (var process in processes)
                            process.Kill();
                }
                notifyIcon.Icon = icons[seconds % icons.Length];
            }
            else if (progress.Value < progress.Maximum && progress.Value++ == 0 && IsUpdate)
                WindowState = FormWindowState.Minimized;
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
            }
        }));
        void JustBeforeFormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseReason.UserClosing.Equals(e.CloseReason))
                e.Cancel = DialogResult.Cancel.Equals(MessageBox.Show(warning, nameof(warning).ToUpper(), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2));
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
        Interface.Securities OpenAPI
        {
            get;
        }
        bool IsUpdate
        {
            get
            {
                var exist = IsSecuritiesAPI;

                foreach (var path in new[] { server, x86 })
                {
                    FileInfo file = new(path);
                    bool update;

                    if (file.Exists)
                    {
                        var exe = new FileInfo(server.Equals(path) ? @"C:\Server\Server.exe" : @"C:\Program Files (x86)\Algorithmic Trading\Securities.exe");

                        if (exe.Exists)
                            update = exe.LastWriteTime.Ticks < file.LastWriteTime.Ticks;

                        else
                            update = true;
                    }
                    else
                    {
                        update = true;

                        if (file.Directory?.Exists is false)
                            file.Directory.Create();
                    }
                    if (update)
                    {
                        DirectoryInfo directory = new(server.Equals(path) ? @"C:\Server" : @"C:\Program Files (x86)\Algorithmic Trading");

                        if (directory.Exists is false)
                            directory.Create();

                        System.IO.Compression.ZipFile.ExtractToDirectory(path, directory.FullName, true);
                    }
                }
                return exist;
            }
        }
        bool IsSecuritiesAPI
        {
            get
            {
                using (var securities = Registry.CurrentUser.OpenSubKey(software))
                    if (securities is not null)
                        using (var api = securities.OpenSubKey(OpenAPI.ToString().ToLower()))
                            if (api is not null)
                                foreach (string name in api.GetValueNames())
                                    if (nameof(OpenAPI).Equals(name))
                                        return true;

                using (var securities = Registry.LocalMachine.OpenSubKey(software))
                    if (securities is not null)
                        using (var api = securities.OpenSubKey(OpenAPI.ToString().ToLower()))
                            if (api is not null)
                                foreach (string name in api.GetValueNames())
                                    if (nameof(OpenAPI).Equals(name))
                                        return true;
                return false;
            }
        }
        const string server = @"C:\Server\wwwroot\update\server.zip";
        const string x86 = @"C:\Server\wwwroot\update\x86.zip";
        const string warning = "It can be fatal to data when manually terminated.\n\nDo you really want to end it?";
        const string software = "Software";
        readonly Icon[] icons;
    }
}