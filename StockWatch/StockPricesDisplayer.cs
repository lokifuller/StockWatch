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

                textBlock.ContextMenu = CreateContextMenu(symbol, price);

                StockPricesListBox.Items.Add(textBlock);
            }
        }

        private ContextMenu CreateContextMenu(string symbol, StockPrice stockPrice)
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