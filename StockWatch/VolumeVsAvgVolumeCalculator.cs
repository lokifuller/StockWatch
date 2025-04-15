using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StockPricesApp
{
    public partial class MainWindow : Window
    {
        public void CalculateAndDisplayVolumeComparison(string symbol, List<GatherStockPrice> stockPrices)
        {
            // Check for data validity
            if (stockPrices == null || stockPrices.Count < 11)
            {
                MessageBox.Show("Not enough data to calculate Volume vs Average Volume");
                return;
            }

            long todaysVolume = stockPrices.First().volume;

            long averageVolume = (long)stockPrices.Skip(1).Take(10).Average(p => p.volume);

            long diff = todaysVolume - averageVolume;
            double diffPercent = averageVolume != 0 ? ((double)diff / averageVolume) * 100 : 0;

            StackPanel volumePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            TextBlock volumeTextBlock = new TextBlock
            {
                Text = $"Volume for {symbol}: {todaysVolume} Avg: {averageVolume} " +
                       $"({(diff >= 0 ? "+" : "")}{diffPercent:F2}%)",
                Foreground = Brushes.LightBlue
            };

            volumePanel.Children.Add(volumeTextBlock);

            // Add volume panel to the ListBox
            StockPricesListBox.Items.Add(volumePanel);

            MenuItem copyVolumeMenuItem = new MenuItem
            {
                Header = "Copy Volume Comparison",
                Tag = volumeTextBlock.Text
            };
            copyVolumeMenuItem.Click += CopyVolumeMenuItem_Click;

            volumeTextBlock.ContextMenu = new ContextMenu();
            volumeTextBlock.ContextMenu.Items.Add(copyVolumeMenuItem);
        }

        private void CopyVolumeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is string volumeText)
            {
                Clipboard.SetText(volumeText);
            }
        }
    }
}