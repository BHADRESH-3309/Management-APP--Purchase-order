namespace WebManagementApp.Models
{
    public class BtoBStockReportModel
    {
        public string MasterSKU { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string GTIN { get; set; }
        public string UPC { get; set; }
        public string AmershamQty { get; set; }
        public string WatfordQty { get; set; }
        public string TotalQty { get; set; }
        public decimal AvgCostPrice { get; set; }
        public string BtoBMargin { get; set; }
        public string MarketplaceMargin { get; set; }

        public List<BrandNameLists> brandNamelists { get; set; }
        public List<MasterSKULists> masterSKULists { get; set; }
    }

    public class BrandNameLists
    {
        public Guid idBrand { get; set; }
        public string Brand { get; set; }
    }

    public class MasterSKULists
    {
        public Guid idMasterSKU { get; set; }
        public string MasterSKU { get; set; }
    }
}
