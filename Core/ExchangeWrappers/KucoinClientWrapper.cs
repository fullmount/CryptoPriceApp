using Kucoin.Net.Interfaces.Clients;

namespace Core.ExchangeWrappers
{
    public class KucoinClientWrapper(IKucoinRestClient restClient, IKucoinSocketClient socketClient) : IExchangeClient
    {
        public string ExchangeName => "Kucoin";
        private readonly IKucoinRestClient _restClient = restClient;
        private readonly IKucoinSocketClient _socketClient = socketClient;

        public async Task<decimal?> GetPriceAsync(string symbol)
        {
            var result = await _restClient.SpotApi.ExchangeData.GetTickerAsync(symbol);
            return result.Success ? result.Data.LastPrice : null;
        }

        public async Task SubscribeToWebSockets(string symbol, Action<string, decimal> onPriceUpdate)
        {
            await _socketClient.SpotApi.SubscribeToTickerUpdatesAsync(symbol, data =>
            {
                onPriceUpdate?.Invoke(data.Data.Symbol, data.Data.LastPrice ?? 0);
            });
        }

        public async Task UnsubscribeFromWebSockets()
        {
            await _socketClient.UnsubscribeAllAsync();
        }
    }
}
