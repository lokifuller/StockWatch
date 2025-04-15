using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StockPricesApp
{
    public partial class MainWindow : Window
    {
        public void CalculateAndDisplayVolume(string symbol, System.Collections.Generic.List<GatherStockPrice> stockPrices)
        {
            if (stockPrices == null || stockPrices.Count < 1)
            {
                MessageBox.Show("No data available for volume calculation");
                return;
            }

            long todaysVolume = stockPrices.First().volume;

            TextBlock volumeTextBlock = new TextBlock
            {
                Text = $"Volume for {symbol}: {todaysVolume}",
                Foreground = Brushes.LightBlue
            };

            MenuItem copyVolumeMenuItem = new MenuItem
            {
                Header = "Copy Volume",
                Tag = todaysVolume
            };
            copyVolumeMenuItem.Click += (sender, e) =>
            {
                Clipboard.SetText(todaysVolume.ToString());
            };

            volumeTextBlock.ContextMenu = new ContextMenu();
            volumeTextBlock.ContextMenu.Items.Add(copyVolumeMenuItem);

            StockPricesListBox.Items.Add(volumeTextBlock);
        }
    }
}