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
        private void CalculateAndDisplayRSI(string symbol, List<GatherStockPrice> stockPrices)
        {
            int periodLength = Math.Min(stockPrices.Count, 10);
            decimal[] closePrices = stockPrices.Select(p => p.close).Reverse().ToArray();

            double[] positiveChanges = new double[closePrices.Length];
            double[] negativeChanges = new double[closePrices.Length];
            double[] averageGain = new double[closePrices.Length];
            double[] averageLoss = new double[closePrices.Length];
            double[] rsi = new double[closePrices.Length];

            for (int i = 1; i < closePrices.Length; i++)
            {
                decimal priceDiff = closePrices[i] - closePrices[i - 1];

                if (priceDiff > 0)
                    positiveChanges[i] = (double)priceDiff;
                else
                    negativeChanges[i] = Math.Abs((double)priceDiff);
            }

            for (int i = periodLength; i < closePrices.Length; i++)
            {
                double gainSum = 0.0;
                double lossSum = 0.0;

                for (int x = i - periodLength + 1; x <= i; x++)
                {
                    gainSum += positiveChanges[x];
                    lossSum += negativeChanges[x];
                }

                averageGain[i] = gainSum / periodLength;
                averageLoss[i] = lossSum / periodLength;

                double rs = averageGain[i] / (averageLoss[i] == 0 ? 1 : averageLoss[i]);
                rsi[i] = 100 - (100 / (1 + rs));
            }

            for (int i = periodLength; i < closePrices.Length; i++)
            {
                TextBlock rsiTextBlock = new TextBlock
                {
                    Text = $"RSI for {symbol}: {rsi[i].ToString("0.00")}",
                    Foreground = Brushes.DarkOrange
                };

                StockPricesListBox.Items.Add(rsiTextBlock);

                MenuItem copyRSIMenuItem = new MenuItem
                {
                    Header = "Copy RSI",
                    Tag = rsi[i]
                };
                copyRSIMenuItem.Click += CopyRSIMenuItem_Click;
                rsiTextBlock.ContextMenu = new ContextMenu();
                rsiTextBlock.ContextMenu.Items.Add(copyRSIMenuItem);
            }
        }

        private void CopyRSIMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is double rsiValue)
            {
                Clipboard.SetText(rsiValue.ToString("0.00"));
            }
        }
    }
}