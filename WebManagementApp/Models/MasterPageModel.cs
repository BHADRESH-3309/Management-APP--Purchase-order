namespace WebManagementApp.Models
{
    public class MasterPageModel
    {
    }
    public class CategoryModel
    {
        public Guid idCategory { get; set; }
        public string CategoryName { get; set; }
        public string DateAdd { get; set; }
        public string ModifyDate { get; set; }
        public string Mode { get; set; } 
    }
    public class ShippingLabelModel
    {
        public Guid idShippingLabel { get; set; }
        public string ShippingLabelName { get; set; }
        public decimal ShippingRate { get; set; }
        public string DateAdd { get; set; }
        public string ModifyDate { get; set; }
        public string Mode { get; set; }
    }

    public class SupplierModel
    {
        public Guid idSupplier { get; set; }
        public string SupplierName { get; set; }
        public string ContactPerson { get; set; }
        public string Address1{ get; set; }
        public string Address2{ get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DateAdd { get; set; }
        public string ModifyDate { get; set; }
        public string Mode { get; set; }
    }
}
