using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace WebManagementApp.Models
{
    public class PurchaseOrderModel
    {
        public Guid idPurchaseOrder {  get; set; }
        public Guid idPurchaseOrderProduct {  get; set; }

        [Required]
        public int PONumber { get; set; }
       
        [Required(ErrorMessage = "Please enter the supplier name.")]
        public string? SupplierName { get; set; }

        [Required(ErrorMessage = "Please select a valid date.")]
        [DataType(DataType.Date)]
        public string? DeliveryDate { get; set; }

        [Required]
        public string? Note { get; set; }

        public string? MasterSKU {  get; set; }

        public string? ItemName { get; set; }

        public string? Brand { get; set; }

        public Guid idBrand { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string? Currency { get; set; }

        [Required(ErrorMessage = "Currency Rate is required")]
        [Range(typeof(decimal), "0.00000001", "99999999.99999999", ErrorMessage = "Please enter a valid exchange rate up to 8 decimal places.")]
        public decimal CurrencyRate { get; set; }
        public decimal Exchange { get; set; }

        /* Update PO fields */
        [Display(Name = "Status:")]
        public string? Status {  get; set; }

        [Display(Name = "Received Date:")]
        public DateTime? ReceivedDate {  get; set; }

        [Display(Name = "Received Total Quantity:")]
        public string? ReceivedCount { get; set; }

        [Display(Name = "Damage Count:")]
        public int DamageCount {  get; set; }

        [Display(Name = "Missing Count:")]
        public int MissingCount {  get; set; }

        [Display(Name = "Issue Description:")]
        public string? IssueDescription {  get; set; }

        [Display(Name = "Invoice Name:")]
        public string? InvoiceName {  get; set; }

        [Display(Name = "Invoice Upload:")]
        public IFormFile? InvoiceFile { get; set; }

        public DateTime? ModifyDate { get; set; }

        /* Company Stock location quantity */
        [Display(Name = "Amersham Quantity:")]
        public string? AmershamQty { get; set; }

        //[Display(Name = "Amersham Qty AVA Supplies:")]
        //public string? AmershamQtyAVASupplies { get; set; }

        [Display(Name = "Watford Quantity:")]
        public string? WatfordQty { get; set; }

        //[Display(Name = "Watford Qty AVA Supplies:")]
        //public string? WatfordQtyAVASupplies { get; set; }

        [Display(Name = "Total Qty:")]
        public string? TotalQty { get; set; }

        [Display(Name = "Company Name:")]
        public string? CompanyName { get; set; }

        /* Transfer Stock */
        //[Display(Name = "Transfer Stock From:")]
        //public string? TransferStockFromCompany { get; set; }
        //[Display(Name = "Transfer Stock To:")]
        //public string? TransferStockToCompany { get; set; }
        //public string? TransferAmershamQty { get; set; }
        //public string? TransferWatfordQty { get; set; }
        //public string? TransferAmershamAVAQty { get; set; }
        //public string? TransferWatfordAVAQty { get; set; }

        public List<MasterSKUList> masterSKUList { get; set; }
        public List<PONumberList> poNumberList { get; set; }
        public List<PurchaseOrderItemViewModel> Items { get; set; } = new();
        public List<PurchaseOrderItemListModel> ItemsList { get; set; } = new();

        public List<SupplierList> supplerlist { get; set; }
        public List<BrandsList> brandNamelist { get; set; }
        public List<GTINList> gtinlist { get; set; }

        /* Inventory Claims */
        public Guid idInventoryClaims {  get; set; }
        public string? IssueType { get; set; }
        public string? ClaimAmount { get; set; }
        public DateTime? SettlementDate { get; set; }

        /* Stock Aging Report */
        public int Aging_0_30 { get; set; }
        public int Aging_30_60 { get; set; }
        public int Aging_60_90 { get; set; }
        public int Aging_90_plus { get; set; }

        public class PurchaseOrderItemViewModel
        {
            public Guid idPurchaseOrderProduct { get; set; }

            [Required]
            public string MasterSKU { get; set; }

            public string ItemName {  get; set; }

            public string Brand { get; set; }

            public string? GTIN { get; set; }

            [Required]
            public int Quantity { get; set; }

            [Required]
            public decimal Price { get; set; }

            [Required]
            public string Currency { get; set; }

            public decimal Exchange {  get; set; }
        }

        public class MasterSKUList
        {
            public Guid idMasterSKU { get; set; }
            public string SKU { get; set; }
        }

        public class PONumberList
        {
            public Guid idPurchaseOrder { get; set; }
            public string PONumber { get; set; }
        }

        public class SupplierList
        {
            public Guid idSupplier { get; set; }
            public string SupplierName { get; set; }
        }

        public class BrandsList
        {
            public Guid idBrand { get; set; }
            public string Brand { get; set; }
        }    
        
        public class CurrencyModel
        {
            public string CurrencyName { get; set; }                                            
            public string CurrencyRate { get; set; }                                            

        }

        public class GTINList
        {
            public Guid idMasterSKU { get; set; }
            public string GTIN { get; set; }
        }

        public class PurchaseOrderItemListModel
        {
            public Guid idPurchaseOrderProduct { get; set; }
            public Guid idPurchaseOrder { get; set; }
            public string? GTIN { get; set; }
            public string MasterSKU { get; set; }
            public string ItemName { get; set; }
            public int Quantity { get; set; }
            public int ReceivedCount { get; set; }
            public int AmershamQuantity { get; set; }
            public int WatfordQuantity { get; set; }
            public int DamageCount { get; set; }
            public int MissingCount { get; set; }
            public string? IssueDescription { get; set; }
        }

    }
}
