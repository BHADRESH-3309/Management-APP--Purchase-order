using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace WebManagementApp.Models
{
    public class MasterSKUVM
    {
        public Guid idMasterSKU { get; set; }

        [DisplayName("Master SKU:")]
        public string SKU { get; set; }

        [DisplayName("Title:")]
        public string Title { get; set; }

        [DisplayName("Description:")]
        public string? Description { get; set; }

        public string? Image { get; set; }

        [DisplayName("Height (CM):")]
        public string? Height { get; set; }

        [DisplayName("Width (CM):")]
        public string? Width { get; set; }

        [DisplayName("Length (CM):")]
        public string? Length { get; set; }

        [DisplayName("Weight (KG):")]
        public string? Weight { get; set; }

        [DisplayName("Volumetric Wgt (KG):")]
        public string VolumetricWeight { get; set; }

        [DisplayName("Source:")]
        public string? ProductSource { get; set; }
        public string Brand { get; set; }

        [Required(ErrorMessage = "Please select a Brand.")]
        public Guid? idBrand { get; set; }
        public bool IsError { get; set; } = true;
        public string? Message { get; set; }

        [DisplayName("GTIN:")]
        public string? GTIN { get; set; }

        [DisplayName("EAN:")]
        public string? EAN { get; set; }

        [DisplayName("Barcode:")]
        public string? Barcode { get; set; }
        [DisplayName("Note:")]
        public string? Note { get; set; }

        // Margins
        [DisplayName("B2B Product")]
        public bool IsBtoB { get; set; }
        [DisplayName("B2B Margin(%):")]
        public string? BtoBMargin { get; set; }
        [DisplayName("Marketplace FBM Margin(%):")]
        public string? MarketplaceMargin { get; set; }
        [DisplayName("Marketplace FBA Margin(%):")]
        public string? MarketplaceFBAMargin { get; set; }

        // [DisplayName("Product Cost (£):")]
        // [RegularExpression("^[0-9]*\\.?[0-9]+$", ErrorMessage = "Please enter valid product cost")]
        // public string? _ProductCost { get; set; }
        //  public decimal ProductCost { get; set; }

        //  [DisplayName("Product Cost (€):")]
        //  public string? ProductCostEUR { get; set; }

        //   [DisplayName("Product Cost (₹):")]
        //  public string? ProductCostINR { get; set; }
        //   [DisplayName("Product Cost ($):")]
        //  public string? ProductCostUSD { get; set; }

        //  [DisplayName("Selling Price (£):")]
        //  public string? SellingPrice { get; set; }

        //  [DisplayName("Stock Location:")]
        //   public string? StockLocation { get; set; }

        //  [DisplayName("Quantity:")]
        //  public string? Quantity { get; set; }

        //  [DisplayName("DamagedQuantity:")]
        //  public string DamagedQuantity { get; set; }

        public IFormFile postedImage { get; set; }
      //  public List<StockLocationDetailVM> StockLocationDetails { get; set; } = new List<StockLocationDetailVM>();
      //  public List<string> StockLocations { get; set; } = new List<string>();
        public List<BrandVM> brandlist { get; set; }
        public List<ShippingLabelVM> shippingLabelList { get; set; }
        public List<CategoryVM> categoryList { get; set; }
        [Required(ErrorMessage = "Please select a Category.")]
        public Guid? idCategory {  get; set; }
        [Required(ErrorMessage = "Please select a Shipping Label.")]
        public Guid? idShippingLabel { get; set;}

        [DisplayName("Category")]
        public string CategoryName { get; set; }

        [DisplayName("Shipping Label")]
        public string ShippingLabelName { get; set; }
    }
    public class StockLocationDetailVM
    {
        public  Guid idStockLocation { get; set; }
        [DisplayName("Stock Location:")]
        public string StockLocation { get; set; } 

        [DisplayName("Quantity:")]
        public string Quantity { get; set; }

        [DisplayName("DamagedQuantity:")]
        public string DamagedQuantity { get; set; }
    }
    public class BrandVM
    {
        public Guid idBrand { get; set; }

        public string Brand { get; set; }

        public string DateAdd { get; set; }
        public string Mode { get; set; }

    }
    public class ImportResult
    {
        public DataTable NewData { get; set; }
        public DataTable UpdatedData { get; set; }
    }

    public class ShippingLabelVM
    {
        public Guid idShippingLabel { get; set; }

        public string ShippingLabelName { get; set; }
    }

    public class CategoryVM
    {
        public Guid idCategory { get; set; }

        public string CategoryName { get; set; }
    }
}

