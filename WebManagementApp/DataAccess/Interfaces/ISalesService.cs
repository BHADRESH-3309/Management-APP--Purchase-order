using WebManagementApp.Models;

namespace WebManagementApp.DataAccess.Interfaces
{
    public interface ISalesService
    {
        Task<ResponseModel> UpdateFees( string idSales, string listingFee, string advertisingFee,string transactionFee, string salesChannelName);
        Task<ResponseModel> UpdateTrackingNoAndFees(string trackingNo, string idOrder, string idSales, string listingFee, string advertisingFee, string transactionFee, string salesChannelName,string stockLocation);
        Task<ResponseModel> GetSaleschannelData(string salesChannelName, string startDate, string endDate,string mappedType);
        bool IsStockAvailable(string idSales,string stockLocation, string salesChannelName, int quantity);
        Task<ResponseModel> GetSales(string sku, string salesChannelName, string startDate, string endDate, string companyName);
    }
}
