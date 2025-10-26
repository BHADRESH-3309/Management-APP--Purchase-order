using WebManagementApp.Models;

namespace WebManagementApp.DataAccess.Interfaces
{
    public interface IMasterInventoryService
    {
        Task<ResponseModel> GetMasterInventory(bool isDamagedQuantity , bool isMarketplaceQuantity);
        Task<ResponseModel> GetMasterInventoryLog(string masterSKU);
        Task<ResponseModel> GetStockLocationHistoryData(string masterSKU);

        Task<ResponseModel> AddWarehouseQuantity(string sku, string? amershamQuantity, string? watfordQuantity, decimal? productCost, string supplierName, string user, string additionalText = "");
        Task<ResponseModel> ReduceWarehouseQuantity(string sku, string? amershamQuantity, string? watfordQuantity, string reduceQuantityReason, string user);
        Task<ResponseModel> AddWarehouseDamagedQuantity(string sku, string? amershamDamagedQuantity, string? watfordDamagedQuantity, string user, string additionalText = "");
        Task<ResponseModel> ReduceWarehouseDamagedQuantity(string sku, string? amershamDamagedQuantity, string? watfordDamagedQuantity, string user);

        Task<string[]> GetSupplierNames();

        Task<ResponseModel> GetMasterInventoryBySKU(string sku, bool isDamagedQuantity, bool isMarketplaceQuantity);
        Task<ResponseModel> GetMasterInventoryByMappingSKU(string marketSku, bool isDamagedQuantity, bool isMarketplaceQuantity);

        Task AddMasterInventoryShippingFees(string masterSKU, decimal shippingFees);

        Task<ResponseModel> AddProductCost(string sku, string productCost, string? date, string user);
        Task<ResponseModel> UpdateCostAndDate(string idStockLocationHistory, decimal costprice, bool isDateChange, DateTime newDate, string userName);
    }
}
