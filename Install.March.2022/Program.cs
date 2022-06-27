using Newtonsoft.Json;

using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Security.Principal;

namespace ShareInvest
{
    static class Program
    {
        static string GetPath(string? pin)
        {
            var physical = string.Empty;
            var chaos = pin?.Split('-');

            foreach (var net in NetworkInterface.GetAllNetworkInterfaces())
            {
                physical = net.GetPhysicalAddress().ToString();

                if (string.IsNullOrEmpty(physical) is false && physical.Length == 0xC)
                    break;
            }
            if (chaos is not null && chaos.Length == 5)
                return string.Concat(chaos[^1], physical[0..3], chaos[^2], physical[3..6], chaos[^3], physical[6..9], chaos[^4], physical[9..0xC], chaos[^5]) switch
                {
                    _ => string.Empty
                };
            return string.Empty;
        }
        [STAThread]
        static async Task Main()
        {
            Condition.SetDebug();

            if (IsAdministrator)
            {
                var icons = new[]
                {
                    Properties.Resources.recycling_recyclebin_empty,
                    Properties.Resources.recycle_recyclebin_full
                };
                ApplicationConfiguration.Initialize();
                Application.Run(new DirectoryInfo(@"C:\Server").Exists ? new Server(icons) : new Install(icons, Interface.Securities.Kiwoom));
                Process.Start("shutdown.exe", "-r");
                new Module(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true).AddStartupProgram("Algorithmic Trading", Application.ExecutablePath);
            }
            else if (Condition.IsDebug)
            {
                string path, arg, release, bin;
                const string at = @"C:\Algorithmic Trading";

                switch (MessageBox.Show("Please specify the upload path.", nameof(ShareInvest), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1))
                {
                    case DialogResult.Yes:
                        bin = @"bin\Release\net6.0\win-x64\publish";
                        release = "dotnet publish -c release -r win-x64 --sc";
                        path = @$"{GetPath(KeyDecoder.ProductKeyFromRegistry)}\Algorithmic.NET6\Web.March.2022\Server";
                        arg = "server";
                        break;

                    case DialogResult.Cancel:
                        bin = @"bin\Release\net6.0-windows8.0\win-x86\publish";
                        path = @$"{GetPath(KeyDecoder.ProductKeyFromRegistry)}\Algorithmic.NET6\Securities.March.2022";
                        release = "dotnet publish -c release -r win-x86 --no-self-contained";
                        arg = "x86";
                        break;

                    case DialogResult.No:
                        arg = "x64";
                        bin = @"bin\Release\net6.0-windows8.0\win-x64\publish";
                        path = @$"{GetPath(KeyDecoder.ProductKeyFromRegistry)}\Algorithmic.NET6\Interface.March.2022";
                        release = "dotnet publish -c release -r win-x64 --no-self-contained";
                        break;

                    case DialogResult.OK:
                        arg = "install";
                        path = @$"{GetPath(KeyDecoder.ProductKeyFromRegistry)}\Algorithmic.NET6\Install.March.2022";
                        bin = @"bin\Release\net6.0-windows8.0\win-x64\publish";
                        release = "dotnet publish -c release -r win-x64 --no-self-contained";
                        break;

                    default:
                        Debug.WriteLine("You passed an invalid command.");
                        return;
                }
                if (new Module(Path.Combine(at, arg, "publish.zip")) is Module module &&
                    string.IsNullOrEmpty(module.Path) is false)
                {
                    using (var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            Verb = "runas",
                            FileName = "cmd",
                            WorkingDirectory = path,
                            UseShellExecute = false,
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    })
                    {
                        process.OutputDataReceived += module.OnReceiveOutputData;

                        if (process.Start())
                        {
                            process.BeginOutputReadLine();
                            process.StandardInput.Write(release + Environment.NewLine);
                            process.StandardInput.Close();
                            await process.WaitForExitAsync();
                        }
                        process.OutputDataReceived -= module.OnReceiveOutputData;
                        System.IO.Compression.ZipFile.CreateFromDirectory(Path.Combine(process.StartInfo.WorkingDirectory, bin), module.Path);
                    }
                    if (await new Interface.API("https://coreapi.shareinvest.net").PostStructAsync(new Interface.File
                    {
                        Name = arg,
                        Data = await File.ReadAllBytesAsync(module.Path)
                    })
                        is Interface.File file)
                        Debug.WriteLine(JsonConvert.SerializeObject(file, Formatting.Indented));
                }
                return;
            }
            else
            {
                var wd = new DirectoryInfo(@"C:\Algorithmic Trading");

                if (wd.Exists is false)
                    wd.Create();

                if (wd.FullName.Equals(Application.StartupPath) is false)
                {
                    var files = Directory.GetFiles(Application.StartupPath, "*", SearchOption.TopDirectoryOnly);

                    if (files.Length > Directory.GetFiles(wd.FullName, "*", SearchOption.TopDirectoryOnly).Length)
                        foreach (var file in files)
                            File.Copy(new FileInfo(file).FullName, Path.Combine(wd.FullName, Path.GetFileName(file)), true);
                }
                if (new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        Verb = "runas",
                        UseShellExecute = true,
                        WorkingDirectory = wd.FullName,
                        FileName = Path.GetFileName(Application.ExecutablePath)
                    }
                }.Start())
                    GC.Collect();
            }
            Process.GetCurrentProcess().Kill();
        }
        static bool IsAdministrator => WindowsIdentity.GetCurrent() is WindowsIdentity identity && new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
    }
}