using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using WebManagementApp.DataAccess.Interfaces;
using WebManagementApp.Models;

namespace WebManagementApp.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }
        public async Task<IActionResult> CurrencyRate()
        {
            CurrencyRateModelVM currencyRateModelVM = new CurrencyRateModelVM();
            try
            {
                currencyRateModelVM = await _settingsService.GetCurrencyRates();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Retrieve Currency rates error: " + ex.Message;
            }
            return View(currencyRateModelVM);
        }
        [HttpPost]
        public async Task<IActionResult> CurrencyRate(CurrencyRateModelVM currencyRateModelVM)
        {
            try
            {
                currencyRateModelVM = await _settingsService.UpdateCurrencyRates(currencyRateModelVM);
                TempData["SuccessMessage"] = "Currency rates updated successfully.";

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Currency rates updated error: " + ex.Message;
            }
            return RedirectToAction("CurrencyRate");
        }
    }
}
