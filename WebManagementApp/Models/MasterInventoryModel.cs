using System.ComponentModel;
using System.Data;

namespace WebManagementApp.Models
{
    public class MasterInventoryModel
    {
        public string MasterSKU { get; set; }
        public string SKU { get; set; }
        public string idMasterSKU { get; set; }
        public string Title { get; set; }
        public string FBA { get; set; }
        public string FBAAVASupplies { get; set; }
        public string AmershamQty { get; set; }                                                                                                              
        public string WatfordQty { get; set; }           
        public string TotalQty { get; set; }   
        public string DamagedAmershamQty { get; set; }
        public string DamagedWatfordQty { get; set; }
        public string AvgCostPrice { get; set; }
        public string AmazonQty { get; set; }
        public string AmazonQtyAVASupplies { get; set; }
        public string eBayQty { get; set; }
        public string OnBuyQty { get; set; }
        public string ShopifyQty { get; set; }
        public float? ShippingFee { get; set; }
        public string GTIN { get; set; }
        public string EAN { get; set; }
        public string? MappingSKU { get; set; }

        [DisplayName("Damaged Quantity")]
        public bool IsDamagedQuantity { get; set; }

        [DisplayName("Marketplace Quantity")]
        public bool IsMarketplaceQuantity { get; set; }
        public string salesChannel { get; set; }
    }
    
    public class StockLocationModel
    {
        public Guid idMasterSKU { get; set; }
        public Guid idStockLocation { get; set; }
        public string SKU { get; set; }
        public string StockLocation { get; set; }
        public string Quantity { get; set; }
        public string CostPrice { get; set; }
        public string AvgCostPrice { get; set; }
        public string DamagedQuantity { get; set; }
        public string LogMessage { get; set; }
        public string DateAdd { get; set; }
        public string SupplierName { get; set; }
    }
}
