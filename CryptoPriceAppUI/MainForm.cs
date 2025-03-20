using Core.Services;
using Microsoft.Extensions.Configuration;

namespace CryptoPriceAppUI
{
    public partial class MainForm : Form
    {
        private readonly ExchangeService _exchangeService;
        private readonly List<string> _selectedPairs;
        private readonly Dictionary<string, decimal> _latestPrices = [];
        private readonly Dictionary<string, DateTime> _lastUpdateTime = [];
        private bool isSubscribed = false;
        private CancellationTokenSource _cts;

        public MainForm(ExchangeService exchangeService, IConfiguration config)
        {
            InitializeComponent();
            _exchangeService = exchangeService;
            _selectedPairs = config.GetSection("TradingPairs").Get<List<string>>() ?? [];

            comboBoxPair.Items.AddRange([.. _selectedPairs]);
            comboBoxPair.SelectedIndex = 0;
            _cts = new CancellationTokenSource();
        }

        private async void BtnGetPrices_Click(object sender, EventArgs e)
        {
            string selectedPair = comboBoxPair.SelectedItem?.ToString() ?? string.Empty;
            var prices = await _exchangeService.GetPricesAsync(selectedPair);

            listBoxPrices.Items.Clear();
            foreach (var price in prices)
            {
                listBoxPrices.Items.Add($"{price.Key}: {price.Value}");
            }
        }

        private async void BtnSubscribe_Click(object sender, EventArgs e)
        {
            await Subscribe();
        }

        private async void ComboBoxPair_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSubscribed)
            {
                await Subscribe();
            }
        }

        private async void BtnUnsubscribe_Click(object sender, EventArgs e)
        {
            if (!isSubscribed)
            {
                return;
            }    
            _cts.Cancel();
            isSubscribed = false;
            await _exchangeService.UnsubscribeFromWebSockets();
        }

        private void UpdateUI()
        {
            listBoxPrices.Items.Clear();
            foreach (var kvp in _latestPrices.OrderBy(x => x.Key))
            {
                listBoxPrices.Items.Add($"{kvp.Key}: {kvp.Value}");
            }
        }

        private async Task Subscribe()
        {
            try
            {
                string selectedPair = comboBoxPair.SelectedItem?.ToString() ?? string.Empty;
                await _exchangeService.SubscribeToWebSockets(
                    selectedPair, 
                    (exchange, price) =>
                    {
                        if (!_lastUpdateTime.TryGetValue(exchange, out DateTime value) ||
                            (DateTime.UtcNow - value).TotalSeconds >= 5)
                        {
                            lock (_latestPrices)
                            {
                                _latestPrices[exchange] = price;
                                value = DateTime.UtcNow;
                                _lastUpdateTime[exchange] = value;
                            }
                            Invoke(new Action(UpdateUI));
                        }
                    },
                    (errorMessage) =>
                    {
                        Invoke(new Action(() =>
                        {
                            lblStatus.Text = errorMessage;
                        }));
                    },
                    _cts.Token
                );
                isSubscribed = true;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cts.Cancel();
        }
    }
}
