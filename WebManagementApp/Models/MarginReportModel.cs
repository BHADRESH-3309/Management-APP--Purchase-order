namespace WebManagementApp.Models
{
    public class MarginReportModel
    {
        public string MasterSKU { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public decimal AvgCostPrice { get; set; }
        public string Brand { get; set; }
        public string GTIN { get; set; }
        public string EAN { get; set; }
        public string MarketplaceSKU { get; set; }
        public string SalesChannel { get; set; }
        public string CompanyName { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Fees { get; set; }
        public decimal Fee { get; set; }
        public int UnitSold { get; set; }
        public string ProductURL { get; set; }
        public decimal TotalSales { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal NetProfit { get; set; }
        public decimal Margin { get; set; }
        public decimal Expense { get; set; }
        public int BundleQty { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAvgCostPrice { get; set; }
    }
}
