using System.Diagnostics;
using System.Net.NetworkInformation;

namespace ShareInvest
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            if (KeyDecoder.ProductKeyFromRegistry is string key)
            {
                var physical = string.Empty;
                var chaos = key.Split('-');

                foreach (var net in NetworkInterface.GetAllNetworkInterfaces())
                {
                    physical = net.GetPhysicalAddress().ToString();

                    if (string.IsNullOrEmpty(physical) is false && physical.Length == 0xC)
                        break;
                }
                Condition.SetDebug();
                ApplicationConfiguration.Initialize();
                Application.Run(new Securities(new[]
                {
                    Properties.Resources.bird_idle,
                    Properties.Resources.bird_awake,
                    Properties.Resources.bird_alert,
                    Properties.Resources.bird_sleep,
                    Properties.Resources.bird_invisible
                },
                string.Concat(chaos[^1], physical[0..3], chaos[^2], physical[3..6], chaos[^3], physical[6..9], chaos[^4], physical[9..0xC], chaos[^5])));
            }
            GC.Collect();
            Process.GetCurrentProcess().Kill();
        }
    }
}