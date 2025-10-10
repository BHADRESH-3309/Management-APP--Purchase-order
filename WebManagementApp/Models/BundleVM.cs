namespace WebManagementApp.Models
{
    public class BundleVM
    {
        public Guid idBundle { get; set; }
        public string MasterSKU { get; set; }
        public string SalesChannel { get; set; }
        public string Title { get; set; }
        public string MappingSKU { get; set; }
        public int ReduceQuantity { get; set; }
    }
}
