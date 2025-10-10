using WebManagementApp.DataAccess.DbAccess;
using WebManagementApp.DataAccess.Interfaces;
using WebManagementApp.Models;

namespace WebManagementApp.DataAccess.Services
{
    public class SettingsService : ISettingsService
    {
        ISqlDataAccess _sqlDataAccess;
        public SettingsService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        public async Task<CurrencyRateModelVM> GetCurrencyRates()
        {
            CurrencyRateModelVM currencyRateModelVM = new CurrencyRateModelVM();
            var result = await _sqlDataAccess.GetData<CurrencyrateModel, dynamic>("SELECT ToCurrency, CurrentRate FROM tblCurrencyRate where FromCurrency='GBP'", new { });

            if (result.Count() > 0)
            {
                foreach (var currency in result)
                {
                    if (currency.ToCurrency == "USD")
                    {
                        currencyRateModelVM.USDCurrencyRate = currency.CurrentRate;
                    }
                    else if (currency.ToCurrency == "INR")
                    {
                        currencyRateModelVM.INRCurrencyRate = currency.CurrentRate;
                    }
                    else if (currency.ToCurrency == "EUR")
                    {
                        currencyRateModelVM.EURCurrencyRate = currency.CurrentRate;
                    }
                }
            }
            return currencyRateModelVM;
        }
        private bool IfExistCurrencyCodeEntry(string currencyCode)
        {
            int count = Convert.ToInt32(_sqlDataAccess.GetSingleValue($"select COUNT(*) from tblCurrencyRate where ToCurrency='{currencyCode}'"));
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<CurrencyRateModelVM> UpdateCurrencyRates(CurrencyRateModelVM newCurrencyRateModelVM)
        {
            var currencyRateModelVM = await GetCurrencyRates();
            string query = string.Empty;
            string updateQuary= "update tblCurrencyRate set CurrentRate =@CurrentRate,UpdateTime=GETDATE() where FromCurrency='GBP' and ToCurrency=@ToCurrency; ";
            string insertQuary= "INSERT INTO tblCurrencyRate(FromCurrency, ToCurrency,CurrentRate,DateAdd) VALUES ('GBP',@ToCurrency,@CurrentRate, GETDATE());";

            if (currencyRateModelVM.USDCurrencyRate != newCurrencyRateModelVM.USDCurrencyRate)
            {
                if(IfExistCurrencyCodeEntry("USD"))
                {
                    query = updateQuary;
                }
                else
                {
                    query = insertQuary;
                }
                await _sqlDataAccess.ExecuteDML(query, new { CurrentRate = newCurrencyRateModelVM.USDCurrencyRate, ToCurrency = "USD" });
                currencyRateModelVM.USDCurrencyRate = newCurrencyRateModelVM.USDCurrencyRate;
            }
            if (currencyRateModelVM.INRCurrencyRate != newCurrencyRateModelVM.INRCurrencyRate)
            {
                if (IfExistCurrencyCodeEntry("INR"))
                {
                    query = updateQuary;
                }
                else
                {
                    query = insertQuary;
                }
                await _sqlDataAccess.ExecuteDML(query, new { CurrentRate = newCurrencyRateModelVM.INRCurrencyRate, ToCurrency = "INR" });
                currencyRateModelVM.INRCurrencyRate = newCurrencyRateModelVM.INRCurrencyRate;
            }
            if (currencyRateModelVM.EURCurrencyRate != newCurrencyRateModelVM.EURCurrencyRate)
            {
                if (IfExistCurrencyCodeEntry("EUR"))
                {
                    query = updateQuary;
                }
                else
                {
                    query = insertQuary;
                }
                await _sqlDataAccess.ExecuteDML(query, new { CurrentRate = newCurrencyRateModelVM.EURCurrencyRate, ToCurrency = "EUR" });
                currencyRateModelVM.EURCurrencyRate = newCurrencyRateModelVM.EURCurrencyRate;
            }
            return currencyRateModelVM;
        }
    }
}
