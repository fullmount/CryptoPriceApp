using Bybit.Net.Interfaces.Clients;

namespace Core.ExchangeWrappers
{
    public class BybitClientWrapper(IBybitRestClient restClient, IBybitSocketClient socketClient) : IExchangeClient
    {
        public string ExchangeName => "Bybit";
        private readonly IBybitRestClient _restClient = restClient;
        private readonly IBybitSocketClient _socketClient = socketClient;

        public async Task<decimal?> GetPriceAsync(string symbol)
        {
            var result = await _restClient.V5Api.ExchangeData.GetSpotTickersAsync(symbol);
            return result.Success ? result.Data.List.First()?.LastPrice : null;
        }

        public async Task SubscribeToWebSockets(string symbol, Action<string, decimal> onPriceUpdate)
        {
            await _socketClient.V5SpotApi.SubscribeToTickerUpdatesAsync(symbol, data =>
            {
                onPriceUpdate?.Invoke(data.Data.Symbol, data.Data.LastPrice);
            });
        }

        public async Task UnsubscribeFromWebSockets()
        {
            await _socketClient.UnsubscribeAllAsync();
        }
    }
}
