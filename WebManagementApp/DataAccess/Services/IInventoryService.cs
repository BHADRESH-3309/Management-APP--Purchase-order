using WebManagementApp.Models;

namespace WebManagementApp.DataAccess.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<AmazonInventoryVM>> GetAmazonInventoryData(string mappedType);
        Task<IEnumerable<EbayInventoryVM>> GetEbayInventoryData(string mappedType);
        Task<IEnumerable<OnBuyInventoryVM>> GetOnBuyInventoryData(string mappedType);
        Task<IEnumerable<ShopifyInventoryVM>> GetShopifyInventoryData(string mappedType);
        Task<IEnumerable<AmazonInventoryVM>> GetAmazonAVASuppliesInventoryData(string mappedType);
        Task<ResponseModel> UpdateUnmappedSKU(string marketplaceSKU, string? masterSKU, string? bundleQty,
            bool isSyncProcessEnable, string salesChannelName);
    }
}
