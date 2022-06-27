using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace ShareInvest.Client.Shared
{
    public partial class MainLayoutBase : LayoutComponentBase, IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            if (On is not null)
                On.Dispose();

            if (Hub is not null)
                await Hub.DisposeAsync();

            GC.SuppressFinalize(this);
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (Manager is not null)
                {
                    Hub = new HubConnectionBuilder()
                        .WithUrl(Manager.ToAbsoluteUri("/hubs/message"), o => o.AccessTokenProvider = async () =>
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
                if (Hub is not null)
                {
                    On = Hub.On<string>(nameof(Interface.IHubs.OnReceiveStringMessage), async message =>
                    {
                        if (Js is not null)
                            await Js.InvokeVoidAsync("setMainDisplayMessage", message);
                    });
                    Hub.Closed += async error =>
                    {
                        if (HubConnectionState.Disconnected.Equals(Hub.State))
                        {
                            await Task.Delay(new Random().Next(0, 5) * 0x400);
                            await Hub.StartAsync();
                        }
                        if (Js is not null && error is not null)
                            await Js.InvokeVoidAsync("console.log", error.Message);
                    };
                    Hub.Reconnecting += async error =>
                    {
                        if (Js is not null && error is not null)
                            await Js.InvokeVoidAsync("console.log", error.Message);
                    };
                }
            }
            catch (Exception ex)
            {
                if (Js is not null)
                    await Js.InvokeVoidAsync("console.log", ex.Message);
            }
            finally
            {
                if (Hub is not null &&
                    HubConnectionState.Disconnected.Equals(Hub.State))
                    await base.OnInitializedAsync();
            }
        }
        HubConnection? Hub
        {
            get; set;
        }
        [Inject]
        NavigationManager? Manager
        {
            get; set;
        }
        [Inject]
        IJSRuntime? Js
        {
            get; set;
        }
        IDisposable? On
        {
            get; set;
        }
        [Inject]
        IAccessTokenProvider? TokenProvider
        {
            get; set;
        }
    }
}