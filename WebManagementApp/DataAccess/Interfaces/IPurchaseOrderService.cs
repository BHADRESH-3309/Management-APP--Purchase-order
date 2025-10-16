using WebManagementApp.Models;
using static WebManagementApp.Models.PurchaseOrderModel;

namespace WebManagementApp.DataAccess.Interfaces
{
    public interface IPurchaseOrderService
    {
        List<MasterSKUList> GetMasterSKUList();
        List<GTINList> GetGTINList();
        Task<ResponseModel> GetMasterSKUDetails(string selectedDropdownValue, string type);
        Task<ResponseModel> ImportPurchaseOrderDetails(PurchaseOrderModel model);

        List<PONumberList> GetPONumberList();
        Task<ResponseModel> GetPurchaseOrderDetails(int poNumber);
        Task<ResponseModel> EditPurchaseOrder(PurchaseOrderModel model);
        Task DeleteProduct(string productId);

        List<SupplierList> GetSupplierList();
        List<BrandsList> GetBrandList();
        List<PONumberList> GetPONumber();
        Task<ResponseModel> GetPurchaseOrder(string status, string supplierName, string brand, string poNumber);

        PurchaseOrderModel GetUpdatePOItemDetails(string id);
        PurchaseOrderModel UpdatePurchaseOrder(PurchaseOrderModel model, string userName);

        Task<ResponseModel> GetInventoryClaim();
        Task<ResponseModel> UpdateInventoryClaim(PurchaseOrderModel model);

        Task<ResponseModel> GetStockReceiptReportData();
        Task<ResponseModel> GetStockAgingReportData(string selectedFilter);
        string GetLastPoNumber();
    }
}
