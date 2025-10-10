namespace WebManagementApp.Models
{
    public class RestockInventory
    {
        public string SKU { get; set; }
        public string Title { get; set; }
        public int Stock { get; set; }
        public int TotalSalesQuantity { get; set; }
        public decimal SalesVelocity { get; set; }
        public decimal DaysAvailable { get; set; }
        public string GTIN { get; set; }
        public string Brand { get; set; }
        public int AmershamQty { get; set; }
        public int WatfordQty { get; set; }
        public int fba { get; set; }
    }
}
