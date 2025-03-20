using System.Diagnostics;

namespace Core.Services
{
    public class ExchangeService(IEnumerable<IExchangeClient> clients)
    {
        private readonly List<IExchangeClient> _clients = [.. clients.DistinctBy(c => c.ExchangeName)];
        private const int MaxRetryCount = 5;

        public async Task<Dictionary<string, decimal?>> GetPricesAsync(string symbol, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return [];
            }
            try
            {
                IEnumerable<Task<(string Name, decimal? Price)>> tasks = _clients.Select(async client =>
                (
                    client.ExchangeName,
                    await client.GetPriceAsync(ConvertToNonKucoinSymbol(client.ExchangeName, symbol))
                ));

                var results = await Task.WhenAll(tasks);
                return results.ToDictionary(r => r.Name, r => r.Price);
            }
            catch
            {
                return [];
            }
        }

        public async Task SubscribeToWebSockets(string symbol, Action<string, decimal> onPriceUpdate, Action<string>? onError, CancellationToken token)
        {
            await UnsubscribeFromWebSockets();

            if (token.IsCancellationRequested)
            {
                return;
            }

            foreach (var client in _clients)
            {
                string convertedSymbol = ConvertToNonKucoinSymbol(client.ExchangeName, symbol);
                int attemptNo = 0;

                while (attemptNo < MaxRetryCount)
                {
                    try
                    {
                        await client.SubscribeToWebSockets(convertedSymbol, (exchange, price) =>
                        {
                            try
                            {
                                onPriceUpdate?.Invoke(client.ExchangeName, price);
                            }
                            catch (Exception callbackEx)
                            {
                                onError?.Invoke($"Ошибка в callback для {client.ExchangeName}: {callbackEx.Message}");
                            }
                        });

                        break;
                    }
                    catch (Exception ex)
                    {
                        onError?.Invoke($"Ошибка WebSocket {client.ExchangeName}: {ex.Message}");

                        if (++attemptNo >= MaxRetryCount)
                        {
                            break;
                        }

                        await Task.Delay(5000, token);
                    }
                }
            }
        }


        public async Task UnsubscribeFromWebSockets()
        {
            foreach (var client in _clients)
            {
                try
                {
                    await client.UnsubscribeFromWebSockets();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ошибка при отписке от {client.ExchangeName}: {ex}");
                }
            }
        }

        /// У Kucoin все тикеры через тире, проще добавлять все через тире и убирать у остальных, чем понять, куда именно вставлять тире в тикеры Кукоин
        /// Если например будем в будущем добавлять тикеры вроде KARRAT-BTC или USDT-TRX. Плюс читаемость лучше
        private static string ConvertToNonKucoinSymbol(string name, string symbol)
        {
            return name.Contains("kucoin", StringComparison.CurrentCultureIgnoreCase) ? symbol : symbol.Replace("-", "");
        }
    }
}
