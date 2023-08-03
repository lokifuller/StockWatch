//
// StockPricesApp
//
// This code implements a stock prices application in C#. The application allows users to enter a stock symbol and retrieve and display the corresponding stock prices. It also provides optional functionality to calculate and display the Relative Strength Index (RSI) and Simple Moving Average (SMA) for the stock.
//
// The code utilizes the Newtonsoft.Json library for JSON serialization and RestSharp library for making API requests to retrieve stock prices. The application uses the Twelve Data API to fetch stock prices for the specified symbol.
//
// The main components of the code include:
// - MainWindow class: Represents the main window of the application.
// - StockPrice class: Represents a single stock price entry, with properties for datetime and close price.
// - WindowConfig class: Represents the configuration of the application window, including position and checkbox states.
//
// The code includes various functions for initializing the application window, loading stock symbol and window configuration, retrieving and displaying stock prices, calculating and displaying RSI and SMA, and handling user interactions such as dragging the window, minimizing, closing, and toggling checkboxes.
//
// Function overview:
// - LoadLastStockSymbol(): Loads the last entered stock symbol from a storage file, sets the text in the TickerTextBox, and asynchronously loads stock prices for the symbol.
// - LoadWindowPositionAndCheckboxStates(): Loads the previous window position and checkbox states from a configuration file and sets the corresponding properties and UI states.
// - LoadStockPricesForSymbolAsync(string symbol): Asynchronously retrieves stock prices for the specified symbol using the Twelve Data API. Parses the response JSON and extracts the necessary data. Calls other functions to calculate and display RSI and SMA if enabled. Finally, calls a function to display the stock prices in the UI.
// - CalculateAndDisplayRSI(string symbol, List<StockPrice> stockPrices): Calculates the Relative Strength Index (RSI) for the given stock symbol and stock prices using an algorithm that involves calculating gains, losses, average gain, average loss, and RSI values. Calls a function to display the RSI value in the UI.
// - CalculateAndDisplaySMA(string symbol, List<StockPrice> stockPrices): Calculates the Simple Moving Average (SMA) for the given stock symbol and stock prices using a simple algorithm. Determines whether the current price is above or below the SMA and calls a function to display the SMA value and an associated image in the UI.
// - DisplayStockPrices(string symbol, List<StockPrice> stockPrices): Displays the stock prices in the UI. Iterates over the stock prices, parses the datetime, determines the price color based on the next price, and creates a text block for each stock price entry.
// - SaveCurrentStockSymbol(string symbol): Saves the current stock symbol to a storage file.
// - Border_MouseLeftButtonDown, Border_MouseLeftButtonUp, Border_MouseMove: Event handlers for dragging the window.
// - MinimizeButton_Click, CloseButton_Click: Event handlers for minimizing and closing the window.
// - TickerTextBox_KeyUp: Event handler for retrieving stock prices when the user presses Enter after entering a stock symbol.
// - RSICheckbox_Checked, RSICheckbox_Unchecked: Event handlers for enabling/disabling RSI calculation and updating the UI accordingly.
// - SMACheckbox_Checked, SMACheckbox_Unchecked: Event handlers for enabling/disabling SMA calculation and updating the UI accordingly.
// - SaveCheckboxStatesToConfig: Saves the current window position and checkbox states to a configuration file.
// - ExtensionsToggleButton_Click, ExtensionsPopup_Opened, ExtensionsPopup_Closed: Event handlers for toggling and managing a popup extension menu.

using System.Threading.Tasks;
using System.Windows;

namespace StockPricesApp
{
    public partial class MainWindow : Window
    {
        private const string StorageFilePath = "stock.txt";
        private const string ConfigFilePath = "config.json";
        private bool isRSIEnabled = false;
        private bool isSMAEnabled = false;

        public MainWindow()
        {
            InitializeComponent();
            ConfigureWindowStyle();
            LoadLastStockSymbol();
            LoadWindowPositionAndCheckboxStates();
        }

        private void ConfigureWindowStyle()
        {
            WindowStyle = WindowStyle.None;
        }
    }
}