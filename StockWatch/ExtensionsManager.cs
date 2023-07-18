using System;
using System.Windows;

namespace StockPricesApp
{
    public partial class MainWindow : Window
    {
        private void ExtensionsToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ExtensionsPopup.IsOpen = !ExtensionsPopup.IsOpen;
        }

        private void ExtensionsPopup_Opened(object sender, EventArgs e)
        {
            ExtensionsToggleButton.IsEnabled = false;
        }

        private void ExtensionsPopup_Closed(object sender, EventArgs e)
        {
            ExtensionsToggleButton.IsEnabled = true;
        }
    }
}