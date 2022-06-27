using ShareInvest.Models.OpenAPI;

namespace ShareInvest.Interface
{
    public interface IHubs
    {
        Task OnReceiveBundleMessage(string[] quotes);
        Task OnReceiveQuoteMessage(Stock stock);
        Task OnReceiveOperationMessage(Operation operation, string fasteningTime, string remainingTime);
        Task OnReceiveMethodMessage(Method method, string json);
        Task OnReceiveAccountMessage(Account account);
        Task OnReceiveBalanceMessage(Balance balance);
        Task OnReceiveMessage(Message message);
        Task OnReceiveStringMessage(string message);
        Task OnReceiveStreamingChart(IEnumerable<Chart> enumerable);
    }
}