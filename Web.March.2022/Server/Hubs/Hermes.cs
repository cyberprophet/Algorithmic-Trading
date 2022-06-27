using Microsoft.AspNetCore.SignalR;

using Newtonsoft.Json;

using ShareInvest.Event;
using ShareInvest.Interface;

using System.Diagnostics;

namespace ShareInvest.Server.Hubs
{
    public class Hermes : Hub<IHubs>
    {
        public Hermes(IHubContext<Message, IHubs> message)
        {
            this.message = message;
            quotes = new Dictionary<string, string[]>();
            stocks = new Dictionary<string, string>();
            task = new Dictionary<string, int>();
            charts = new Dictionary<string, IEnumerable<Chart>>();
        }
        public void OnReceiveQueuedQuantity(string id, int count)
        {
            if (task.ContainsKey(id))
                task[id] = count;
        }
        public async Task SendChartAsync(string code, Chart[] chart)
        {
            charts[string.Concat(code, chart[0].Date)] = chart;
            await message.Clients.Group(code).OnReceiveStreamingChart(chart);
        }
        public async Task SendBundleAsync(string code, string[] args)
        {
            quotes[code] = args;
            await message.Clients.Group(code).OnReceiveBundleMessage(args);
        }
        public async Task SendAsync(string id, Method method, string json)
        {
            var name = string.Empty;

            switch (method)
            {
                case Method.OPW00004:

                    if (Condition.IsDebug is false)
                        await Clients.Caller.OnReceiveMethodMessage(method, id);

                    name = id;
                    break;

                case Method.opt10004
                when JsonConvert.DeserializeObject<Interface.OpenAPI.Opt10004>(json) is Interface.OpenAPI.Opt10004 opt10004:

                    if (opt10004.Value is not null &&
                        quotes.TryGetValue(opt10004.Value[0], out string[]? quote) &&
                        quote[0].CompareTo(id) >= 0)
                    {
                        await message.Clients.Group(opt10004.Value[0]).OnReceiveBundleMessage(quote);

                        return;
                    }
                    foreach (var kv in from o in task
                                       orderby o.Value ascending
                                       select o)
                    {
                        name = kv.Key;

                        break;
                    }
                    break;

                case Method.opt10081
                when JsonConvert.DeserializeObject<Interface.OpenAPI.Opt10081>(json) is Interface.OpenAPI.Opt10081 opt10081:

                    if (opt10081.Value is not null &&
                        charts.ContainsKey(string.Concat(opt10081.Value[0], opt10081.Value[1])))
                    {
                        foreach (var kv in from o in charts
                                           where o.Key[..6].Equals(opt10081.Value[0])
                                           orderby o.Key descending
                                           select o)
                            await message.Clients.Groups(kv.Key[..6]).OnReceiveStreamingChart(kv.Value);

                        return;
                    }
                    foreach (var kv in from o in task
                                       orderby o.Value ascending
                                       select o)
                    {
                        name = kv.Key;

                        break;
                    }
                    break;
            }
            if (string.IsNullOrEmpty(name) is false)
                await Clients.Group(name).OnReceiveMethodMessage(method, json);
        }
        public override async Task OnConnectedAsync()
        {
            if (Context.GetHttpContext()?.Request.Query is IQueryCollection collection)
                foreach (var kv in collection)
                    if (string.IsNullOrEmpty(kv.Value) is false)
                        switch (kv.Key)
                        {
                            case "key":
                                await Groups.AddToGroupAsync(Context.ConnectionId, kv.Value);
                                task[kv.Value] = int.MaxValue;
                                break;
                        }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.GetHttpContext()?.Request.Query is IQueryCollection collection)
                foreach (var kv in collection)
                    if (string.IsNullOrEmpty(kv.Value) is false)
                        switch (kv.Key)
                        {
                            case "key":
                                await Groups.RemoveFromGroupAsync(Context.ConnectionId, kv.Value);

                                if (task.Remove(kv.Value, out int count) && Condition.IsDebug)
                                    Debug.WriteLine(JsonConvert.SerializeObject(new
                                    {
                                        id = kv.Value,
                                        count
                                    },
                                    Formatting.Indented));
                                break;
                        }
            await base.OnDisconnectedAsync(exception);
        }
        internal async void OnReceiveMessage(object? sender, SecuritiesEventArgs e)
        {
            switch (e.Convey)
            {
                case Tuple<Method, string, string[]> r:

                    switch (r.Item1)
                    {
                        case Method.주식호가잔량 or Method.주식우선호가 when r.Item2.Length == 6:

                            if (r.Item3.Length > 3)
                                quotes[r.Item2] = r.Item3;

                            await message.Clients.Group(r.Item2).OnReceiveBundleMessage(r.Item3);
                            return;

                        case Method.주식체결 when r.Item2.Length == 6:

                            if (stocks.TryGetValue(r.Item2, out string? price) && r.Item3[1].Equals(price))
                                return;

                            await message.Clients.All.OnReceiveQuoteMessage(new Models.OpenAPI.Stock
                            {
                                Code = r.Item2,
                                Current = r.Item3[1],
                                StartingPrice = r.Item3[9],
                                HighPrice = r.Item3[10],
                                LowPrice = r.Item3[11],
                                CompareToPreviousSign = r.Item3[12],
                                CompareToPreviousDay = r.Item3[2],
                                Rate = r.Item3[3],
                                Volume = r.Item3[7],
                                TransactionAmount = r.Item3[8],
                                FasteningTime = r.Item3[0]
                            });
                            stocks[r.Item2] = r.Item3[1];
                            return;

                        case Method.장시작시간 when r.Item3[0].Length == 1 && Condition.GetOperation(r.Item3[0]) is Operation operation:

                            if (Operation.장마감.Equals(operation) || Operation.장시작.Equals(operation))
                                charts.Clear();

                            await Clients.All.OnReceiveOperationMessage(operation, r.Item3[1], r.Item3[^1]);
                            return;

                        case Method.주식시세 when r.Item2.Length == 6:
                            await message.Clients.All.OnReceiveQuoteMessage(new Models.OpenAPI.Stock
                            {
                                Code = r.Item2,
                                Current = r.Item3[0],
                                StartingPrice = r.Item3[7],
                                HighPrice = r.Item3[8],
                                LowPrice = r.Item3[9],
                                CompareToPreviousSign = r.Item3[10],
                                CompareToPreviousDay = r.Item3[1],
                                Rate = r.Item3[2],
                                Volume = r.Item3[5],
                                TransactionAmount = r.Item3[6]
                            });
                            return;
                    }
                    await message.Clients.All.OnReceiveMethodMessage(r.Item1, r.Item2);
                    break;
            }
        }
        readonly IHubContext<Message, IHubs> message;
        readonly Dictionary<string, string[]> quotes;
        readonly Dictionary<string, string> stocks;
        readonly Dictionary<string, int> task;
        readonly Dictionary<string, IEnumerable<Chart>> charts;
    }
}