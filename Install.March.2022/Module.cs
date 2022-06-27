using Microsoft.Win32;

using System.Diagnostics;

namespace ShareInvest
{
    class Module
    {
        internal Module(string path)
        {
            var file = new FileInfo(path);
            reg = string.Empty;
            Path = path;

            if (file.Exists)
                file.Delete();

            else if (file.Directory?.Exists is false)
                file.Directory.Create();
        }
        internal Module(string reg, bool writable)
        {
            this.writable = writable;
            this.reg = reg;
        }
        internal string? Path
        {
            get;
        }
        internal void OnReceiveOutputData(object sender, DataReceivedEventArgs e)
        {
            if (Condition.IsDebug)
                Debug.WriteLine(e.Data);
        }
        internal void AddStartupProgram(string program, string path)
        {
            using (var regKey = Registry.CurrentUser.OpenSubKey(reg, writable))
                if (regKey is not null)
                    try
                    {
                        if (regKey.GetValue(program) is null)
                            regKey.SetValue(program, path);

                        regKey.Close();
                    }
                    catch (Exception ex)
                    {
                        if (Condition.IsDebug)
                            Debug.WriteLine(ex.Message);
                    }
            GC.Collect();
        }
        readonly string reg;
        readonly bool writable;
    }
}