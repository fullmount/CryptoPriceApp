using Core.Services;
using Core;
using Core.ExchangeWrappers;
using Microsoft.Extensions.DependencyInjection;
using Binance.Net.Clients;
using Binance.Net.Interfaces.Clients;
using Bybit.Net.Clients;
using Bybit.Net.Interfaces.Clients;
using Kucoin.Net.Clients;
using Kucoin.Net.Interfaces.Clients;
using Bitget.Net.Interfaces.Clients;
using Bitget.Net.Clients;

namespace CryptoPriceApp.tests.Integration
{
    public class ExchangeServiceFixture : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        public ExchangeService Service { get; }

        public ExchangeServiceFixture()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IBinanceRestClient, BinanceRestClient>();
            services.AddSingleton<IBybitRestClient, BybitRestClient>();
            services.AddSingleton<IKucoinRestClient, KucoinRestClient>();
            services.AddSingleton<IBitgetRestClient, BitgetRestClient>();

            services.AddSingleton<IBinanceSocketClient, BinanceSocketClient>();
            services.AddSingleton<IBybitSocketClient, BybitSocketClient>();
            services.AddSingleton<IBitgetSocketClient, BitgetSocketClient>();
            services.AddSingleton<IKucoinSocketClient, KucoinSocketClient>();

            services.AddSingleton<IExchangeClient, BinanceClientWrapper>();
            services.AddSingleton<IExchangeClient, BybitClientWrapper>();
            services.AddSingleton<IExchangeClient, KucoinClientWrapper>();
            services.AddSingleton<IExchangeClient, BitgetClientWrapper>();

            services.AddSingleton<ExchangeService>();

            _serviceProvider = services.BuildServiceProvider();
            Service = _serviceProvider.GetRequiredService<ExchangeService>();
        }

        public void Dispose()
        {
            Service.UnsubscribeFromWebSockets().Wait();
            _serviceProvider.Dispose();
        }
    }
}
