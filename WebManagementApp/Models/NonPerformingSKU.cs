namespace WebManagementApp.Models
{
    public class NonPerformingSKU
    {
        public string SKU { get; set; }
        public int TotalSalesQuantity { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public decimal AvgCostPrice { get; set; }
        public string GTIN { get; set; }
        public string Brand { get; set; }
    }
}
