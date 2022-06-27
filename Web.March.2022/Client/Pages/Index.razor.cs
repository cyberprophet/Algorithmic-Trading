using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

using Newtonsoft.Json;

using System.Globalization;
using System.Net.Http.Json;

namespace ShareInvest.Client.Pages
{
    [Authorize]
    public partial class IndexBase : ComponentBase, IAsyncDisposable
    {
        public event EventHandler<Event.SecuritiesEventArgs>? Send;
        public async ValueTask DisposeAsync()
        {
            if (OnStreamMessage is not null)
            {
                if (Message is not null)
                    await Message.SendAsync("RemoveFromGroup", IsSelectedStock);

                OnStreamMessage.Dispose();
            }
            if (OnBundleMessage is not null)
                OnBundleMessage.Dispose();

            if (OnStringMessage is not null)
                OnStringMessage.Dispose();

            if (OnAccountMessage is not null)
                OnAccountMessage.Dispose();

            if (OnBalanceMessage is not null)
                OnBalanceMessage.Dispose();

            if (OnStockMessage is not null)
                OnStockMessage.Dispose();

            if (OnRealHermes is not null)
                OnRealHermes.Dispose();

            if (OnOperationHermes is not null)
                OnOperationHermes.Dispose();

            if (Hermes is not null)
                await Hermes.DisposeAsync();

            if (Message is not null)
                await Message.DisposeAsync();

            GC.SuppressFinalize(this);
        }
        [JSInvokable]
        public async Task StateHasChanged(string state)
        {
            try
            {
                if (state.Length == 6)
                {
                    if (Pairs is not null && Pairs.TryGetValue(state, out Models.OpenAPI.Stock? stock))
                    {
                        if (state.Equals(IsSelectedStock) is false)
                        {
                            if (Message is not null)
                            {
                                await Message.SendAsync("AddToGroup", state);

                                if (string.IsNullOrEmpty(IsSelectedStock) is false)
                                    await Message.SendAsync("RemoveFromGroup", IsSelectedStock);
                            }
                            if (Hermes is not null && State is not null &&
                                string.IsNullOrEmpty(stock.Date) is false &&
                                (await State).User.Identity?.Name is string name)
                                _ = Task.Run(async () =>
                                {
                                    await Task.Delay(0x100 * 3);
                                    await Hermes.SendAsync("SendAsync", stock.FasteningTime, Interface.Method.opt10004, JsonConvert.SerializeObject(new Interface.OpenAPI.Opt10004
                                    {
                                        Value = new[] { state },
                                        RQName = Interface.OpenAPI.Opt10004.rqName
                                    }));
                                    await Task.Delay(0x100 * 3);
                                    await Hermes.SendAsync("SendAsync", name, Interface.Method.opt10081, JsonConvert.SerializeObject(new Interface.OpenAPI.Opt10081
                                    {
                                        Value = new[]
                                        {
                                            state,
                                            stock.Date,
                                            "1"
                                        },
                                        RQName = Interface.OpenAPI.Opt10081.rqName
                                    }));
                                });
                        }
                        stock.IsShown = stock.IsShown is false;
                        Pairs[state] = stock;
                    }
                    IsSelectedStock = state;
                }
                else
                    IsLoading = Array.Exists(new[]
                    {
                        "zoom_changed"
                    },
                    o => state.Equals(o));
            }
            catch (Exception ex)
            {
                if (Js is not null)
                    await Js.InvokeVoidAsync("console.log", ex.Message);
            }
            finally
            {
                if (IsBoardClicked && Js is not null)
                    await Js.InvokeVoidAsync("console.log", state);

                else
                    StateHasChanged();
            }
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (Http is not null &&
                    await Http.GetFromJsonAsync<Models.OpenAPI.Stock[]>("api/stock?page=map") is Models.OpenAPI.Stock[] models)
                {
                    Pairs = models.ToDictionary(o => o.Code!, o => o);
                    Chart = new Stack<Interface.Chart>();
                }
                if (Navigation is not null)
                {
                    Message = new HubConnectionBuilder()
                        .WithUrl(Navigation.ToAbsoluteUri("/hubs/message"), o => o.AccessTokenProvider = async () =>
                        {
                            if (TokenProvider is not null)
                            {
                                (await TokenProvider.RequestAccessToken()).TryGetToken(out var accessToken);

                                return accessToken.Value;
                            }
                            return null;
                        })
                        .WithAutomaticReconnect()
                        .Build();
                    Hermes = new HubConnectionBuilder()
                        .WithUrl(Navigation.ToAbsoluteUri("/hubs/hermes"), o => o.AccessTokenProvider = async () =>
                        {
                            if (TokenProvider is not null)
                            {
                                (await TokenProvider.RequestAccessToken()).TryGetToken(out var accessToken);

                                return accessToken.Value;
                            }
                            return null;
                        })
                        .WithAutomaticReconnect()
                        .Build();
                }
                if (Message is not null)
                {
                    OnStreamMessage = Message.On<IEnumerable<Interface.Chart>>(nameof(Interface.IHubs.OnReceiveStreamingChart), async chart =>
                    {
                        if (Js is not null && Chart is not null && Pairs is not null)
                        {
                            if (Chart.TryPeek(out Interface.Chart stock))
                            {
                                if (stock.Code.Equals(IsSelectedStock))
                                {
                                    if (stock.Date.CompareTo(chart.LastOrDefault().Date) <= 0)
                                    {
                                        await Js.InvokeVoidAsync("console.log", Chart.Count);

                                        return;
                                    }
                                }
                                else
                                    Chart.Clear();
                            }
                            foreach (var ch in chart)
                                Chart.Push(ch);
                        }
                    });
                    OnBundleMessage = Message.On<string[]>(nameof(Interface.IHubs.OnReceiveBundleMessage), async bundle =>
                    {
                        if (Js is not null && Pairs is not null && string.IsNullOrEmpty(IsSelectedStock) is false &&
                            Pairs.TryGetValue(IsSelectedStock, out Models.OpenAPI.Stock? stock) &&
                            int.TryParse(stock.Price, out int price))
                        {
                            var queue = new Queue<object>();

                            switch (bundle.Length)
                            {
                                case 103:

                                    break;

                                case 69:

                                    for (int i = 0; i < bundle.Length; i++)
                                    {
                                        var color = bundle[i][0] == '-' || bundle[i][0] == '+';

                                        if (int.TryParse(color ? bundle[i][1..] : bundle[i], out int sp))
                                        {
                                            if (i > 0 && i % 3 == 0 && i <= 30 || i > 30 && i % 3 == 1 && i < 60)
                                            {
                                                var percent = sp / (double)price - 1;
                                                queue.Enqueue(new
                                                {
                                                    innerText = sp.ToString("N0"),
                                                    innerPercent = sp > 0 ? (percent > 0 ? string.Concat('+', percent.ToString("P2")) : percent.ToString("P2")) : string.Empty,
                                                    color = sp > 0 ? (color ? (bundle[i][0] == '-' ? "#00bfff" : "#ff0000") : "#f8f8ff") : (i % 3 == 0 ? "#000080" : "#800000")
                                                });
                                                continue;
                                            }
                                            if (i == 0 && bundle[i].Length == 6)
                                            {
                                                await Js.InvokeVoidAsync("setQuotesTime", "stock-quotes-grid-time", bundle[i]);

                                                continue;
                                            }
                                        }
                                    }
                                    break;

                                case 2:
                                    await Js.InvokeVoidAsync("console.log", bundle);
                                    return;
                            }
                            await Js.InvokeVoidAsync("setQuotes", queue);
                        }
                    });
                    OnStringMessage = Message.On<string>(nameof(Interface.IHubs.OnReceiveStringMessage), async message =>
                    {
                        if (Js is not null)
                            await Js.InvokeVoidAsync("setMainDisplayMessage", message);
                    });
                    OnStockMessage = Message.On<Models.OpenAPI.Stock>(nameof(Interface.IHubs.OnReceiveQuoteMessage), async stock =>
                    {
                        if (InformationOnListedStocks is not null && Js is not null &&
                            string.IsNullOrEmpty(stock.Code) is false && stock.Code.Length == 6)
                            switch (IsSelectedLabel)
                            {
                                case (int)LabelOfTitle.Stock when Array.Find(InformationOnListedStocks, o => stock.Code.Equals(o.Code)) is Models.OpenAPI.Stock model:

                                    if (stock.Current?.Length > 1 && stock.Current.Equals(model.Current) is false)
                                    {
                                        if (int.TryParse(stock.Current[0] is '-' ? stock.Current[1..] : stock.Current, out int price) && price > 0 &&
                                            int.TryParse(stock.StartingPrice?[0] is '-' ? stock.StartingPrice[1..] : stock.StartingPrice, out int start) &&
                                            int.TryParse(model.UpperLimit?[0] is '-' ? model.UpperLimit[1..] : model.UpperLimit, out int upper) &&
                                            int.TryParse(model.LowerLimit?[0] is '-' ? model.LowerLimit[1..] : model.LowerLimit, out int lower) &&
                                            int.TryParse(stock.LowPrice?[0] is '-' ? stock.LowPrice[1..] : stock.LowPrice, out int low) &&
                                            int.TryParse(stock.HighPrice?[0] is '-' ? stock.HighPrice[1..] : stock.HighPrice, out int high) &&
                                            double.TryParse(stock.Rate?[0] is '-' ? stock.Rate[1..] : stock.Rate, out double rate) &&
                                            long.TryParse(stock.Volume, out long volume) && volume > 0 &&
                                            int.TryParse(stock.TransactionAmount, out int amount) &&
                                            int.TryParse(stock.CompareToPreviousDay?[0] is '-' ? stock.CompareToPreviousDay[1..] : stock.CompareToPreviousDay, out int day))
                                        {
                                            var color = price - start;
                                            var sn = new SecondaryIndicators.Normalization(upper, lower);
                                            double bottom = sn.Normalize(low), lowerTail = sn.Normalize(start), body = sn.Normalize(price), top = sn.Normalize(high);

                                            if (await Js.InvokeAsync<bool>("drawTheDailyChartBar", new
                                            {
                                                code = stock.Code,
                                                bottom = bottom.ToString("P3"),
                                                lowerTail = ((color > 0 ? lowerTail : body) - bottom).ToString("P3"),
                                                body = (color > 0 ? body - lowerTail : lowerTail - body).ToString("P3"),
                                                top = (top - (color > 0 ? body : lowerTail)).ToString("P3"),
                                                sign = stock.CompareToPreviousSign,
                                                rate = rate == 0 ? string.Empty : (rate * 1e-2).ToString("P2"),
                                                amount = amount.ToString("N0"),
                                                volume = volume.ToString("N0"),
                                                price = price.ToString("N0"),
                                                day = day == 0 ? string.Empty : day.ToString("N0"),
                                                change = stock.CompareToPreviousSign?.Equals(model.CompareToPreviousSign) is false,
                                                color
                                            })
                                            && Condition.IsDebug)
                                                await Js.InvokeVoidAsync("console.log", new
                                                {
                                                    IsSelectedLabel,
                                                    stock.Code,
                                                    model.Name
                                                });
                                        }
                                        if (string.IsNullOrEmpty(stock.FasteningTime) is false)
                                        {
                                            if ("122630".Equals(stock.Code) && stock.FasteningTime.Equals(model.FasteningTime) is false)
                                                await Js.InvokeVoidAsync("setMarketTime", new
                                                {
                                                    change = stock.CompareToPreviousSign?.Equals(model.CompareToPreviousSign) is false,
                                                    sign = stock.CompareToPreviousSign,
                                                    time = stock.FasteningTime.Insert(4, " : ").Insert(2, " : ")
                                                });
                                            model.FasteningTime = stock.FasteningTime;
                                        }
                                        model.Current = stock.Current;
                                        model.StartingPrice = stock.StartingPrice;
                                        model.HighPrice = stock.HighPrice;
                                        model.LowPrice = stock.LowPrice;
                                        model.CompareToPreviousSign = stock.CompareToPreviousSign;
                                        model.CompareToPreviousDay = stock.CompareToPreviousDay;
                                        model.Rate = stock.Rate;
                                    }
                                    model.Volume = stock.Volume;
                                    model.TransactionAmount = stock.TransactionAmount;
                                    break;

                                case (int)LabelOfTitle.Chart when stock.Code.Equals(IsSelectedStock):

                                    break;

                                case (int)LabelOfTitle.Account when Accounts is not null && Accounts.Any(o => o.Balances.Any(o => stock.Code.Equals(o.Code))):

                                    break;
                            }
                        Send?.Invoke(this, new Event.SecuritiesEventArgs(stock));
                    });
                    OnAccountMessage = Message.On<Models.OpenAPI.Account>(nameof(Interface.IHubs.OnReceiveAccountMessage), async account =>
                    {
                        if (string.IsNullOrEmpty(account.AccNo) is false && Js is not null)
                        {
                            if (Accounts is not null && long.TryParse(account.PresumeDeposit, out long deposit))
                            {
                                var param = new
                                {
                                    date = new DateTime(account.Lookup),
                                    label = "D+2예수금",
                                    value = deposit
                                };
                                if (Accounts.Find(o => account.AccNo.Equals(o.AccNo)) is Models.OpenAPI.Account model)
                                {
                                    model.NumberOfPrints = account.NumberOfPrints;
                                    model.Balance = account.Balance;
                                    model.Asset = account.Asset;
                                    model.PresumeAsset = account.PresumeAsset;
                                    model.Deposit = account.Deposit;
                                    model.PresumeDeposit = account.PresumeDeposit;
                                    model.TotalPurchaseAmount = account.TotalPurchaseAmount;
                                    model.Lookup = account.Lookup;
                                    model.Date = account.Date;
                                    account.Acc = model.Acc;
                                }
                                else
                                {
                                    account.Acc = account.AccNo[..^2].Insert(4, "-");
                                    Accounts.Add(account);
                                }
                                await Js.InvokeVoidAsync("setValues", account.Acc, param);
                            }
                            await Js.InvokeVoidAsync("disable", string.Concat(Method, account.AccNo, account.Key));
                            await Js.InvokeVoidAsync("console.log", account);
                        }
                    });
                    OnBalanceMessage = Message.On<Models.OpenAPI.Balance>(nameof(Interface.IHubs.OnReceiveBalanceMessage), async balance =>
                    {
                        if (Js is not null && string.IsNullOrEmpty(balance.AccNo) is false)
                        {
                            if (Accounts is not null && Accounts.Any(o => balance.AccNo.Equals(o.AccNo)) && long.TryParse(balance.Evaluation, out long eva))
                            {
                                string? acc;
                                var param = new
                                {
                                    date = new DateTime(balance.Lookup),
                                    label = balance.Name,
                                    value = eva
                                };
                                if (Accounts.Find(o => balance.AccNo.Equals(o.AccNo) && o.Balances.Any(o => o.Code.Equals(balance.Code))) is Models.OpenAPI.Account model)
                                {
                                    var bal = model.Balances.Single(o => o.Code.Equals(balance.Code));
                                    bal.Amount = balance.Amount;
                                    bal.Rate = balance.Rate;
                                    bal.PreviousPurchaseQuantity = balance.PreviousPurchaseQuantity;
                                    bal.PreviousSalesQuantity = balance.PreviousSalesQuantity;
                                    bal.PurchaseQuantity = balance.PurchaseQuantity;
                                    bal.SalesQuantity = balance.SalesQuantity;
                                    bal.Quantity = balance.Quantity;
                                    bal.PaymentBalance = balance.PaymentBalance;
                                    bal.Average = balance.Average;
                                    bal.Current = balance.Current;
                                    bal.Purchase = balance.Purchase;
                                    bal.Evaluation = balance.Evaluation;
                                    acc = model.Acc;
                                }
                                else
                                {
                                    var account = Accounts.Single(o => balance.AccNo.Equals(o.AccNo));
                                    account.Balances.Add(balance);
                                    acc = account.Acc;
                                }
                                await Js.InvokeVoidAsync("setValues", acc, param);
                            }
                            await Js.InvokeVoidAsync("console.log", balance);
                        }
                    });
                    Message.Closed += async error =>
                    {
                        if (HubConnectionState.Disconnected.Equals(Message.State))
                        {
                            await Task.Delay(new Random().Next(0, 5) * 0x400);
                            await Message.StartAsync();
                        }
                        if (Js is not null && error is not null)
                            await Js.InvokeVoidAsync("console.log", error.Message);
                    };
                    Message.Reconnecting += async error =>
                    {
                        if (Js is not null && error is not null)
                            await Js.InvokeVoidAsync("console.log", error.Message);
                    };
                }
                if (Hermes is not null)
                {
                    OnOperationHermes = Hermes.On<Interface.Operation, string, string>(nameof(Interface.IHubs.OnReceiveOperationMessage), async (operation, ft, rt) =>
                    {
                        if (Js is not null)
                            await Js.InvokeVoidAsync("console.log", operation);
                    });
                    OnRealHermes = Hermes.On<Interface.Method, string>(nameof(Interface.IHubs.OnReceiveMethodMessage), async (method, id) =>
                    {
                        if (Js is not null)
                            switch (method)
                            {
                                case Interface.Method.OPW00004:
                                    await Js.InvokeVoidAsync("console.log", new
                                    {
                                        id,
                                        method
                                    });
                                    break;
                            }
                        Method = method;
                    });
                    Hermes.Closed += async error =>
                    {
                        if (HubConnectionState.Disconnected.Equals(Hermes.State))
                        {
                            await Task.Delay(new Random().Next(0, 5) * 0x400);
                            await Hermes.StartAsync();
                        }
                        if (Js is not null && error is not null)
                            await Js.InvokeVoidAsync("console.log", error.Message);
                    };
                    Hermes.Reconnecting += async error =>
                    {
                        if (Js is not null && error is not null)
                            await Js.InvokeVoidAsync("console.log", error.Message);
                    };
                }
                Accounts = new List<Models.OpenAPI.Account>();
                IsSelectedLabel = (int)LabelOfTitle.Stock;
            }
            catch (Exception ex)
            {
                if (Js is not null)
                    await Js.InvokeVoidAsync("console.log", ex.Message);
            }
            finally
            {
                Send += async (sender, e) =>
                {
                    if (Pairs is not null && Js is not null &&
                        e.Convey is Models.OpenAPI.Stock stock &&
                        string.IsNullOrEmpty(stock.Code) is false &&
                        Pairs.TryGetValue(stock.Code, out Models.OpenAPI.Stock? ms) &&
                        ms.Overview is not null &&
                        string.IsNullOrEmpty(stock.CompareToPreviousSign) is false &&
                        stock.Current?.Length > 1 && stock.Current.Equals(ms.Current) is false)
                    {
                        if (stock.CompareToPreviousSign.Equals(ms.CompareToPreviousSign) is false)
                        {
                            await Js.InvokeVoidAsync("setIcon", stock.Code, Condition.GetIcon(stock.CompareToPreviousSign));
                            ms.CompareToPreviousSign = stock.CompareToPreviousSign;
                        }
                        if (int.TryParse(stock.FasteningTime, out int sft) && int.TryParse(ms.FasteningTime, out int mft) && sft > mft + 3 &&
                            int.TryParse(stock.CompareToPreviousDay?[0] is '-' ? stock.CompareToPreviousDay[1..] : stock.CompareToPreviousDay, out int day) &&
                            int.TryParse(stock.Current[0] is '-' ? stock.Current[1..] : stock.Current, out int price) && price > 0 &&
                            double.TryParse(stock.Rate?[0] is '-' ? stock.Rate[1..] : stock.Rate, out double rate))
                        {
                            await Js.InvokeVoidAsync("setContentWindows", IsPlayed || ms.IsShown, new
                            {
                                code = stock.Code,
                                name = ms.Name,
                                classification = ms.Overview.Classification?[0] == 'K',
                                sign = stock.CompareToPreviousSign,
                                html = Condition.GetInnerHTML(stock.CompareToPreviousSign, day.ToString("N0")),
                                after = price.ToString("N0"),
                                before = rate > 0 ? (rate * 1e-2).ToString("P2") : string.Empty
                            });
                            ms.FasteningTime = stock.FasteningTime;
                        }
                        ms.CompareToPreviousDay = stock.CompareToPreviousDay;
                        ms.Rate = stock.Rate;
                        ms.Current = stock.Current;
                        Pairs[stock.Code] = ms;
                    }
                };
                if (Message is not null &&
                    HubConnectionState.Disconnected.Equals(Message.State))
                    await Message.StartAsync();

                if (Hermes is not null &&
                    HubConnectionState.Disconnected.Equals(Hermes.State))
                    await Hermes.StartAsync();

                await base.OnInitializedAsync();
            }
        }
        protected override async Task OnAfterRenderAsync(bool first)
        {
            if (Js is not null)
            {
                if (Pairs?.Count > 0)
                    switch (Render++)
                    {
                        case 2:
                            await Js.InvokeVoidAsync("setGridOpacity", 975e-3);
                            break;

                        case 3 when InformationOnListedStocks is not null:

                            var task = Task.Run(async () =>
                            {
                                var stack = new Stack<object>();

                                foreach (var item in InformationOnListedStocks)
                                {
                                    if (int.TryParse(item.Volume, out int vol) && vol == 0 &&
                                        item.State?.Split('|') is string[] state && state.Length > 1 && "거래정지".Equals(state[1]))
                                        continue;

                                    if (int.TryParse(item.UpperLimit, out int upper) && int.TryParse(item.LowerLimit, out int lower))
                                    {
                                        var sn = new SecondaryIndicators.Normalization(Math.Abs(upper), Math.Abs(lower));
                                        double bottom = double.NaN, lowerTail = double.NaN, body = double.NaN, top = double.NaN;
                                        int price = 0, start = 0, color;

                                        if (int.TryParse(item.LowPrice, out int low))
                                            bottom = sn.Normalize(Math.Abs(low));

                                        if (int.TryParse(item.Current, out int current))
                                        {
                                            body = sn.Normalize(Math.Abs(current));
                                            price = Math.Abs(current);
                                        }
                                        if (int.TryParse(item.HighPrice, out int high))
                                            top = sn.Normalize(Math.Abs(high));

                                        if (int.TryParse(item.StartingPrice, out int sp))
                                        {
                                            lowerTail = sn.Normalize(Math.Abs(sp));
                                            start = Math.Abs(sp);
                                        }
                                        color = price - start;
                                        var draw = new
                                        {
                                            code = item.Code,
                                            bottom = bottom.ToString("P3"),
                                            lowerTail = ((color > 0 ? lowerTail : body) - bottom).ToString("P3"),
                                            body = (color > 0 ? body - lowerTail : lowerTail - body).ToString("P3"),
                                            top = (top - (color > 0 ? body : lowerTail)).ToString("P3"),
                                            color
                                        };
                                        if ((double.IsNormal(bottom) || bottom == 0) &&
                                            (double.IsNormal(lowerTail) || lowerTail == 0) &&
                                            (double.IsNormal(body) || body == 0) &&
                                            (double.IsNormal(top) || top == 0) &&
                                            price > 0 && start > 0 &&
                                            await Js.InvokeAsync<bool>("drawTheDailyChartBar", draw))
                                        {
                                            stack.Push(draw);
                                            await Task.Delay(0x200);
                                            await Js.InvokeVoidAsync("console.log", draw);
                                        }
                                    }
                                }
                                while (stack.Count > 0 && stack.TryPop(out object? pop))
                                    if (await Js.InvokeAsync<bool>("drawTheDailyChartBar", pop))
                                    {
                                        stack.Push(pop);
                                        await Task.Delay(0x200);
                                    }
                            });
                            break;

                        case 1 when Http is not null:
                            InformationOnListedStocks = await Http.GetFromJsonAsync<Models.OpenAPI.Stock[]>("api/stock?page=left");
                            SortListedStocks = from o in InformationOnListedStocks
                                               orderby o.Code
                                               select o;
                            break;

                        case 0:
                            var stack = new Stack<object>();

                            foreach (var kv in Pairs)
                                if (kv.Value.Overview is not null &&
                                    int.TryParse(kv.Value.CompareToPreviousDay?[0] is '-' ? kv.Value.CompareToPreviousDay[1..] : kv.Value.CompareToPreviousDay, out int day) &&
                                    int.TryParse(kv.Value.Current?[0] is '-' ? kv.Value.Current[1..] : kv.Value.Current, out int current) &&
                                    double.TryParse(kv.Value.Rate?[0] is '-' ? kv.Value.Rate[1..] : kv.Value.Rate, out double rate))
                                    stack.Push(new
                                    {
                                        position = new
                                        {
                                            lat = kv.Value.Overview.Latitude,
                                            lng = kv.Value.Overview.Longitude
                                        },
                                        png = Condition.GetIcon(kv.Value.CompareToPreviousSign),
                                        classification = kv.Value.Overview.Classification?[0] == 'K',
                                        name = kv.Value.Name,
                                        contents = new
                                        {
                                            code = kv.Key,
                                            sign = kv.Value.CompareToPreviousSign,
                                            html = Condition.GetInnerHTML(kv.Value.CompareToPreviousSign, day.ToString("N0")),
                                            after = current.ToString("N0"),
                                            before = rate > 0 ? (rate * 1e-2).ToString("P2") : string.Empty
                                        },
                                        code = kv.Key
                                    });
                            await Js.InvokeVoidAsync("initMap", stack);
                            break;
                    }
                if (first)
                {
                    await Js.InvokeVoidAsync("load", new
                    {
                        packages = new[] { "corechart", "controls", "annotationchart" },
                        language = "ko-KR"
                    });
                    await Js.InvokeVoidAsync("geolocation", DotNetObjectReference.Create(this));
                }
            }
            await base.OnAfterRenderAsync(first);
        }
        protected internal async Task OnClick(Models.OpenAPI.Stock stock, MouseEventArgs _)
        {
            if (string.IsNullOrEmpty(stock.Code) is false && Js is not null && stock.Overview is not null)
                await Js.InvokeVoidAsync("panTo", stock.Code, stock.Overview.Latitude, stock.Overview.Longitude);
        }
        protected internal async Task OnClick(Interface.OpenAPI.TR tr, MouseEventArgs _)
        {
            if (tr.Value is not null)
            {
                Method = tr switch
                {
                    Interface.OpenAPI.OPW00004 => Interface.Method.OPW00004,
                    _ => Interface.Method.Error
                };
                if (Hermes is not null && HubConnectionState.Connected.Equals(Hermes.State))
                    await Hermes.SendAsync("SendAsync", tr.Value[^1], Method, JsonConvert.SerializeObject(tr switch
                    {
                        Interface.OpenAPI.OPW00004 => new Interface.OpenAPI.OPW00004
                        {
                            PrevNext = 0,
                            Value = new[] { tr.Value[0], string.Empty, "0", "00" }
                        },
                        _ => string.Empty
                    }));
                if (Js is not null)
                    await Js.InvokeVoidAsync("disable", string.Concat(tr.GetType().Name, tr.Value[0], tr.Value[^1]));
            }
        }
        protected internal void OnClick(char selectedLabel, MouseEventArgs _)
        {
            Task.Run(async () =>
            {
                if (Js is not null)
                {
                    switch ((LabelOfSubTitle)selectedLabel)
                    {
                        case LabelOfSubTitle.Chat:

                            break;

                        case LabelOfSubTitle.Quotes:

                            break;
                    }
                    await Js.InvokeVoidAsync("console.log", (LabelOfSubTitle)selectedLabel);
                }
            });
            IsSelectedSubTitle = selectedLabel;
        }
        protected internal void OnClick(int selectedLabel, MouseEventArgs _)
        {
            Task.Run(async () =>
            {
                if (Js is not null)
                    switch ((LabelOfTitle)selectedLabel)
                    {
                        case LabelOfTitle.Stock when InformationOnListedStocks is not null:

                            foreach (var item in InformationOnListedStocks)
                            {
                                if (int.TryParse(item.Volume, out int vol) && vol == 0 &&
                                    item.State?.Split('|') is string[] state && state.Length > 1 && "거래정지".Equals(state[1]))
                                    continue;

                                if (int.TryParse(item.UpperLimit, out int upper) && int.TryParse(item.LowerLimit, out int lower))
                                {
                                    var sn = new SecondaryIndicators.Normalization(Math.Abs(upper), Math.Abs(lower));
                                    double bottom = double.NaN, lowerTail = double.NaN, body = double.NaN, top = double.NaN;
                                    int price = 0, start = 0, color;

                                    if (int.TryParse(item.LowPrice, out int low))
                                        bottom = sn.Normalize(Math.Abs(low));

                                    if (int.TryParse(item.Current, out int current))
                                    {
                                        body = sn.Normalize(Math.Abs(current));
                                        price = Math.Abs(current);
                                    }
                                    if (int.TryParse(item.HighPrice, out int high))
                                        top = sn.Normalize(Math.Abs(high));

                                    if (int.TryParse(item.StartingPrice, out int sp))
                                    {
                                        lowerTail = sn.Normalize(Math.Abs(sp));
                                        start = Math.Abs(sp);
                                    }
                                    color = price - start;
                                    var draw = new
                                    {
                                        code = item.Code,
                                        bottom = bottom.ToString("P3"),
                                        lowerTail = ((color > 0 ? lowerTail : body) - bottom).ToString("P3"),
                                        body = (color > 0 ? body - lowerTail : lowerTail - body).ToString("P3"),
                                        top = (top - (color > 0 ? body : lowerTail)).ToString("P3"),
                                        color
                                    };
                                    if ((double.IsNormal(bottom) || bottom == 0) &&
                                        (double.IsNormal(lowerTail) || lowerTail == 0) &&
                                        (double.IsNormal(body) || body == 0) &&
                                        (double.IsNormal(top) || top == 0) &&
                                        price > 0 && start > 0 &&
                                        await Js.InvokeAsync<bool>("drawTheDailyChartBar", draw))
                                        await Task.Delay(0x200);

                                    else
                                        await Js.InvokeVoidAsync("console.log", draw);
                                }
                            }
                            await Js.InvokeVoidAsync("setResizeTimer", 3 * 0x200);
                            break;

                        case LabelOfTitle.Chart when Chart is not null:

                            if (Chart.Count == 0 && Hermes is not null && State is not null && Pairs is not null &&
                                string.IsNullOrEmpty(IsSelectedStock) is false &&
                                Pairs.TryGetValue(IsSelectedStock, out Models.OpenAPI.Stock? ms) &&
                                string.IsNullOrEmpty(ms.Date) is false &&
                                (await State).User.Identity?.Name is string user)
                            {
                                await Hermes.SendAsync("SendAsync", user, Interface.Method.opt10081, JsonConvert.SerializeObject(new Interface.OpenAPI.Opt10081
                                {
                                    Value = new[]
                                    {
                                        IsSelectedStock,
                                        ms.Date,
                                        "1"
                                    },
                                    RQName = Interface.OpenAPI.Opt10081.rqName
                                }));
                                await Task.Delay(0x200 * 5);
                            }
                            var queue = new Queue<object>();
                            var name = string.Empty;

                            while (Chart.TryPop(out Interface.Chart chart))
                            {
                                if (DateTime.TryParseExact(chart.Date, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime dt) &&
                                    int.TryParse(chart.Start, out int start) &&
                                    int.TryParse(chart.Current, out int current) &&
                                    int.TryParse(chart.High, out int high) &&
                                    int.TryParse(chart.Low, out int low))
                                    queue.Enqueue(new
                                    {
                                        c = new object[]
                                        {
                                            new { v = dt },
                                            new { v = start > current ? high : low },
                                            new { v = start },
                                            new { v = current },
                                            new { v = start > current ? low : high }
                                        }
                                    });
                                name = chart.Name;
                            }
                            if (queue.Count > 0)
                                await Js.InvokeVoidAsync("drawCandlestickChart", new
                                {
                                    cols = new[]
                                    {
                                        new { type = "date" },
                                        new { type = "number" },
                                        new { type = "number" },
                                        new { type = "number" },
                                        new { type = "number" }
                                    },
                                    rows = queue
                                },
                                name);
                            break;

                        case LabelOfTitle.Account when Accounts is not null && Accounts.Count > 0:
                            List<Models.OpenAPI.Account>? list = null;
                            var label = new[]
                            {
                                new { label = "Date", type = "date" },
                                new { label = "D+2예수금", type = "number" }
                            };
                            foreach (var acc in Accounts)
                            {
                                var rows = new Queue<object>();

                                if (Http is not null &&
                                await Http.GetFromJsonAsync<Models.OpenAPI.Account[]?>($"api/chart?account={acc.AccNo}") is Models.OpenAPI.Account[] models)
                                {
                                    var cols = label.ToList();

                                    if (Accounts.Count > 1)
                                    {
                                        if (list is null)
                                            list = new List<Models.OpenAPI.Account>(models);

                                        else
                                            list.AddRange(models);
                                    }
                                    foreach (var bal in from o in models
                                                        orderby o.Lookup ascending
                                                        select o.Balances)
                                        foreach (var m in bal)
                                            if (string.IsNullOrEmpty(m.Code) is false && cols.Any(o => m.Code.Equals(o.label)) is false)
                                                cols.Add(new
                                                {
                                                    label = m.Code,
                                                    type = "number"
                                                });
                                    foreach (var ac in from o in models
                                                       orderby o.Lookup ascending
                                                       select o)
                                        if (string.IsNullOrEmpty(ac.Date) is false && long.TryParse(ac.PresumeDeposit, out long deposit))
                                        {
                                            var c = new object[cols.Count];
                                            c[0] = new
                                            {
                                                v = new DateTime(ac.Lookup)
                                            };
                                            c[1] = new
                                            {
                                                v = deposit
                                            };
                                            for (int i = 2; i < c.Length; i++)
                                                c[i] = new
                                                {
                                                    v = long.TryParse(ac.Balances.SingleOrDefault(o => cols[i].label.Equals(o.Code))?.Evaluation, out long eval) ? eval : 0
                                                };
                                            rows.Enqueue(new
                                            {
                                                c
                                            });
                                        }
                                    for (int i = 2; i < cols.Count; i++)
                                        if (Pairs is not null && cols[i].label.Length == 6)
                                            cols[i] = new
                                            {
                                                label = Pairs.TryGetValue(cols[i].label, out Models.OpenAPI.Stock? stock) && string.IsNullOrEmpty(stock.Name) is false ? stock.Name : Condition.GetStockName(models, cols[i].label),
                                                type = "number"
                                            };
                                    if (rows.Count > 0)
                                        await Js.InvokeVoidAsync("draw", string.Concat(acc.Key, "period", acc.AccNo), string.Concat(acc.Key, "chart", acc.AccNo), new
                                        {
                                            cols,
                                            rows
                                        },
                                        acc.Acc);
                                }
                            }
                            if (list is not null && list.Count > 0)
                            {
                                var cols = label.ToList();
                                var rows = new Queue<object>();
                                var arr = (from o in list
                                           orderby o.Lookup ascending
                                           select new
                                           {
                                               o.Date,
                                               o.Balances
                                           })
                                           .ToArray();
                                for (int index = 0; index < arr.Length; index++)
                                {
                                    foreach (var m in arr[index].Balances)
                                        if (string.IsNullOrEmpty(m.Code) is false && cols.Any(o => m.Code.Equals(o.label)) is false)
                                            cols.Add(new
                                            {
                                                label = m.Code,
                                                type = "number"
                                            });
                                    if ((index == 0 || index == arr.Length - 1 || arr[index + 1].Date?.CompareTo(arr[index].Date) > 0) &&
                                        DateTime.TryParseExact(arr[index].Date, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime dt))
                                    {
                                        var c = new object[cols.Count];
                                        c[0] = new
                                        {
                                            v = dt
                                        };
                                        c[1] = new
                                        {
                                            v = list.Where(o => o.Date.Equals(arr[index].Date)).Sum(o => long.TryParse(o.PresumeDeposit, out long deposit) ? deposit : 0)
                                        };
                                        for (int i = 2; i < cols.Count; i++)
                                        {
                                            var sum = 0L;

                                            foreach (var bal in from o in list
                                                                where o.Date.Equals(arr[index].Date)
                                                                select o.Balances)
                                                if (bal.Count > 0)
                                                    sum += bal.Where(o => cols[i].label.Equals(o.Code)).Sum(o => long.TryParse(o.Evaluation, out long eva) ? eva : 0);

                                            c[i] = new
                                            {
                                                v = sum
                                            };
                                        }
                                        rows.Enqueue(new
                                        {
                                            c
                                        });
                                    }
                                }
                                for (int i = 2; i < cols.Count; i++)
                                    if (Pairs is not null && cols[i].label.Length == 6)
                                        cols[i] = new
                                        {
                                            label = Pairs.TryGetValue(cols[i].label, out Models.OpenAPI.Stock? stock) && string.IsNullOrEmpty(stock.Name) is false ? stock.Name : Condition.GetStockName(list, cols[i].label),
                                            type = "number"
                                        };
                                if (rows.Count > 0)
                                    await Js.InvokeVoidAsync("draw", "total-period-account", "total-chart-account", new
                                    {
                                        cols,
                                        rows
                                    },
                                    string.Empty);
                            }
                            break;
                    }
            });
            IsSelectedLabel = selectedLabel;
        }
        protected internal async Task OnClick(MouseEventArgs _)
        {
            if (State is not null && (await State).User.Identity?.Name is string name && Http is not null &&
                await Http.GetFromJsonAsync<Interface.Initialization[]>($"api/user?name={name}") is Interface.Initialization[] securities &&
                securities.Length > 0 &&
                Accounts is not null)
                foreach (var key in securities)
                    switch (key.Changes)
                    {
                        case (int)Interface.Securities.Kiwoom
                        when await Http.GetFromJsonAsync<Models.OpenAPI.Account[]>($"api/account?key={key.Id}") is Models.OpenAPI.Account[] models:
                            Accounts.AddRange(models);
                            break;
                    }
            IsBoardClicked = IsBoardClicked is false;
        }
        protected internal void OnBoardClick(MouseEventArgs _)
        {
            Accounts?.Clear();
            IsBoardClicked = IsBoardClicked is false;
        }
        protected internal void OpenTheInfoWindow(MouseEventArgs _) => IsUnfold = IsUnfold is false;
        protected internal void PressTheButton(MouseEventArgs _) => IsPlayed = IsPlayed is false;
        protected internal bool GetDisable(int label) => ((int)LabelOfTitle.Chart == label || (int)LabelOfSubTitle.Quotes == label) && string.IsNullOrEmpty(IsSelectedStock);
        protected internal string GetName(int label)
        {
            if (((int)LabelOfTitle.Chart == label || (int)LabelOfSubTitle.Quotes == label) &&
                InformationOnListedStocks is not null &&
                string.IsNullOrEmpty(IsSelectedStock) is false &&
                Array.Find(InformationOnListedStocks, o => IsSelectedStock.Equals(o.Code))?.Name is string name)
                return name;

            return string.Empty;
        }
        protected internal int IsSelectedLabel
        {
            get; private set;
        }
        protected internal int IsSelectedSubTitle
        {
            get; private set;
        }
        protected internal uint Render
        {
            get; private set;
        }
        protected internal bool IsBoardClicked
        {
            get; private set;
        }
        protected internal bool IsLoading
        {
            get; private set;
        }
        protected internal bool IsUnfold
        {
            get; private set;
        }
        protected internal bool IsPlayed
        {
            get; private set;
        }
        protected internal string? IsSelectedStock
        {
            get; private set;
        }
        protected internal IOrderedEnumerable<Models.OpenAPI.Stock>? SortListedStocks
        {
            get; private set;
        }
        protected internal List<Models.OpenAPI.Account>? Accounts
        {
            get; private set;
        }
        protected internal Dictionary<string, Models.OpenAPI.Stock>? Pairs
        {
            get; private set;
        }
        [Inject]
        protected internal NavigationManager? Navigation
        {
            get; set;
        }
        [Inject]
        IJSRuntime? Js
        {
            get; set;
        }
        IDisposable? OnBundleMessage
        {
            get; set;
        }
        IDisposable? OnStreamMessage
        {
            get; set;
        }
        IDisposable? OnStringMessage
        {
            get; set;
        }
        IDisposable? OnAccountMessage
        {
            get; set;
        }
        IDisposable? OnBalanceMessage
        {
            get; set;
        }
        IDisposable? OnStockMessage
        {
            get; set;
        }
        IDisposable? OnRealHermes
        {
            get; set;
        }
        IDisposable? OnOperationHermes
        {
            get; set;
        }
        [Inject]
        IAccessTokenProvider? TokenProvider
        {
            get; set;
        }
        [Inject]
        HttpClient? Http
        {
            get; set;
        }
        [CascadingParameter]
        Task<AuthenticationState>? State
        {
            get; set;
        }
        HubConnection? Message
        {
            get; set;
        }
        HubConnection? Hermes
        {
            get; set;
        }
        Models.OpenAPI.Stock[]? InformationOnListedStocks
        {
            get; set;
        }
        Interface.Method Method
        {
            get; set;
        }
        Stack<Interface.Chart>? Chart
        {
            get; set;
        }
        protected internal static string GetCurrentColor(char initial)
        {
            if (char.IsDigit(initial))
                return white;

            return '+' == initial ? red : blue;
        }
        const string white = "flat";
        const string red = "up";
        const string blue = "down";
    }
    enum LabelOfSubTitle
    {
        Chat = 'C',
        Quotes = 'Q'
    }
    enum LabelOfTitle
    {
        Account = 'A',
        Stock = 'S',
        Chart = 'T'
    }
}