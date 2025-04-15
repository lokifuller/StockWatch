namespace StockPricesApp
{
    public class GatherStockPrice
    {
        public string datetime { get; set; }
        
        public decimal close { get; set; } // gathers price at market close

        public decimal high { get; set; } // gathers price at intra-day high

        public decimal low { get; set; } // gathers price a intra-day low
         
        public decimal open { get; set; } // gathers price at market open

        public long volume { get; set; } // gathers trading volume
    }
}