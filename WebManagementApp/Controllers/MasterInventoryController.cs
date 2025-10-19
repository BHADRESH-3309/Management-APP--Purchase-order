using WebManagementApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebManagementApp.DataAccess.Interfaces;

namespace WebManagementApp.Controllers
{
    [Authorize]
    public class MasterInventoryController : Controller
    {

        IMasterInventoryService _masterInventoryService;

        public MasterInventoryController(IMasterInventoryService masterInventoryService)
        {
            _masterInventoryService = masterInventoryService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Get master inventory data.
        [HttpGet]
        public async Task<JsonResult> GetMasterInventory(string sku,string marketSku, bool isDamagedQuantity = false, bool isMarketplaceQuantity = false)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                if(!string.IsNullOrEmpty(sku))
                {
                    result = await _masterInventoryService.GetMasterInventoryBySKU(sku, isDamagedQuantity, isMarketplaceQuantity);
                }
                else if(!string.IsNullOrEmpty(marketSku))
                {
                    result = await _masterInventoryService.GetMasterInventoryByMappingSKU(marketSku, isDamagedQuantity, isMarketplaceQuantity);
                }
                else
                {
                    result = await _masterInventoryService.GetMasterInventory(isDamagedQuantity, isMarketplaceQuantity);
                }
                         
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result);
        }

        // Get master inventory log for a specific SKU.
        [HttpGet]
        public async Task<JsonResult> GetMasterInventoryLog(string masterSKU)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                if (!string.IsNullOrEmpty(masterSKU))
                {
                    result = _masterInventoryService.GetMasterInventoryLog(masterSKU).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> ManageWarehouseQuantity(string sku, string operation, string? amershamQuantity, 
            string? watfordQuantity, decimal? productCost, string supplierName, string? reduceQuantityReason)
        {
            ResponseModel result = new ResponseModel();
            string userName = HttpContext.Session.GetString("Email");

            try
            {
                if(operation == "increase")
                {
                    result = await _masterInventoryService.AddWarehouseQuantity(sku, amershamQuantity, watfordQuantity, 
                        productCost, supplierName, userName);
                }
                else
                {
                    result = await _masterInventoryService.ReduceWarehouseQuantity(sku, amershamQuantity, watfordQuantity, 
                        reduceQuantityReason, userName);
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result);
        }        

        [HttpPost]
        public async Task<JsonResult> ManageWarehouseDamagedQuantity(string sku, string operation, string? amershamDamagedQuantity, 
            string? watfordDamagedQuantity)
        {
            ResponseModel result = new ResponseModel();
            string userName = HttpContext.Session.GetString("Email");

            try
            {
                if (operation == "increase")
                {
                    result = await _masterInventoryService.AddWarehouseDamagedQuantity(sku, amershamDamagedQuantity, 
                        watfordDamagedQuantity, userName);
                }
                else
                {
                    result = await _masterInventoryService.ReduceWarehouseDamagedQuantity(sku, amershamDamagedQuantity, 
                        watfordDamagedQuantity, userName);
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> AddProductCost(string sku, string productCost, string? date)
        {
            ResponseModel result = new ResponseModel();
            string userName = HttpContext.Session.GetString("Email");

            try
            {
                result = await _masterInventoryService.AddProductCost(sku, productCost, date, userName);
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetStockLocationHistory(string masterSKU)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                if (!string.IsNullOrEmpty(masterSKU))
                {
                    result = await _masterInventoryService.GetStockLocationHistoryData(masterSKU.Trim());
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetSupplierNames()
        {
            var result = await _masterInventoryService.GetSupplierNames();
            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> AddShippingFees(string masterSKU, decimal shippingFees)
        {
            ResponseModel result = new ResponseModel();

            try
            {
                await _masterInventoryService.AddMasterInventoryShippingFees(masterSKU, shippingFees);

                result.Message = $"Shipping fees successfully added for MasterSKU : {masterSKU}";
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateCostAndDate(string idStockLocationHistory,decimal costprice, bool isDateChange, DateTime newDate)
        {
            ResponseModel result = new ResponseModel();
            string userName = HttpContext.Session.GetString("Email");
            try
            {
                var response = await _masterInventoryService.UpdateCostAndDate(idStockLocationHistory, costprice, 
                    isDateChange, newDate, userName);

                result.IsError = response.IsError;
                result.Message = response.Message;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result);
        }

    }
}









