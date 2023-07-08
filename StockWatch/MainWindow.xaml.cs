//
// StockPricesApp
//
// This code implements a stock prices application in C#. The application allows users to enter a stock symbol and retrieve and display the corresponding stock prices. It also provides optional functionality to calculate and display the Relative Strength Index (RSI) and Simple Moving Average (SMA) for the stock.
//
// The code utilizes the Newtonsoft.Json library for JSON serialization and RestSharp library for making API requests to retrieve stock prices. The application uses the Twelve Data API to fetch stock prices for the specified symbol.
//
// The main components of the code include:
// - MainWindow class: Represents the main window of the application.
// - StockPrice class: Represents a single stock price entry, with properties for datetime and close price.
// - WindowConfig class: Represents the configuration of the application window, including position and checkbox states.
//
// The code includes various functions for initializing the application window, loading stock symbol and window configuration, retrieving and displaying stock prices, calculating and displaying RSI and SMA, and handling user interactions such as dragging the window, minimizing, closing, and toggling checkboxes.
//
// Function overview:
// - LoadLastStockSymbol(): Loads the last entered stock symbol from a storage file, sets the text in the TickerTextBox, and asynchronously loads stock prices for the symbol.
// - LoadWindowPositionAndCheckboxStates(): Loads the previous window position and checkbox states from a configuration file and sets the corresponding properties and UI states.
// - LoadStockPricesForSymbolAsync(string symbol): Asynchronously retrieves stock prices for the specified symbol using the Twelve Data API. Parses the response JSON and extracts the necessary data. Calls other functions to calculate and display RSI and SMA if enabled. Finally, calls a function to display the stock prices in the UI.
// - CalculateAndDisplayRSI(string symbol, List<StockPrice> stockPrices): Calculates the Relative Strength Index (RSI) for the given stock symbol and stock prices using an algorithm that involves calculating gains, losses, average gain, average loss, and RSI values. Calls a function to display the RSI value in the UI.
// - CalculateAndDisplaySMA(string symbol, List<StockPrice> stockPrices): Calculates the Simple Moving Average (SMA) for the given stock symbol and stock prices using a simple algorithm. Determines whether the current price is above or below the SMA and calls a function to display the SMA value and an associated image in the UI.
// - DisplayStockPrices(string symbol, List<StockPrice> stockPrices): Displays the stock prices in the UI. Iterates over the stock prices, parses the datetime, determines the price color based on the next price, and creates a text block for each stock price entry.
// - SaveCurrentStockSymbol(string symbol): Saves the current stock symbol to a storage file.
// - Border_MouseLeftButtonDown, Border_MouseLeftButtonUp, Border_MouseMove: Event handlers for dragging the window.
// - MinimizeButton_Click, CloseButton_Click: Event handlers for minimizing and closing the window.
// - TickerTextBox_KeyUp: Event handler for retrieving stock prices when the user presses Enter after entering a stock symbol.
// - RSICheckbox_Checked, RSICheckbox_Unchecked: Event handlers for enabling/disabling RSI calculation and updating the UI accordingly.
// - SMACheckbox_Checked, SMACheckbox_Unchecked: Event handlers for enabling/disabling SMA calculation and updating the UI accordingly.
// - SaveCheckboxStatesToConfig: Saves the current window position and checkbox states to a configuration file.
// - ExtensionsToggleButton_Click, ExtensionsPopup_Opened, ExtensionsPopup_Closed: Event handlers for toggling and managing a popup extension menu.

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
        private const string StorageFilePath = "stock.txt";
        private const string ConfigFilePath = "config.json";
        private bool isRSIEnabled = false;
        private bool isSMAEnabled = false;

        class StockPrice
        {
            public string datetime { get; set; }
            public decimal close { get; set; }
        }

        class WindowConfig
        {
            public double Left { get; set; }
            public double Top { get; set; }
            public bool IsRSIEnabled { get; set; } 
            public bool IsSMAEnabled { get; set; } 
        }

        public MainWindow()
        {
            InitializeComponent();
            ConfigureWindowStyle();
            LoadLastStockSymbol();
            LoadWindowPositionAndCheckboxStates();
        }

        private void ConfigureWindowStyle()
        {
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = Brushes.Transparent;
        }

        private void LoadLastStockSymbol()
        {
            if (File.Exists(StorageFilePath))
            {
                string symbol = File.ReadAllText(StorageFilePath);
                TickerTextBox.Text = symbol;
                _ = LoadStockPricesForSymbolAsync(symbol);
            }
        }

        private void LoadWindowPositionAndCheckboxStates()
        {
            if (File.Exists(ConfigFilePath))
            {
                string configJson = File.ReadAllText(ConfigFilePath);
                WindowConfig config = JsonConvert.DeserializeObject<WindowConfig>(configJson);
                Left = config.Left;
                Top = config.Top;
                isRSIEnabled = config.IsRSIEnabled;
                isSMAEnabled = config.IsSMAEnabled;
                RSICheckbox.IsChecked = isRSIEnabled;
                SMACheckbox.IsChecked = isSMAEnabled;
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

                    StockPricesListBox.Items.Clear();

                    TextBlock paddingText = new TextBlock
                    {
                        Text = "Padding text block",
                        Foreground = Brushes.Black
                    };

                    if (isRSIEnabled)
                    {
                        CalculateAndDisplayRSI(symbol, stockPrices);
                    }

                    if (isSMAEnabled)
                    {
                        CalculateAndDisplaySMA(symbol, stockPrices);
                    }

                    DisplayStockPrices(symbol, stockPrices);
                }
            }

            SaveCurrentStockSymbol(symbol);
        }

        private void CalculateAndDisplayRSI(string symbol, List<StockPrice> stockPrices)
        {
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

            decimal smoothingFactor = 2m / (10m + 1m);

            for (int i = 10; i < priceChanges.Length; i++)
            {
                decimal currentGain = priceChanges[i] > 0 ? priceChanges[i] : 0;
                decimal currentLoss = Math.Abs(priceChanges[i] < 0 ? priceChanges[i] : 0);

                averageGain = (currentGain * smoothingFactor) + (averageGain * (1m - smoothingFactor));
                averageLoss = (currentLoss * smoothingFactor) + (averageLoss * (1m - smoothingFactor));
            }

            decimal relativeStrength = averageGain / averageLoss;
            decimal rsi = 100m - (100m / (1m + relativeStrength));

            TextBlock rsiTextBlock = new TextBlock
            {
                Text = $"RSI for {symbol}: {rsi.ToString("0.00")}",
                Foreground = Brushes.Purple
            };

            StockPricesListBox.Items.Add(rsiTextBlock);
        }

        private void CalculateAndDisplaySMA(string symbol, List<StockPrice> stockPrices)
        {
            decimal[] closePrices = stockPrices.Select(p => p.close).Reverse().ToArray();
            decimal sum = 0;
            int periodLength = Math.Min(closePrices.Length, 9);

            for (int i = 0; i < periodLength; i++)
            {
                sum += closePrices[i];
            }

            decimal sma = sum / periodLength;

            string imageName = "";

            if (stockPrices.Last().close <= sma)
            {
                imageName = "Above SMA/Above SMA.png";
            }
            else
            {
                imageName = "Below SMA/Below SMA.png";
            }

            StackPanel smaPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            TextBlock smaTextBlock = new TextBlock
            {
                Text = $"SMA for {symbol}: {sma.ToString("0.00")}",
                Foreground = Brushes.Blue
            };

            smaPanel.Children.Add(smaTextBlock);

            Image image = new Image
            {
                Source = new BitmapImage(new Uri($"Assets/{imageName}", UriKind.Relative)),
                Width = 16,
                Height = 16,
                Margin = new Thickness(5, 0, 0, 0)
            };

            smaPanel.Children.Add(image);

            StockPricesListBox.Items.Add(smaPanel);
        }

        private void DisplayStockPrices(string symbol, List<StockPrice> stockPrices)
        {
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

        private void SaveCurrentStockSymbol(string symbol)
        {
            File.WriteAllText(StorageFilePath, symbol);
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            offset = e.GetPosition(this);
            ((UIElement)sender).CaptureMouse();
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            ((UIElement)sender).ReleaseMouseCapture();
            SaveCheckboxStatesToConfig();
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
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

        private void RSICheckbox_Checked(object sender, RoutedEventArgs e)
        {
            isRSIEnabled = true;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            _ = LoadStockPricesForSymbolAsync(symbol);
        }

        private void RSICheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            isRSIEnabled = false;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            _ = LoadStockPricesForSymbolAsync(symbol);
        }

        private void SMACheckbox_Checked(object sender, RoutedEventArgs e)
        {
            isSMAEnabled = true;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            _ = LoadStockPricesForSymbolAsync(symbol);
        }

        private void SMACheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            isSMAEnabled = false;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            _ = LoadStockPricesForSymbolAsync(symbol);
        }

        private void SaveCheckboxStatesToConfig()
        {
            WindowConfig config = new WindowConfig
            {
                Left = Left,
                Top = Top,
                IsRSIEnabled = isRSIEnabled,
                IsSMAEnabled = isSMAEnabled
            };

            string configJson = JsonConvert.SerializeObject(config);
            File.WriteAllText(ConfigFilePath, configJson);
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