namespace Core
{
    public interface IExchangeClient
    {
        string ExchangeName { get; }
        Task<decimal?> GetPriceAsync(string symbol);
        Task SubscribeToWebSockets(string symbol, Action<string, decimal> onPriceUpdate);
        Task UnsubscribeFromWebSockets();
    }
}
