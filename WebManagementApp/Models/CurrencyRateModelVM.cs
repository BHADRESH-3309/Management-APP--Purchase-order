using System.ComponentModel.DataAnnotations;

namespace WebManagementApp.Models
{
    public class CurrencyRateModelVM
    {
        public decimal USDCurrencyRate { get; set; }
        public decimal INRCurrencyRate { get; set; }
        public decimal EURCurrencyRate { get; set; }
    }
}
