using Newtonsoft.Json;

using ShareInvest.Event;

using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace ShareInvest
{
    public class PipeStream : IDisposable
    {
        public event EventHandler<SecuritiesEventArgs>? Send;
        public void Dispose()
        {
            if (Server is not null)
                Server.Dispose();

            if (Client is not null)
                Client.Dispose();

            GC.SuppressFinalize(this);
        }
        public void StartServerProgress()
        {
            Task.Run(async () =>
            {
                while (Client is not null)
                {
                    await Client.ConnectAsync();

                    using (var sr = new StreamReader(Client))
                        while (Client.IsConnected)
                            try
                            {
                                var str = sr.ReadLine();

                                if (string.IsNullOrEmpty(str))
                                    continue;

                                SecuritiesEventArgs? args = null;
                                var hermes = JsonConvert.DeserializeObject<Interface.Hermes>(str);

                                switch (hermes.Method)
                                {
                                    case Interface.Method.주식시세 or Interface.Method.주식체결 or Interface.Method.주식우선호가 or Interface.Method.주식호가잔량 or
                                        Interface.Method.장시작시간:
                                        args = new SecuritiesEventArgs(hermes.Method, JsonConvert.DeserializeObject<Interface.Real>(hermes.Parameter));
                                        break;

                                    case Interface.Method.Company when await Condition.Dart.GetContextAsync(Models.Dart.corpCode_xml, string.Empty) is Stack<Models.CompanyOverview> co:
                                        Condition.CompanyOverview = co;
                                        continue;

                                    case Interface.Method.OPTKWFID when JsonConvert.DeserializeObject<Models.OpenAPI.Stock>(hermes.Parameter) is Models.OpenAPI.Stock stock:
                                        Delay.GetInstance(0x3C).RequestTheMission(new Task(async () =>
                                        {
                                            if (string.IsNullOrEmpty(stock.Code) is false &&
                                                Condition.CompanyOverview is not null &&
                                                Condition.CompanyOverview.Any(o => stock.Code.Equals(o.Code)) &&
                                                Condition.CompanyOverview.Single(o => stock.Code.Equals(o.Code)) is Models.CompanyOverview co &&
                                                await Condition.Dart.GetContextAsync(Models.Dart.company_json, co.CorpCode) is Models.CompanyOverview company)
                                            {
                                                company.Date = DateTime.Now;
                                                company.ModifyDate = co.ModifyDate;
                                                await Condition.API.PutContextAsync(company);
                                            }
                                        }));
                                        continue;

                                    case Interface.Method.Message:
                                        Console.WriteLine(hermes.Parameter);
                                        continue;

                                    case Interface.Method.Error when JsonConvert.DeserializeObject<Models.OpenAPI.Message>(hermes.Parameter) is Models.OpenAPI.Message message:
                                        Console.WriteLine(JsonConvert.SerializeObject(await Condition.API.PostContextAsync(message), Formatting.Indented));
                                        continue;
                                }
                                if (args is not null)
                                    Send?.Invoke(this, args);
                            }
                            catch (Exception ex)
                            {
                                if (Condition.IsDebug)
                                {
                                    Debug.WriteLine(ex.Message);

                                    continue;
                                }
                                Console.WriteLine(ex.Message);
                            }
                    Client = new NamedPipeClientStream(serverName, pipeName, PipeDirection.In, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation);
                }
            });
            Delay.GetInstance(0x259).Run();
        }
        public bool IsConnected => Client is not null && Client.IsConnected;
        public PipeStream(string serverName, string pipeName)
        {
            Client = new NamedPipeClientStream(serverName, pipeName, PipeDirection.In, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation);
            this.serverName = serverName;
            this.pipeName = pipeName;
        }
        [SupportedOSPlatform("windows8.0")]
        public PipeStream(string pipeName)
        {
            Server = new NamedPipeServerStream(pipeName, PipeDirection.Out, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            this.pipeName = pipeName;
            serverName = ".";
        }
        public NamedPipeServerStream? Server
        {
            get;
        }
        NamedPipeClientStream? Client
        {
            get; set;
        }
        readonly string serverName;
        readonly string pipeName;
    }
}