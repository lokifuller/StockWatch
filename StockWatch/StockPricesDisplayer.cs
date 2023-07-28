using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StockPricesApp
{
    public partial class MainWindow : Window
    {
        private void DisplayStockPrices(string symbol, List<GatherStockPrice> stockPrices)
        {
            for (int i = 0; i < stockPrices.Count - 1; i += 1)
            {
                GatherStockPrice price = stockPrices[i];
                GatherStockPrice nextPrice = stockPrices[i + 1];

                DateTime dateTime = DateTime.ParseExact(price.datetime, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                decimal priceChange = nextPrice.close - price.close;
                double percentageChange = (double)(priceChange / price.close) * 100;

                string changeSymbol = priceChange > 0 ? "-" : "+";

                Brush priceColor = priceChange > 0 ? Brushes.Red : Brushes.Green;

                TextBlock textBlock = new TextBlock
                {
                    Text = $"Stock price for {symbol} on {price.datetime}: {price.close} ({changeSymbol}{Math.Abs(percentageChange):F2}%)",
                    Foreground = priceColor
                };

                textBlock.ContextMenu = CreateContextMenu(symbol, price);

                StockPricesListBox.Items.Add(textBlock);
            }
        }

        private ContextMenu CreateContextMenu(string symbol, GatherStockPrice stockPrice)
        {
            ContextMenu contextMenu = new ContextMenu();

            MenuItem copyPriceMenuItem = new MenuItem
            {
                Header = "Copy Stock Price",
                Tag = stockPrice.close
            };
            copyPriceMenuItem.Click += CopyPriceMenuItem_Click;
            contextMenu.Items.Add(copyPriceMenuItem);


            return contextMenu;
        }

        private void CopyPriceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is decimal stockPrice)
            {
                Clipboard.SetText(stockPrice.ToString("0.00"));
            }
        }
    }
}