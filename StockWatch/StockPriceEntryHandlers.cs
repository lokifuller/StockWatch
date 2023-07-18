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

        private void RSICheckbox_Checked(object sender, RoutedEventArgs e)
        {
            isRSIEnabled = true;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            _ = LoadStockPricesForSymbolAsync(symbol);
        }

        private void RSICheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            isRSIEnabled = false;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            _ = LoadStockPricesForSymbolAsync(symbol);
        }

        private void SMACheckbox_Checked(object sender, RoutedEventArgs e)
        {
            isSMAEnabled = true;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            _ = LoadStockPricesForSymbolAsync(symbol);
        }

        private void SMACheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            isSMAEnabled = false;
            string symbol = TickerTextBox.Text.Trim().ToUpper();
            SaveCheckboxStatesToConfig();
            _ = LoadStockPricesForSymbolAsync(symbol);
        }
    }
}