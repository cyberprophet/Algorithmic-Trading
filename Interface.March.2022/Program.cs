using ShareInvest.Interface;

namespace ShareInvest
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new CoreAPI(new API(Condition.Url), new Icon[]
            {
                    Properties.Resources.server_download,
                    Properties.Resources.server_upload,
                    Properties.Resources.server_white,
                    Properties.Resources.server_black
            }));
            GC.Collect();
        }
    }
}