using Core;
using Core.Services;
using Moq;
namespace CryptoPriceApp.tests.Unit
{
    public class ExchangeServiceTests
    {
        private readonly Mock<IExchangeClient> _mockBinanceClient = new();
        private readonly Mock<IExchangeClient> _mockBybitClient = new();
        private readonly Mock<IExchangeClient> _mockKucoinClient = new();
        private readonly Mock<IExchangeClient> _mockBitgetClient = new();
        private readonly ExchangeService _exchangeService;

        public ExchangeServiceTests()
        {
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
            _mockBinanceClient.Setup(c => c.GetPriceAsync("BTCUSDT")).ReturnsAsync(50000m);
            _mockBybitClient.Setup(c => c.GetPriceAsync("BTCUSDT")).ReturnsAsync(49990m);
            _mockKucoinClient.Setup(c => c.GetPriceAsync("BTC-USDT")).ReturnsAsync(50010m);
            _mockBitgetClient.Setup(c => c.GetPriceAsync("BTCUSDT")).ReturnsAsync(50005m);

            CancellationTokenSource cts = new();

            // Act
            var prices = await _exchangeService.GetPricesAsync("BTC-USDT", cts.Token);

            // Assert
            Assert.Equal(4, prices.Count);
            Assert.Equal(50000m, prices["Binance"]);
            Assert.Equal(49990m, prices["Bybit"]);
            Assert.Equal(50010m, prices["Kucoin"]);
            Assert.Equal(50005m, prices["Bitget"]);
        }

        [Fact]
        public async Task SubscribeToWebSockets_ShouldInvokeSubscriptionForAllExchanges()
        {
            // Arrange
            var priceUpdates = new Dictionary<string, decimal>();
            Action<string, decimal> callback = (exchange, price) => priceUpdates[exchange] = price;
            Action<string> onError = _ => { };

            _mockBinanceClient.Setup(c => c.SubscribeToWebSockets("BTCUSDT", It.IsAny<Action<string, decimal>>()))
                              .Returns(Task.CompletedTask)
                              .Verifiable();

            _mockBybitClient.Setup(c => c.SubscribeToWebSockets("BTCUSDT", It.IsAny<Action<string, decimal>>()))
                            .Returns(Task.CompletedTask)
                            .Verifiable();

            _mockKucoinClient.Setup(c => c.SubscribeToWebSockets("BTC-USDT", It.IsAny<Action<string, decimal>>()))
                             .Returns(Task.CompletedTask)
                             .Verifiable();

            _mockBitgetClient.Setup(c => c.SubscribeToWebSockets("BTCUSDT", It.IsAny<Action<string, decimal>>()))
                             .Returns(Task.CompletedTask)
                             .Verifiable();

            CancellationTokenSource cts = new();

            // Act
            await _exchangeService.SubscribeToWebSockets("BTC-USDT", callback, onError, cts.Token);

            // Assert
            _mockBinanceClient.Verify(c => c.SubscribeToWebSockets("BTCUSDT", It.IsAny<Action<string, decimal>>()), Times.Once);
            _mockBybitClient.Verify(c => c.SubscribeToWebSockets("BTCUSDT", It.IsAny<Action<string, decimal>>()), Times.Once);
            _mockKucoinClient.Verify(c => c.SubscribeToWebSockets("BTC-USDT", It.IsAny<Action<string, decimal>>()), Times.Once);
            _mockBitgetClient.Verify(c => c.SubscribeToWebSockets("BTCUSDT", It.IsAny<Action<string, decimal>>()), Times.Once);
        }

        [Fact]
        public async Task UnsubscribeFromWebSockets_ShouldInvokeUnsubscriptionForAllExchanges()
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
