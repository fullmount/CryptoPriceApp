using Binance.Net.Interfaces.Clients;

namespace Core.ExchangeWrappers
{
    public class BinanceClientWrapper(IBinanceRestClient restClient, IBinanceSocketClient socketClient) : IExchangeClient
    {
        public string ExchangeName => "Binance";
        private readonly IBinanceRestClient _restClient = restClient;
        private readonly IBinanceSocketClient _socketClient = socketClient;

        public async Task<decimal?> GetPriceAsync(string symbol)
        {
            var result = await _restClient.SpotApi.ExchangeData.GetPriceAsync(symbol);
            return result.Success ? result.Data.Price : null;
        }

        public async Task SubscribeToWebSockets(string symbol, Action<string, decimal> onPriceUpdate)
        {
            await _socketClient.SpotApi.ExchangeData.SubscribeToTickerUpdatesAsync(symbol, data =>
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
