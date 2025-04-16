using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StockPricesApp
{
    public partial class MainWindow : Window
    {
        private void CalculateAndDisplaySMA(string symbol, List<GatherStockPrice> stockPrices)
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

            if (stockPrices.First().close >= sma)
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
                Foreground = Brushes.MediumPurple
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

            MenuItem copySMAMenuItem = new MenuItem
            {
                Header = "Copy SMA",
                Tag = sma
            };
            copySMAMenuItem.Click += CopySMAMenuItem_Click;
            smaTextBlock.ContextMenu = new ContextMenu();
            smaTextBlock.ContextMenu.Items.Add(copySMAMenuItem);
        }

        private void CopySMAMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is decimal smaValue)
            {
                Clipboard.SetText(smaValue.ToString("0.00"));
            }
        }
    }
}