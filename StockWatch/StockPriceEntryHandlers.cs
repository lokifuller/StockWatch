// Example from StockPriceEntryHandlers.cs
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StockPricesApp
{
    public partial class MainWindow : Window
    {
        private async void TickerTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string symbol = TickerTextBox.Text.Trim().ToUpper();
                await LoadStockPricesForSymbolAsync(symbol);
            }
        }

        private async void RSICheckbox_Checked(object sender, RoutedEventArgs e)
        {
            isRSIEnabled = true;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            await LoadStockPricesForSymbolAsync(symbol);
        }

        private async void RSICheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            isRSIEnabled = false;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            await LoadStockPricesForSymbolAsync(symbol);
        }

        private async void SMACheckbox_Checked(object sender, RoutedEventArgs e)
        {
            isSMAEnabled = true;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            await LoadStockPricesForSymbolAsync(symbol);
        }

        private async void SMACheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            isSMAEnabled = false;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            await LoadStockPricesForSymbolAsync(symbol);
        }

        private async void VolumeCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            isVolumeEnabled = true;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            await LoadStockPricesForSymbolAsync(symbol);
        }

        private async void VolumeCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            isVolumeEnabled = false;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            await LoadStockPricesForSymbolAsync(symbol);
        }

        private async void VolumeVsAvgCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            isVolumeVsAvgEnabled = true;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            await LoadStockPricesForSymbolAsync(symbol);
        }

        private async void VolumeVsAvgCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            isVolumeVsAvgEnabled = false;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            await LoadStockPricesForSymbolAsync(symbol);
        }
    }
}
