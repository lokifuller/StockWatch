using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StockPricesApp
{
    public partial class MainWindow : Window
    {
        private bool isDragging = false;
        private Point offset;
        private const string StorageFilePath = "stock.txt"; // File path for storing the stock symbol
        private const string ConfigFilePath = "config.json"; // File path for storing the window position
        private bool isRSIEnabled = false; // Flag to track the RSI checkbox state
        private bool isSMAEnabled = false; // Flag to track the SMA checkbox state
        class StockPrice
        {
            public string datetime { get; set; }
            public decimal close { get; set; }
        }

        class WindowConfig
        {
            public double Left { get; set; }
            public double Top { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None; // Remove the window border and title bar
            AllowsTransparency = true; // Allow transparency
            Background = Brushes.Transparent; // Set the background color to transparent

            // Load the last stock symbol from storage, if available
            if (File.Exists(StorageFilePath))
            {
                string symbol = File.ReadAllText(StorageFilePath);
                TickerTextBox.Text = symbol;
                _ = LoadStockPricesForSymbolAsync(symbol);
            }

            // Initialize the RSI checkbox state
            isRSIEnabled = false;

            // Load the window position from configuration
            if (File.Exists(ConfigFilePath))
            {
                string configJson = File.ReadAllText(ConfigFilePath);
                WindowConfig config = JsonConvert.DeserializeObject<WindowConfig>(configJson);
                Left = config.Left;
                Top = config.Top;
            }
        }

        private async Task LoadStockPricesForSymbolAsync(string symbol)
        {
            var client = new RestClient("https://twelve-data1.p.rapidapi.com/stocks?exchange=NASDAQ&format=json");
            var request = new RestRequest("time_series", Method.Get);
            request.AddParameter("symbol", symbol);
            request.AddParameter("interval", "1day");
            request.AddParameter("outputsize", "11");
            request.AddHeader("X-RapidAPI-Key", "02d91c48ccmsha5de74d7d78c89dp197a54jsn3531436e0019");
            request.AddHeader("X-RapidAPI-Host", "twelve-data1.p.rapidapi.com");

            var response = await client.ExecuteAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                dynamic jsonData = JsonConvert.DeserializeObject(response.Content);
                var prices = jsonData?.values;
                if (prices != null)
                {
                    List<StockPrice> stockPrices = JsonConvert.DeserializeObject<List<StockPrice>>(prices.ToString());

                    StockPricesListBox.Items.Clear(); // Clear the previous stock prices

                    TextBlock paddingText = new TextBlock
                    {
                        Text = "Padding text block",
                        Foreground = Brushes.Black
                    };

                    if (isRSIEnabled)
                    {
                        // Calculate RSI
                        decimal[] closePrices = stockPrices.Select(p => p.close).Reverse().ToArray();
                        decimal[] priceChanges = new decimal[closePrices.Length - 1];
                        for (int i = 0; i < priceChanges.Length; i++)
                        {
                            priceChanges[i] = closePrices[i + 1] - closePrices[i];
                        }

                        decimal[] gains = priceChanges.Where(p => p > 0).ToArray();
                        decimal[] losses = priceChanges.Where(p => p < 0).ToArray();

                        decimal averageGain = gains.Take(10).Average();
                        decimal averageLoss = Math.Abs(losses.Take(10).Average());

                        decimal smoothingFactor = 2m / (10m + 1m); // EMA smoothing factor

                        for (int i = 10; i < priceChanges.Length; i++)
                        {
                            decimal currentGain = priceChanges[i] > 0 ? priceChanges[i] : 0;
                            decimal currentLoss = Math.Abs(priceChanges[i] < 0 ? priceChanges[i] : 0);

                            averageGain = (currentGain * smoothingFactor) + (averageGain * (1m - smoothingFactor));
                            averageLoss = (currentLoss * smoothingFactor) + (averageLoss * (1m - smoothingFactor));
                        }

                        decimal relativeStrength = averageGain / averageLoss;
                        decimal rsi = 100m - (100m / (1m + relativeStrength));

                        // Display RSI
                        TextBlock rsiTextBlock = new TextBlock
                        {
                            Text = $"RSI for {symbol}: {rsi.ToString("0.00")}",
                            Foreground = Brushes.Purple
                        };
                        StockPricesListBox.Items.Add(rsiTextBlock);
                    }

                    if (isSMAEnabled)
                    {
                        decimal[] closePrices = stockPrices.Select(p => p.close).Reverse().ToArray();

                        decimal sum = 0;
                        int periodLength = Math.Min(closePrices.Length, 9); // Adjust period length based on available prices

                        for (int i = 0; i < periodLength; i++)
                        {
                            sum += closePrices[i];
                        }

                        decimal sma = sum / periodLength;

                        // Display SMA
                        TextBlock smaTextBlock = new TextBlock
                        {
                            Text = $"SMA for {symbol}: {sma.ToString("0.00")}",
                            Foreground = Brushes.Blue
                        };
                        StockPricesListBox.Items.Add(smaTextBlock);
                    }

                    for (int i = 0; i < stockPrices.Count - 1; i += 1)
                    {
                        StockPrice price = stockPrices[i];
                        StockPrice nextPrice = stockPrices[i + 1];

                        DateTime dateTime = DateTime.ParseExact(price.datetime, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                        Brush priceColor = nextPrice.close < price.close ? Brushes.Green : Brushes.Red;

                        TextBlock textBlock = new TextBlock
                        {
                            Text = $"Stock price for {symbol} on {price.datetime}: {price.close}",
                            Foreground = priceColor
                        };

                        StockPricesListBox.Items.Add(textBlock);
                    }
                }
            }

            // Save the current stock symbol to storage
            File.WriteAllText(StorageFilePath, symbol);
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Set isDragging to true and capture the mouse
            isDragging = true;
            offset = e.GetPosition(this);
            ((UIElement)sender).CaptureMouse();
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Set isDragging to false and release the mouse capture
            isDragging = false;
            ((UIElement)sender).ReleaseMouseCapture();

            // Save the window position to configuration
            WindowConfig config = new WindowConfig { Left = Left, Top = Top };
            string configJson = JsonConvert.SerializeObject(config);
            File.WriteAllText(ConfigFilePath, configJson);
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                // Calculate the new position of the window based on the mouse movement
                Point newPoint = e.GetPosition(null);
                double deltaX = newPoint.X - offset.X;
                double deltaY = newPoint.Y - offset.Y;
                Left += deltaX;
                Top += deltaY;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void TickerTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string symbol = TickerTextBox.Text.Trim().ToUpper();
                await LoadStockPricesForSymbolAsync(symbol);
            }
        }

        private async void SMACheckbox_Checked(object sender, RoutedEventArgs e)
        {
            isSMAEnabled = true; // Set the flag to indicate that SMA is enabled
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            await LoadStockPricesForSymbolAsync(symbol);
        }

        private async void SMACheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            isSMAEnabled = false;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            await LoadStockPricesForSymbolAsync(symbol);
        }
        private async void RSICheckbox_Checked(object sender, RoutedEventArgs e)
        {
            isRSIEnabled = true; // Set the flag to indicate that RSI is enabled
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            await LoadStockPricesForSymbolAsync(symbol);
        }

        private async void RSICheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            isRSIEnabled = false;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            await LoadStockPricesForSymbolAsync(symbol);
        }

        private void ExtensionsToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ExtensionsPopup.IsOpen = !ExtensionsPopup.IsOpen;
        }

        private void ExtensionsPopup_Opened(object sender, EventArgs e)
        {
            ExtensionsToggleButton.IsEnabled = false;
        }

        private void ExtensionsPopup_Closed(object sender, EventArgs e)
        {
            ExtensionsToggleButton.IsEnabled = true;
        }
    }
}