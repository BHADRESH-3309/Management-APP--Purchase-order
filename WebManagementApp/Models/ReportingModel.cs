namespace WebManagementApp.Models
{
    public class ReportingModel
    {
        public string MasterSKU { get; set; }
        public string Title { get; set; }
        public string AmershamQty { get; set; }
        public string WatfordQty { get; set; }
        public string TotalQty { get; set; }
        public decimal ProductCost { get; set; }
        public decimal TotalStockValue { get; set; }
        public string FBA { get; set; }
        public string Brand { get; set; }
        public string GTIN { get; set; }
        public int Aging_0_30 { get; set; }
        public int Aging_30_60 { get; set; }
        public int Aging_60_90 { get; set; }
        public int Aging_90_plus { get; set; }
        public string Age { get; set; }
        public string DateAdd { get; set; }
        public string FBAGreenwize { get; set; }
        public string FBAAVASupplies { get; set; }
        public List<BrandList> brandNameList { get; set; }
        public List<SalesChannelList> salesChannelList { get; set; }
    }

    public class StockAgingDataHistory
    {
        public string DateAdd { get; set; }
        public int TotalStockValue { get; set; }
        public int FBA { get; set; }
        public decimal ProductCost { get; set; }
    }

    public class BrandNameList
    {
        public Guid idBrand { get; set; }
        public string Brand { get; set; }
    }
    public class SalesChannelList
    {
        public int idSalesChannel { get; set; }
        public string ChannelName { get; set; }
    }
}
