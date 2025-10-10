namespace WebManagementApp.Models
{
    public class ProductModel
    {
        public Guid idMasterSKU { get; set; }
        public string Image {  get; set; }
        public string Title { get; set; }
        public string SKU { get; set; }
        public string MarketplaceSKU { get; set; }
        public string ProductSource { get; set; }
        public string Brand { get; set; }
        public string GTIN { get; set; }
        public string EAN { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public decimal Weight { get; set; }
        public string Note { get; set; }
        public decimal AvgCostPrice { get; set; }
        public int Quantity { get; set; }
        public bool IsSyncProcessEnable { get; set; }

        // Margins
        public bool IsBtoB { get; set; }
        public string BtoBMargin { get; set; }
        public string MarketplaceMargin { get; set; }
        public string BtoBPrice { get; set; }
        public string MarketplacePrice { get; set; }

        // SKU Mapping
        public MappingSkuFileModel MappingSkuFileModel { get; set; }
        public string? MappingSKU { get; set; }
        public string SalesChannel { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }

        public List<BrandList> brandlist { get; set; }
    }
    public class BrandList
    {
        public Guid idBrand { get; set; }
        public string Brand { get; set; }
    }
}
