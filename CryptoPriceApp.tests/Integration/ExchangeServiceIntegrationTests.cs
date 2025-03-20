using Core;
using Core.Services;
using Moq;

namespace CryptoPriceApp.tests.Integration
{
    public class ExchangeServiceIntegrationTests
    {
        private readonly Mock<IExchangeClient> _mockBinanceClient;
        private readonly Mock<IExchangeClient> _mockBybitClient;
        private readonly Mock<IExchangeClient> _mockKucoinClient;
        private readonly Mock<IExchangeClient> _mockBitgetClient;
        private readonly ExchangeService _exchangeService;

        public ExchangeServiceIntegrationTests()
        {
            _mockBinanceClient = new Mock<IExchangeClient>();
            _mockBybitClient = new Mock<IExchangeClient>();
            _mockKucoinClient = new Mock<IExchangeClient>();
            _mockBitgetClient = new Mock<IExchangeClient>();

            // Устанавливаем уникальные имена бирж
            _mockBinanceClient.Setup(c => c.ExchangeName).Returns("Binance");
            _mockBybitClient.Setup(c => c.ExchangeName).Returns("Bybit");
            _mockKucoinClient.Setup(c => c.ExchangeName).Returns("Kucoin");
            _mockBitgetClient.Setup(c => c.ExchangeName).Returns("Bitget");

            var clients = new List<IExchangeClient>
        {
            _mockBinanceClient.Object,
            _mockBybitClient.Object,
            _mockKucoinClient.Object,
            _mockBitgetClient.Object
        };

            _exchangeService = new ExchangeService(clients);
        }

        [Fact]
        public async Task GetPricesAsync_ShouldReturnValidPrices()
        {
            // Arrange
            _mockBinanceClient.Setup(c => c.GetPriceAsync("BTCUSDT")).ReturnsAsync(50000);
            _mockBybitClient.Setup(c => c.GetPriceAsync("BTCUSDT")).ReturnsAsync(49990);
            _mockKucoinClient.Setup(c => c.GetPriceAsync("BTC-USDT")).ReturnsAsync(50010);
            _mockBitgetClient.Setup(c => c.GetPriceAsync("BTCUSDT")).ReturnsAsync(50005);

            // Act
            var prices = await _exchangeService.GetPricesAsync("BTC-USDT");

            // Assert
            Assert.NotNull(prices);
            Assert.Equal(4, prices.Count);
            Assert.Equal(50000, prices["Binance"]);
            Assert.Equal(49990, prices["Bybit"]);
            Assert.Equal(50010, prices["Kucoin"]);
            Assert.Equal(50005, prices["Bitget"]);
        }

        [Fact]
        public async Task SubscribeToWebSockets_ShouldInvokeSubscriptionForAllExchanges()
        {
            // Arrange
            var callbackInvoked = new Dictionary<string, bool>
            {
                { "Binance", false },
                { "Bybit", false },
                { "Kucoin", false },
                { "Bitget", false }
            };

            _mockBinanceClient.Setup(c => c.SubscribeToWebSockets("BTCUSDT", It.IsAny<Action<string, decimal>>()))
                              .Callback<string, Action<string, decimal>>((_, cb) => { callbackInvoked["Binance"] = true; })
                              .Returns(Task.CompletedTask);

            _mockBybitClient.Setup(c => c.SubscribeToWebSockets("BTCUSDT", It.IsAny<Action<string, decimal>>()))
                            .Callback<string, Action<string, decimal>>((_, cb) => { callbackInvoked["Bybit"] = true; })
                            .Returns(Task.CompletedTask);

            _mockKucoinClient.Setup(c => c.SubscribeToWebSockets("BTC-USDT", It.IsAny<Action<string, decimal>>()))
                             .Callback<string, Action<string, decimal>>((_, cb) => { callbackInvoked["Kucoin"] = true; })
                             .Returns(Task.CompletedTask);

            _mockBitgetClient.Setup(c => c.SubscribeToWebSockets("BTCUSDT", It.IsAny<Action<string, decimal>>()))
                             .Callback<string, Action<string, decimal>>((_, cb) => { callbackInvoked["Bitget"] = true; })
                             .Returns(Task.CompletedTask);

            var tokenSource = new CancellationTokenSource();

            // Act
            await _exchangeService.SubscribeToWebSockets("BTC-USDT", (ex, price) => { }, null, tokenSource.Token);

            // Assert
            Assert.All(callbackInvoked.Values, Assert.True);
        }

        [Fact]
        public async Task UnsubscribeFromWebSockets_ShouldInvokeUnsubscribeForAllExchanges()
        {
            // Arrange
            _mockBinanceClient.Setup(c => c.UnsubscribeFromWebSockets()).Returns(Task.CompletedTask).Verifiable();
            _mockBybitClient.Setup(c => c.UnsubscribeFromWebSockets()).Returns(Task.CompletedTask).Verifiable();
            _mockKucoinClient.Setup(c => c.UnsubscribeFromWebSockets()).Returns(Task.CompletedTask).Verifiable();
            _mockBitgetClient.Setup(c => c.UnsubscribeFromWebSockets()).Returns(Task.CompletedTask).Verifiable();

            // Act
            await _exchangeService.UnsubscribeFromWebSockets();

            // Assert
            _mockBinanceClient.Verify(c => c.UnsubscribeFromWebSockets(), Times.Once);
            _mockBybitClient.Verify(c => c.UnsubscribeFromWebSockets(), Times.Once);
            _mockKucoinClient.Verify(c => c.UnsubscribeFromWebSockets(), Times.Once);
            _mockBitgetClient.Verify(c => c.UnsubscribeFromWebSockets(), Times.Once);
        }
    }
}
