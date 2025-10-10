namespace WebManagementApp.Models
{
    public class SalesModel
    {
        public Guid idSales {  get; set; }
        public Guid idMasterSKU {  get; set; }
        public string OrderId { get; set; }
        public string OrderDate { get; set; }
        public string MarketplaceSKU { get; set; }
        public string Title { get; set; }
        public string Quantity { get; set; }
        public string StockLocation { get; set; }
        public List<string> StockLocationList { get; set; }
        public string SalesChannel { get; set; }
        public string FulfillmentChannel { get; set; }
        public string Source { get; set; }
        public string FulfillmentBy { get; set; }
        public string ProductSource { get; set; }
        public string TrackingNo { get; set; }
        public string LabelPath { get; set; }
        public bool IsEvriShipping { get; set; }
        public bool IsRoyalMailShipping { get; set; }
        public bool IsQuantityProcess { get; set; }

        public decimal Price { get; set; }
        public decimal ReferralFee {  get; set; }
        public decimal VariableClosingFee {  get; set; }
        public decimal FBAFees {  get; set; }
        public decimal BoostFee {  get; set; }
        public decimal SalesFee {  get; set; }
        public string ListingFee {  get; set; }
        public decimal FinalValueFee {  get; set; }
        public string TransactionFee {  get; set; }
        public string AdvertisingFee {  get; set; }
        public string Day30 { get; set; }
        public string Day60 { get; set; }
        public string Day90 { get; set; }
        public string DateAdd { get; set; }
        public string ItemStatus { get; set; }
    }
}
