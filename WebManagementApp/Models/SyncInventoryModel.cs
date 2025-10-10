namespace WebManagementApp.Models
{
    public class SyncInventoryModel
    {
        public Guid idSyncInventory {  get; set; }
        public string Source_id {  get; set; }
        public string DDlRegion {  get; set; }
        public string MasterSKU {  get; set; }
        public int MasterSKUQuantity { get; set; }
        public string MarketplaceSKU { get; set; }
        public int MarketplaceSKUQuantity { get; set; }
        public int SyncQuantity { get; set; }
        public string SalesChannel { get; set; }
        public string SyncTime { get; set; }
        public string CompanyName { get; set; }
        public bool IsSyncProcessEnable { get; set; }
    }
    public class SKUDetails
    {
        public string Quantity { get; set; }
        public string TotalQuantity { get; set; }
        public string DateAdd { get; set; }
    }
    public class MasterInventorySku
    {
        public string SKU { get; set; }
    }
    public class AmzInventorySku
    {
        public string SKU { get; set; }
    }
    public class EbayInventorySku
    {
        public string SKU { get; set; }
    }
    public class OnBuyInventorySku
    {
        public string SKU { get; set; }
    }
    public class ShopifyInventorySku
    {
        public string SKU { get; set; }
    }
}
