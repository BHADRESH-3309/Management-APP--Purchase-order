namespace WebManagementApp.Models
{
    public class MappingSkuRequestModel
    {
        public string idMasterSKU { get; set; }
        public string MasterSKU { get; set; }
        public List<DataModel> Data { get; set; }
    }

    public class DataModel
    {
        public string SelectValue { get; set; }
        public string InputValue { get; set; }
        public string idMappingSKU { get; set; }
        public int Quantity { get; set; }
        public bool IsSyncProcessEnable { get; set; }
    }
}
