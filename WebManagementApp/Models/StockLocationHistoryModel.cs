namespace WebManagementApp.Models
{
    public class StockLocationHistoryModel
    {
        public Guid idStockLocationHistory { get; set; }
        public string CompanyName { get; set; }
        public string StockLocation { get; set; }
        public string Quantity { get; set; }
        public string CostPrice { get; set; }
        public string SupplierName { get; set; }
        public string DateAdd { get; set; }
    }
}
