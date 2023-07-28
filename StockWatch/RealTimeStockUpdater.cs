using System;
using System.Timers;
using System.Windows.Threading;

namespace StockPricesApp
{
    public class RealTimeStockUpdater
    {
        private readonly Timer stockUpdateTimer;
        private const int UpdateInterval = 6000; // 10 minutes
        private readonly MainWindow mainWindow;

        public RealTimeStockUpdater(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            stockUpdateTimer = new Timer(UpdateInterval);
            stockUpdateTimer.Elapsed += StockUpdateTimer_Elapsed;
            stockUpdateTimer.Start();
        }

        private void StockUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            mainWindow.Dispatcher.Invoke(async () =>
            {
                string symbol = mainWindow.TickerTextBox.Text.Trim();
                if (!string.IsNullOrWhiteSpace(symbol))
                {
                    await mainWindow.LoadStockPricesForSymbolAsync(symbol);
                }
            });
        }
    }
}