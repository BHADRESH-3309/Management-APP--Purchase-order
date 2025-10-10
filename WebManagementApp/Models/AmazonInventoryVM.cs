namespace WebManagementApp.Models
{
    public class AmazonInventoryVM
    {
        public string SKU { get; set; }
        public string MasterSKU { get; set; }
        public string Title { get; set; }
        public string ASIN { get; set; }
        public int Quantity { get; set; }
        public int AfnWarehouseQuantity { get; set; }
        public int AfnFulfillableQuantity { get; set; }
        public int AfnUnsellableQuantity { get; set; }
        public int AfnReservedQuantity { get; set; }
        public int AfnInboundReceivingQuantity { get; set; }
        public string FulfillmentBy { get; set; }
        public decimal Price { get; set; }
        public string Source { get; set; }
        public string RecordUpdateTime { get; set; }
        public string EAN { get; set; }
        public int BundleQty { get; set; }
        public bool isSyncProcessEnable { get; set; }
    }
}
