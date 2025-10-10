using WebManagementApp.Models;

namespace WebManagementApp.DataAccess.Interfaces
{
    public interface ISettingsService
    {
        Task<CurrencyRateModelVM> GetCurrencyRates();
        Task<CurrencyRateModelVM> UpdateCurrencyRates(CurrencyRateModelVM newCurrencyRateModelVM);
    }
}
