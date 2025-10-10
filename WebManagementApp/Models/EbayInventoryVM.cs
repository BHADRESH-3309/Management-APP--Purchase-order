namespace WebManagementApp.Models
{
    public class EbayInventoryVM
    {
        public string SKU { get; set; }
        public string MasterSKU { get; set; }
        public string Title { get; set; }
        public string ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string RecordUpdateTime { get; set; }
        public string EAN { get; set; }
        public int BundleQty { get; set; }
        public bool isSyncProcessEnable { get; set; }
    }
}
