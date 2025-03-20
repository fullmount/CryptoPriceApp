using Bitget.Net.Interfaces.Clients;

namespace Core.ExchangeWrappers
{
    public class BitgetClientWrapper(IBitgetRestClient restClient, IBitgetSocketClient socketClient) : IExchangeClient
    {
        public string ExchangeName => "Bitget";
        private readonly IBitgetRestClient _restClient = restClient;
        private readonly IBitgetSocketClient _socketClient = socketClient;

        public async Task<decimal?> GetPriceAsync(string symbol)
        {
            var result = await _restClient.SpotApi.ExchangeData.GetTickerAsync(ConvertToBitgetTicker(symbol));
            return result.Success ? result.Data.ClosePrice : null;
        }

        public async Task SubscribeToWebSockets(string symbol, Action<string, decimal> onPriceUpdate)
        {
            await _socketClient.SpotApi.SubscribeToTickerUpdatesAsync(symbol, data =>
            {
                onPriceUpdate?.Invoke(data.Data.Symbol, data.Data.LastPrice);
            });
        }

        public async Task UnsubscribeFromWebSockets()
        {
            await _socketClient.UnsubscribeAllAsync();
        }

        private static string ConvertToBitgetTicker(string symbol)
        {
            return symbol + "_SPBL";
        }
    }
}
