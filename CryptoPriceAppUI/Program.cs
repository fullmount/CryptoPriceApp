using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Core;
using Core.ExchangeWrappers;
using Core.Services;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Clients;
using Kucoin.Net.Interfaces.Clients;
using Kucoin.Net.Clients;
using Bitget.Net.Interfaces.Clients;
using Bitget.Net.Clients;
using Bybit.Net.Interfaces.Clients;
using Bybit.Net.Clients;


namespace CryptoPriceAppUI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var services = ConfigureServices;
            using var serviceProvider = services.BuildServiceProvider();

            var form = serviceProvider.GetRequiredService<MainForm>();
            Application.Run(form);
        }

        private static IServiceCollection ConfigureServices
        {
            get
            {
                var services = new ServiceCollection();
                string projectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\"));

                var config = new ConfigurationBuilder()
                    .SetBasePath(projectPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

                services.AddSingleton<IConfiguration>(config);
                services.AddSingleton<IExchangeClient, BinanceClientWrapper>();
                services.AddSingleton<IExchangeClient, BitgetClientWrapper>();
                services.AddSingleton<IExchangeClient, KucoinClientWrapper>();
                services.AddSingleton<IExchangeClient, BybitClientWrapper>();
                //REST clients
                services.AddScoped<IBinanceRestClient, BinanceRestClient>();
                services.AddScoped<IKucoinRestClient, KucoinRestClient>();
                services.AddScoped<IBitgetRestClient, BitgetRestClient>();
                services.AddScoped<IBybitRestClient, BybitRestClient>();
                //Web socket clients
                services.AddSingleton<IBinanceSocketClient, BinanceSocketClient>();
                services.AddSingleton<IBybitSocketClient, BybitSocketClient>();
                services.AddSingleton<IKucoinSocketClient, KucoinSocketClient>();
                services.AddSingleton<IBitgetSocketClient, BitgetSocketClient>();

                services.AddSingleton<ExchangeService>();

                services.AddTransient<MainForm>();

                return services;
            }
        }
    }
}