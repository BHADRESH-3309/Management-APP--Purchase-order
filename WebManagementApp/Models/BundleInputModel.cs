using System.ComponentModel.DataAnnotations;

namespace WebManagementApp.Models
{
    public class BundleInputModel
    {
        public Guid idBundle { get; set; }
        [Display(Name = "Master SKU:*")]
        [Required(ErrorMessage = "*required")]
        public string MasterSKU { get; set; }
        [Display(Name = "Sales Channel:*")]
        [Required(ErrorMessage = "*required")]
        public string SalesChannel { get; set; }

        [Display(Name = "Marketplace SKU:*")]
        [Required(ErrorMessage = "*required")]
        public string MappingSKU { get; set; }

        [Display(Name = "Reduce Quantity:*")]
        [Required(ErrorMessage = "*required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid quantity.")]
        public int ReduceQuantity { get; set; }
    }
}
