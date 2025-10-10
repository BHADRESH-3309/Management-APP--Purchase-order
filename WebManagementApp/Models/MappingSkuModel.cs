namespace WebManagementApp.Models
{
    public class MappingSkuModel
    {
        public Guid idMappingSKU { get; set; }
        public string SKU { get; set; }
        public string MappingType { get; set; }
        public int Quantity { get; set; }
        public bool IsSyncProcessEnable { get; set; }
        public Guid idMasterSKU { get; set; }
        public string CompanyName { get; set; }
    }
}
