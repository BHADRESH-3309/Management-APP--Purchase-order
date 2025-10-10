namespace WebManagementApp.Models
{
    public class TopSKU
    {
        public string SKU { get; set; }
        public int TotalSalesQuantity { get; set; }
        public decimal Margin { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public decimal AvgCostPrice { get; set; }
        public string GTIN { get; set; }
        public string Brand { get; set; }
        public int SalesQty { get; set; }
        public int MappedQty { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal AvgPriceCost { get; set; }
        public decimal WeightedAvgPriceCost { get; set; }
        public decimal Fees { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal Expenses { get; set; }
        public decimal Revenue { get; set; }
    }
}
