using Core;
using Core.Services;
using Moq;

namespace CryptoPriceApp.tests.Integration
{
    public class ExchangeServiceIntegrationTests(ExchangeServiceFixture fixture) : IClassFixture<ExchangeServiceFixture>
    {
        private readonly ExchangeService _service = fixture.Service;

        [Fact]
        public async Task GetPricesAsync_ShouldReturnPricesFromRealClients()
        {
            var result = await _service.GetPricesAsync("BTC-USDT");

            Assert.NotEmpty(result);
            Assert.All(result.Values, price => Assert.NotNull(price));
            Assert.All(result.Values, price => Assert.True(price > 0));
        }

        [Fact]
        public async Task SubscribeToWebSockets_ShouldReceiveUpdates()
        {
            var receivedPrices = new Dictionary<string, decimal>();

            void OnPriceUpdate(string exchange, decimal price)
            {
                receivedPrices[exchange] = price;
            }

            void OnError(string error)
            {
                Assert.Fail($"Ошибка подписки: {error}");
            }

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await _service.SubscribeToWebSockets("BTC-USDT", OnPriceUpdate, OnError, cts.Token);

            
            await Task.Delay(5000);
            await _service.UnsubscribeFromWebSockets();

            Assert.NotEmpty(receivedPrices);
            Assert.All(receivedPrices.Values, price => Assert.True(price > 0));
        }

        [Fact]
        public async Task UnsubscribeFromWebSockets_ShouldStopReceivingUpdates()
        {
            var receivedPrices = new List<decimal>();

            void OnPriceUpdate(string exchange, decimal price)
            {
                receivedPrices.Add(price);
            }

            void OnError(string error)
            {
                Assert.Fail($"Ошибка подписки: {error}");
            }

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await _service.SubscribeToWebSockets("BTC-USDT", OnPriceUpdate, OnError, cts.Token);
            await Task.Delay(5000);

            await _service.UnsubscribeFromWebSockets();
            int countBefore = receivedPrices.Count;

            await Task.Delay(5000);
            int countAfter = receivedPrices.Count;

            Assert.Equal(countBefore, countAfter);
        }

    }

}
