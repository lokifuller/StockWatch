using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StockPricesApp
{
    public partial class MainWindow : Window
    {
        private bool isDragging = false;
        private Point offset;

        class StockPrice
        {
            public string datetime { get; set; }
            public decimal close { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None; // Remove the window border and title bar
            AllowsTransparency = true; // Allow transparency
            Background = Brushes.Transparent; // Set the background color to transparent
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

                    // Calculate RSI
                    decimal[] closePrices = stockPrices.Select(p => p.close).Reverse().ToArray();
                    decimal[] priceChanges = new decimal[closePrices.Length - 1];
                    for (int i = 0; i < priceChanges.Length; i++)
                    {
                        priceChanges[i] = closePrices[i + 1] - closePrices[i];
                    }

                    decimal[] gains = priceChanges.Where(p => p > 0).ToArray();
                    decimal[] losses = priceChanges.Where(p => p < 0).ToArray();

                    decimal averageGain = gains.Take(10).Sum() / 10;
                    decimal averageLoss = Math.Abs(losses.Take(10).Sum()) / 10;

                    for (int i = 10; i < priceChanges.Length; i++)
                    {
                        decimal currentGain = priceChanges[i] > 0 ? priceChanges[i] : 0;
                        decimal currentLoss = Math.Abs(priceChanges[i] < 0 ? priceChanges[i] : 0);

                        averageGain = (averageGain * 9 + currentGain) / 10;
                        averageLoss = (averageLoss * 9 + currentLoss) / 10;
                    }

                    decimal relativeStrength = averageGain / averageLoss;
                    decimal rsi = 100 - (100 / (1 + relativeStrength));

                    // Display RSI
                    TextBlock rsiTextBlock = new TextBlock
                    {
                        Text = $"RSI for {symbol}: {rsi.ToString("0.00")}",
                        Foreground = Brushes.Purple
                    };
                    StockPricesListBox.Items.Add(rsiTextBlock);

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
    }
}