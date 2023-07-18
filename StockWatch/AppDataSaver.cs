using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace StockPricesApp
{
    public partial class MainWindow : Window
    {
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
        private void SaveCurrentStockSymbol(string symbol)
        {
            File.WriteAllText(StorageFilePath, symbol);
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
    }
}