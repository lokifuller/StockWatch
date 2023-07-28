using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StockPricesApp
{
    public partial class MainWindow : Window
    {
        public async Task LoadStockPricesForSymbolAsync(string symbol)
        {
            var client = new RestClient("https://twelve-data1.p.rapidapi.com/stocks?exchange=NASDAQ&format=json");
            var request = new RestRequest("time_series", Method.Get);
            request.AddParameter("symbol", symbol);
            request.AddParameter("interval", "1day");
            request.AddParameter("outputsize", "11");
            request.AddHeader("X-RapidAPI-Key", "02d91c48ccmsha5de74d7d78c89dp197a54jsn3531436e0019");
            request.AddHeader("X-RapidAPI-Host", "twelve-data1.p.rapidapi.com");

            var response = await client.ExecuteAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                dynamic jsonData = JsonConvert.DeserializeObject(response.Content);
                var prices = jsonData?.values;
                if (prices != null)
                {
                    List<GatherStockPrice> stockPrices = JsonConvert.DeserializeObject<List<GatherStockPrice>>(prices.ToString());

                    StockPricesListBox.Items.Clear();

                    TextBlock paddingText = new TextBlock
                    {
                        Text = "Padding text block",
                        Foreground = Brushes.Black
                    };

                    if (isRSIEnabled)
                    {
                        CalculateAndDisplayRSI(symbol, stockPrices);
                    }

                    if (isSMAEnabled)
                    {
                        CalculateAndDisplaySMA(symbol, stockPrices);
                    }

                    DisplayStockPrices(symbol, stockPrices);
                }
            }

            SaveCurrentStockSymbol(symbol);
        }
    }
}