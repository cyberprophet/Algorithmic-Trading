using Microsoft.Win32;

using System.Diagnostics;

namespace ShareInvest
{
    partial class Install : Form
    {
        internal Install(Icon[] icons, Interface.Securities securities)
        {
            InitializeComponent();
            OpenAPI = securities;
            progress.Style = ProgressBarStyle.Continuous;
            progress.Maximum = 300;
            this.icons = icons;
            api = new Interface.API(Condition.Url);
            timer.Start();
        }
        async Task<bool> StartProgress()
        {
            var exist = IsSecuritiesAPI;

            if (exist is false)
                Process.Start(new ProcessStartInfo("https://www1.kiwoom.com/h/customer/download/VOpenApiInfoView?F0gIF=buCct2QJl59J9FMvd6vaYFmpCAVUNy9p9eVyMDek9sEdbuCi9sB5rhe6fh9XDF8e6xC0VIUkT2YYYxOutmCBAIjGDGCBr689bnCfrhe6f6KTNIKutUKlws85MnVFD6Q2DnVtTAK4AD8Awkcv6sepwn8If6e2fkjGtUgXNFCowm8QfDQkcveGNUcmfxc3woclcxVptYKcfFeFfkNgwUapNh86N4MQfADgDYmuAUModAe2NmezDncaDiMET4KgTDxaYvet6mEubnYQd40NbmmDMhmkrIV2Txev6xQ9wic7rnYTM68hKUeBV4KSDDEYwe0utI8TN2mXdIY2KhwuMFEyNIwaTIEir68EKICfws8vbAC8bmEvKG8tNFmXd402wF8JK4VytA80fnDXleUg6hYGK6pGTIYnYFZat400tnm6t403MAVvMsKnT6mvY407SAi5qyCB9sB5cFM8Nk8xlsMslFfQVo9otANulFVFloVsVA8xV2fGc6UQtAlGloUucFNXt69aNAYFN2UgN4msNsUiNkMFc69vc2luc6DkNkfQlk9kt6VFVsZQN4ZaVAxkt4UkVAc8tAUklo9vVomFcANiVFmsl6C5V4KscAfXt6MecsxQlkDuNoD5qyC8wu9J6vvPnni%253D&JRO2QHs6F=ZHVtbXlWYWwlM0Qw")
                {
                    UseShellExecute = exist is false
                });
            foreach (var path in new[] { x64, x86 })
            {
                var file = new FileInfo(path);
                var x = file.DirectoryName?.Split('\\')[^1];
                bool update = false;

                if (file.Exists)
                {
                    if (await api.GetContextAsync($"file?time={file.LastWriteTime.Ticks}&name={x}") is Interface.File info)
                        update = info.Ticks > file.LastWriteTime.Ticks;
                }
                else
                {
                    update = true;

                    if (file.Directory?.Exists is false)
                        file.Directory.Create();
                }
                if (update && await api.GetContextAsync($"file?name={x}") is Interface.File files)
                {
                    var directory = new DirectoryInfo($@"C:\Program Files{files.Name}\Algorithmic Trading");
                    await File.WriteAllBytesAsync(path, files.Data);

                    if (directory.Exists is false)
                        directory.Create();

                    System.IO.Compression.ZipFile.ExtractToDirectory(path, directory.FullName, true);
                }
            }
            if (exist is false)
                exist = IsSecuritiesAPI;

            return exist;
        }
        async void TimerTick(object sender, EventArgs e)
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                var seconds = DateTime.Now.Second;

                if (seconds % 60 == 1)
                {
                    if (Process.GetProcessesByName(nameof(Interface.API)).Length > 0)
                    {
                        if (Process.GetProcessesByName("Securities").Length < 1)
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
                    else
                        Dispose();
                }
                notifyIcon.Icon = icons[seconds % icons.Length];
            }
            else if (progress.Value < progress.Maximum && progress.Value++ == 0 && await StartProgress())
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

                using (var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = "API.exe",
                        WorkingDirectory = @"C:\Program Files\Algorithmic Trading",
                        Verb = "runas"
                    }
                })
                    if (process.Start())
                        notifyIcon.Text = process.ProcessName;

                ResumeLayout();
            }
        }));
        void JustBeforeFormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseReason.UserClosing.Equals(e.CloseReason))
                e.Cancel = DialogResult.Cancel.Equals(MessageBox.Show(warning, nameof(warning).ToUpper(), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2));
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
        Interface.Securities OpenAPI
        {
            get;
        }
        readonly Icon[] icons;
        readonly Interface.API api;
        const string warning = "It can be fatal to data when manually terminated.\n\nDo you really want to end it?";
        const string software = "Software";
        const string x86 = @"C:\Algorithmic Trading\x86\publish.zip";
        const string x64 = @"C:\Algorithmic Trading\x64\publish.zip";
    }
}