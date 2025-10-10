namespace WebManagementApp.Models
{
    public class MappingSkus
    {
        public Guid idMasterSKU { get; set; }
        public string MasterSKU { get; set; }
        public string Title { get; set; }
        public string? MappingSKU { get; set; }
        public string SalesChannel { get; set; }
    }
}
