using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
using WebManagementApp.DataAccess.Interfaces;
using WebManagementApp.DataAccess.Services;
using WebManagementApp.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static WebManagementApp.Models.PurchaseOrderModel;

namespace WebManagementApp.Controllers
{
    public class PurchaseOrderController : Controller
    {
        IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        /* Create/Edit Purchase Order page - Start */
        [HttpGet]
        public ActionResult Index()
        {            
            PurchaseOrderModel model = new PurchaseOrderModel();
            try
            {
                model = new PurchaseOrderModel();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
            }
            return View(model);
        }

        // Retrieve master sku list for dropdown.
        [HttpGet]
        public JsonResult GetMasterSKUList()
        {
            var list = _purchaseOrderService.GetMasterSKUList(); 
            return Json(list);
        }

        // Retrieve GTIN list for dropdown.
        [HttpGet]
        public JsonResult GetGTINList()
        {
            var list = _purchaseOrderService.GetGTINList();
            return Json(list);
        }

        // Method to Auto fill Title and Brand name for selected Master SKU.
        [HttpGet]
        public async Task<JsonResult> GetMasterSKUDetails(string selectedDropdownValue, string type)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                if (!string.IsNullOrEmpty(selectedDropdownValue))
                {
                    //Function call
                    result = await _purchaseOrderService.GetMasterSKUDetails(selectedDropdownValue, type);
                    if (result == null)
                    {
                        result.IsError = true;
                        result.Message = $"{type} not found.";
                    }
                }
                else
                {
                    result.IsError = true;
                    result.Message = $"Please select a {type}.";  
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result);
        }

        // Method to create purchase order.
        [HttpPost]
        //public JsonResult Index(PurchaseOrderModel model)
        public async Task<JsonResult> Index(PurchaseOrderModel model)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                    result = await _purchaseOrderService.ImportPurchaseOrderDetails(model);
                    return Json(result);                
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        /* Create Purchase Order page - End */

        /* Edit Purchase Order page - Start /
         * Returns the list of all PO numbers. */
        [HttpGet]
        public JsonResult GetPONumberList()
        {
            try
            {
                var poNumberList = _purchaseOrderService.GetPONumberList();
                return Json(poNumberList);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Returns purchase order details for a given PO number.
        [HttpGet]
        public async Task<JsonResult> GetPurchaseOrderDetails(int poNumber)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                result = await _purchaseOrderService.GetPurchaseOrderDetails(poNumber);               
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result);
        }

        // Update edited purchase order.
        [HttpPost]
        public async Task<JsonResult> EditPurchaseOrder(PurchaseOrderModel model)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                result = await _purchaseOrderService.EditPurchaseOrder(model);
                return Json(result);
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result);
        }

        // Delete the selected purchase order product.
        [HttpPost]
        public async Task<JsonResult> DeleteProduct(string productId)
        {
            try
            {
                await _purchaseOrderService.DeleteProduct(productId);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Error = true, Message = ex.Message });
            }

            return new JsonResult(new { Error = false, Message = "Purchase order product deleted successfully." });
        }
        /* Edit Purchase Order page - End */

        /* Purchase Order list page - Start *
         * To get Supplier and Brand dropdown values. */
        [HttpGet]
        public ActionResult PurchaseOrder()
        {
            PurchaseOrderModel purchaseOrderModel = new PurchaseOrderModel();

            string message = TempData["SuccessMessage"] as string;
            TempData["SuccessMessage"] = null; // Remove the message from TempData
            ViewData["SuccessMessage"] = message;

            try
            {
                //Function call
                purchaseOrderModel.supplerlist = _purchaseOrderService.GetSupplierList();
                purchaseOrderModel.brandNamelist = _purchaseOrderService.GetBrandList();
                purchaseOrderModel.poNumberList = _purchaseOrderService.GetPONumber();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
            }
            return View(purchaseOrderModel);
        }

        // Get Purchase order data.
        [HttpGet]
        public async Task<JsonResult> GetPurchaseOrder(string status, string supplierName, string brand, 
            string poNumber, string selectedPO)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                if (!string.IsNullOrEmpty(selectedPO))
                {
                    poNumber = selectedPO;
                }

                result = await _purchaseOrderService.GetPurchaseOrder(status, supplierName, brand, poNumber);
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result);
        }
        /* Purchase Order list page - End */

        /* Update Purchase order page - Start *
         * Update Purchase order details. */
        [HttpGet]
        public ActionResult UpdatePurchaseOrder(string id)
        {
            var model = new PurchaseOrderModel();
            try
            {
                model = _purchaseOrderService.GetUpdatePOItemDetails(id);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
            }                      
            return View(model);
        }

        // Method to redirect to purchase order page.
        [HttpPost]
        public ActionResult UpdatePurchaseOrder(PurchaseOrderModel model)
        {
            string userName = HttpContext.Session.GetString("Email");
            try
            {
                _purchaseOrderService.UpdatePurchaseOrder(model, userName);
                TempData["SuccessMessage"] = "Purchase Order updated successfully!";
                return RedirectToAction("PurchaseOrder");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }
        /* Update Purchase order page - End */

        /* Inventory Claims page - Start */
        [HttpGet]
        public IActionResult InventoryClaims()
        {
            return View();
        }

        // Function to get Inventory Claims.
        [HttpGet]
        public async Task<JsonResult> GetInventoryClaim()
        {
            ResponseModel result = new ResponseModel();
            try
            {
                result = await _purchaseOrderService.GetInventoryClaim();
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result);
        }

        // Function to update Inventory Claims.
        [HttpPost]
        public async Task<JsonResult> UpdateInventoryClaim(PurchaseOrderModel model)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                result = await _purchaseOrderService.UpdateInventoryClaim(model);
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result);
        }
        /* Inventory Claims page - End */

        /* Stock Receipt Report page - Start */
        [HttpGet]
        public IActionResult StockReceiptReport()
        {
            return View();
        }

        // Get Stock Receipt Report data.
        [HttpGet]
        public async Task<JsonResult> GetStockReceiptReportData()
        {
            ResponseModel result = new ResponseModel();
            try
            {
                result = await _purchaseOrderService.GetStockReceiptReportData();
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result);
        }
        /* Stock Receipt Report page - End */

        /* Stock Aging Report page - Start */
        [HttpGet]
        public IActionResult StockAgingReport()
        {
            return View();
        }

        // Get Stock Receipt Report data.
        [HttpGet]
        public async Task<JsonResult> GetStockAgingReportData(string selectedFilter)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                result = await _purchaseOrderService.GetStockAgingReportData(selectedFilter);
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result);
        }
        /* Stock Aging Report page - End */
    }
}
