using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using WebManagementApp.DataAccess.DbAccess;
using WebManagementApp.DataAccess.Interfaces;
using WebManagementApp.Models;
using static WebManagementApp.Models.PurchaseOrderModel;

namespace WebManagementApp.DataAccess.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ISqlDataAccess _db;
        private readonly IConfiguration _configuration;
        private IWebHostEnvironment _hostEnvironment;
        private readonly string _companyGreenwize;
        private readonly string _companyAVASupplies;
        Guid idMasterSKU = Guid.Empty;
        string masterSKU = string.Empty, userName = string.Empty;

        public PurchaseOrderService(ISqlDataAccess db, IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _companyGreenwize = configuration["CompanyName:CompanyName1"];
            _companyAVASupplies = configuration["CompanyName:CompanyName2"];
        }

        /* Create Purchase order page - Start * 
         * for return dropdown master sku list. */
        public List<MasterSKUList> GetMasterSKUList()
        {
            List<MasterSKUList> masterSKUList = new List<MasterSKUList>();

            string existingmasterSKUListQuery = string.Empty;
            existingmasterSKUListQuery = $@"select idMasterSKU, SKU from tblMasterSKU";

            masterSKUList = _db.GetData<MasterSKUList, dynamic>(existingmasterSKUListQuery, new { }).GetAwaiter().GetResult().ToList();

            return masterSKUList;
        }
        public string GetLastPoNumber()
        {
            string rtn = "";

            string existingmasterSKUListQuery = string.Empty;
            existingmasterSKUListQuery = $@"SELECT TOP 1 PONumber FROM tblPurchaseOrder ORDER by PONumber DESC";

            rtn = _db.GetData<string, dynamic>(existingmasterSKUListQuery, new { }).GetAwaiter().GetResult().FirstOrDefault();

            return rtn;
        }

        // for return dropdown GTIN list.
        public List<GTINList> GetGTINList()
        {
            List<GTINList> gtinList = new List<GTINList>();

            string existingGTINListQuery = string.Empty;
            existingGTINListQuery = $@"select idMasterSKU, ISNULL(GTIN,'') AS GTIN from tblMasterSKU";

            gtinList = _db.GetData<GTINList, dynamic>(existingGTINListQuery, new { }).GetAwaiter().GetResult().ToList();

            return gtinList;
        }

        // Retrieve Title and Brand name for selected Master SKU. 
        public async Task<ResponseModel> GetMasterSKUDetails(string selectedDropdownValue, string type)
        {
            ResponseModel response = new ResponseModel();
            string query = string.Empty;
            try
            {
                if(type == "Master SKU")
                {
                    query = @"SELECT m.Title AS ItemName, b.Brand, m.GTIN from tblMasterSKU m Inner join tblBrand b 
                                ON m.idBrand = b.idBrand WHERE m.SKU = @selectedDropdownValue";
                }

                if(type == "GTIN")
                {
                    query = @"SELECT m.SKU AS MasterSKU, m.Title AS ItemName, b.Brand from tblMasterSKU m Inner join tblBrand b 
                                ON m.idBrand = b.idBrand WHERE m.GTIN = @selectedDropdownValue";
                    
                }
                var result = await _db.GetData<PurchaseOrderItemViewModel, dynamic>(query, new { selectedDropdownValue });
                response.IsError = false;
                response.Result = result;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.Message = ex.Message;
            }
            return response;
        }

        // Retrieves the list of currency conversion rates from the database.
        public Task<IEnumerable<CurrencyModel>> GetCurrencyRate()
        {
            string query = string.Empty;
            query = @"SELECT ToCurrency AS CurrencyName, CurrentRate AS CurrencyRate from tblCurrencyRate";
            return _db.GetData<CurrencyModel, dynamic>(query, new { });
        }

        // Insert purchase order details.
        public async Task<ResponseModel> ImportPurchaseOrderDetails(PurchaseOrderModel model)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                // Get next available PO Number
                int newPONumber = Convert.ToInt32(_db.GetSingleValue("SELECT ISNULL(MAX(PONumber), 1000) + 1 FROM tblPurchaseOrder"));

                // Insert Purchase Order details
                string insertPOQuery = @"INSERT INTO tblPurchaseOrder(PONumber, SupplierName, DeliveryDate, Note, Currency, CurrencyRate)
                                        VALUES (@PONumber, @SupplierName, @DeliveryDate, @Note, @Currency, @CurrencyRate)";
                await _db.ExecuteDML(insertPOQuery, new
                {
                    PONumber = newPONumber,
                    model.SupplierName,
                    model.DeliveryDate,
                    model.Note,
                    model.Currency,
                    model.CurrencyRate
                });

                // Get PO Number id
                var idPONumber = _db.GetSingleValue($@"select idPurchaseOrder from tblPurchaseOrder where PONumber = '{newPONumber}'");

                // Insert PO Number into Inventory Claims
                string insertPONumberQuery = @"INSERT INTO tblInventoryClaims (idPurchaseOrder) VALUES (@idPurchaseOrder)";
                await _db.ExecuteDML(insertPONumberQuery, new {idPurchaseOrder = idPONumber});

                // Insert each purchase order item(product)
                if (model.Items != null && model.Items.Any())
                {
                    string insertItemQuery = @"INSERT INTO tblPurchaseOrderProduct (idPurchaseOrder, MasterSKU, ItemName, 
                            Brand, GTIN, Quantity, Price, Exchange)
                            VALUES (@idPurchaseOrder, @MasterSKU, @ItemName, @Brand, @GTIN, @Quantity, @Price, @Exchange)";

                    foreach (var item in model.Items)
                    {
                        await _db.ExecuteDML(insertItemQuery, new
                        {
                            idPurchaseOrder = idPONumber,
                            item.MasterSKU,
                            item.ItemName,
                            item.Brand,
                            item.GTIN,
                            item.Quantity,
                            item.Price,
                            item.Exchange
                        });
                    }
                }

                response.IsError = false;
                response.Message = "Purchase Order created successfully.";
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.Message = ex.Message;
            }
            return response;
        }
        /* Create Purchase order page - End */

        /* Edit Purchase order page - Start * 
         * for return dropdown PO Number list. */
        public List<PONumberList> GetPONumberList()
        {
            List<PONumberList> poNumberList = new List<PONumberList>();

            string existingpoNumberListQuery = string.Empty;
            existingpoNumberListQuery = $@"SELECT DISTINCT PONumber from tblPurchaseOrder where PONumber IS NOT NULL 
                                    AND PONumber <> ''
                                    ORDER BY PONumber DESC";

            poNumberList = _db.GetData<PONumberList, dynamic>(existingpoNumberListQuery, new { }).GetAwaiter().GetResult().ToList();

            return poNumberList;
        }

        // Retrieve Purchase order details.
        public async Task<ResponseModel> GetPurchaseOrderDetails(int poNumber)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (poNumber > 0)
                {
                    string purchaseOrderDetailsQuery = @"SELECT idPurchaseOrder, SupplierName, DeliveryDate, Note, 
                        InvoiceName, Currency, CurrencyRate FROM tblPurchaseOrder WHERE PONumber = @poNumber";

                    var purchaseOrderList = await _db.GetData<PurchaseOrderModel, dynamic>(purchaseOrderDetailsQuery,
                                            new { poNumber });
                    var purchaseOrderDetails = purchaseOrderList.FirstOrDefault();

                    if (purchaseOrderDetails == null)
                    {
                        response.IsError = true;
                        response.Message = "Purchase order not found.";
                        return response;
                    }

                    if (purchaseOrderDetails.idPurchaseOrder != Guid.Empty)
                    {
                        // Get product details
                        string itemsQuery = @"SELECT idPurchaseOrderProduct, MasterSKU, ItemName, Brand, GTIN, Quantity, Price, 
                            Exchange FROM tblPurchaseOrderProduct WHERE idPurchaseOrder = @idPurchaseOrder";

                        var items = await _db.GetData<PurchaseOrderItemViewModel, dynamic>(itemsQuery,
                            new { idPurchaseOrder = purchaseOrderDetails.idPurchaseOrder });

                        purchaseOrderDetails.Items = items.ToList();
                    }

                    response.IsError = false;
                    response.Result = new List<dynamic> { purchaseOrderDetails };
                }
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.Message = ex.Message;
            }
            return response;
        }

        // Update purchase order details.
        public async Task<ResponseModel> EditPurchaseOrder(PurchaseOrderModel model)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                /* Invoice upload and validation */
                string invoiceName = string.Empty;
                IFormFile file = model.InvoiceFile;

                string path = Path.Combine(this._hostEnvironment.WebRootPath, "PurchaseOrderInvoice");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (file != null)
                {
                    string dataFileName = Path.GetFileName(file.FileName);

                    string extension = Path.GetExtension(dataFileName);

                    string[] allowedExtsnions = new string[] { ".pdf" };

                    if (!allowedExtsnions.Contains(extension))
                    {
                        response.Message = "Sorry! This file is not allowed, make sure that file having extension as either.pdf.";
                        throw new Exception(response.Message);
                    }

                    //Save the uploaded file.
                    invoiceName = Path.GetFileName(file.FileName);
                    string invoiceFilePath = Path.Combine(path, invoiceName);
                    using (FileStream stream = new FileStream(invoiceFilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    //Delete uploaded file
                    //System.IO.File.Delete(invoiceFilePath);
                }
                else if (!string.IsNullOrEmpty(model.InvoiceName))      // No new file uploaded then keep existing file name
                {
                    invoiceName = model.InvoiceName;
                }

                /* Update Purchase Order details */
                string updatePOQuery = string.Empty;
                updatePOQuery = @"UPDATE tblPurchaseOrder SET SupplierName = @supplierName, DeliveryDate = @deliveryDate, 
                                Note = @note, InvoiceName = @invoiceName, Currency = @currency, CurrencyRate = @currencyRate,
                                ModifyDate = GETDATE() WHERE PONumber = @poNumber ";
                await _db.ExecuteDML(updatePOQuery, new
                {
                    poNumber = model.PONumber,
                    supplierName = model.SupplierName,
                    deliveryDate = model.DeliveryDate,
                    note = model.Note,
                    invoiceName = invoiceName,
                    currency = model.Currency,
                    currencyRate = model.CurrencyRate
                });

                // Get PO Number id
                var idPONumber = _db.GetSingleValue($@"select idPurchaseOrder from tblPurchaseOrder where PONumber = '{model.PONumber}'");

                // Update each purchase order item(product)
                if (model.Items != null && model.Items.Any())
                {
                    string updateItemQuery = string.Empty;
                    updateItemQuery = @"UPDATE tblPurchaseOrderProduct SET MasterSKU = @masterSKU, 
                            ItemName = @itemName, Brand = @brand, GTIN = @gtin, Quantity = @quantity, 
                            Price = @price, Exchange = @exchange, ModifyDate = GETDATE() 
                            WHERE idPurchaseOrderProduct = @idPurchaseOrderProduct";

                    string insertItemQuery = string.Empty;
                    insertItemQuery = @"INSERT INTO tblPurchaseOrderProduct (idPurchaseOrder, MasterSKU, ItemName, 
                            Brand, GTIN, Quantity, Price, Exchange)
                            VALUES (@idPurchaseOrder, @MasterSKU, @ItemName, @Brand, @GTIN, @Quantity, @Price,
                            @Exchange)";

                    foreach (var item in model.Items)
                    {
                        if (item.idPurchaseOrderProduct != Guid.Empty)   
                        {
                            await _db.ExecuteDML(updateItemQuery, new
                            {
                                idPurchaseOrderProduct = item.idPurchaseOrderProduct,
                                masterSKU = item.MasterSKU,
                                itemName = item.ItemName,
                                brand = item.Brand,
                                gtin = item.GTIN,
                                quantity = item.Quantity,
                                price = item.Price,                               
                                exchange = item.Exchange
                            });
                        }
                        else    
                        {
                            await _db.ExecuteDML(insertItemQuery, new
                            {
                                idPurchaseOrder = idPONumber,
                                item.MasterSKU,
                                item.ItemName,
                                item.Brand,
                                item.GTIN,
                                item.Quantity,
                                item.Price,
                                item.Exchange
                            });
                        }                     
                    }
                }

                response.IsError = false;
                response.Message = "Purchase Order updated successfully.";
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.Message = ex.Message;
            }
            return response;
        }

        // Delete purchase order product.
        public async Task DeleteProduct(string productId)
        {
            await _db.ExecuteDML("DELETE from tblPurchaseOrderProduct WHERE idPurchaseOrderProduct = @idPurchaseOrderProduct", 
                new { idPurchaseOrderProduct = productId });
        }
        /* Edit Purchase order page - End */

        /* Purchase order List page - Start */
        //For return dropdown supplier name list.
        public List<SupplierList> GetSupplierList()
        {
            List<SupplierList> supplierList = new List<SupplierList>();

            string existingSupplierList = string.Empty;
            existingSupplierList = $@"SELECT DISTINCT SupplierName from tblPurchaseOrder where SupplierName IS NOT NULL 
                                    AND SupplierName <> ''";

            supplierList = _db.GetData<SupplierList, dynamic>(existingSupplierList, new { }).GetAwaiter().GetResult().ToList();

            //Add Default Item at First Position.
            supplierList.Insert(0, new SupplierList { SupplierName = "All" });

            return supplierList;
        }

        //For return dropdown brand name list.
        public List<BrandsList> GetBrandList()
        {
            List<BrandsList> brandList = new List<BrandsList>();

            string existingBrandList = string.Empty;
            existingBrandList = $@"SELECT idBrand, Brand FROM tblBrand 
                                    WHERE Brand IS NOT NULL AND Brand <> '' GROUP BY Brand,idBrand";
            brandList = _db.GetData<BrandsList, dynamic>(existingBrandList, new { }).GetAwaiter().GetResult().ToList();

            //Add Default Item at First Position.
            brandList.Insert(0, new BrandsList { Brand = "All" });

            return brandList;
        }

        public List<PONumberList> GetPONumber()
        {
            List<PONumberList> poNumberList = new List<PONumberList>();

            string existingpoNumberListQuery = string.Empty;
            existingpoNumberListQuery = $@"SELECT DISTINCT PONumber from tblPurchaseOrder where PONumber IS NOT NULL 
                                    AND PONumber <> ''
                                    ORDER BY PONumber DESC";

            poNumberList = _db.GetData<PONumberList, dynamic>(existingpoNumberListQuery, new { }).GetAwaiter().GetResult().ToList();

            //Add Default Item at First Position.
            poNumberList.Insert(0, new PONumberList { PONumber = "All" });

            return poNumberList;
        }

       // Method to fetch purchase order data.
        public async Task<ResponseModel> GetPurchaseOrder(string status, string supplierName, string brand, string poNumber)
        {
            ResponseModel response = new ResponseModel();
            List<string> conditions = new List<string>();
            try
            {
                string query = @"
                SELECT 
    po.idPurchaseOrder, 
    po.PONumber, 
    po.SupplierName,   
    ISNULL(po.Status,'Created') as [Status],
    SUM(pop.Quantity) AS Quantity,
    SUM(pop.Quantity * pop.Price) AS Price,
    SUM(pop.Exchange) AS Exchange,
    SUM(ISNULL(t.ReceivedCount, 0)) AS ReceivedCount,
    po.DeliveryDate,
    po.ReceivedDate,
    po.Note,
    po.InvoiceName,
    po.DateAdd   
FROM 
    tblPurchaseOrder po 
    INNER JOIN tblPurchaseOrderProduct pop 
        ON po.idPurchaseOrder = pop.idPurchaseOrder  
    OUTER APPLY (
        SELECT 
            SUM(
                ISNULL(DamageCount, 0) + 
                ISNULL(MissingCount, 0) + 
                ISNULL(AmershamQuantity, 0) + 
                ISNULL(WatfordQuantity, 0)
            ) AS ReceivedCount 
        FROM 
            tblPurchaseOrderProductHistory poph 
        WHERE 
            poph.idPurchaseOrderProduct = pop.idPurchaseOrderProduct
    ) t  
                ";


                if (status != "All")
                {
                    conditions.Add($"pop.Status = '{status}' ");
                }

                if (poNumber != "All")
                {
                    conditions.Add($"po.PONumber = '{poNumber}' ");
                }

                if (supplierName != "All")
                {
                    conditions.Add($"po.SupplierName = '{supplierName}' ");
                }

                //if (brand != "All")
                //{
                //    if (brand.Contains(","))
                //    {
                //        // Handle multiple brands
                //        var brandNames = brand
                //            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                //            .Select(b => $"'{b.Trim().Replace("'", "''")}'"); // Escape each brand safely

                //        string inClause = string.Join(",", brandNames);
                //        conditions.Add($"pop.Brand IN ({inClause}) ");
                //    }
                //    else
                //    {
                //        // Handle single brand
                //        string escapedBrandName = brand.Replace("'", "''");
                //        conditions.Add($"pop.Brand = '{escapedBrandName}' ");
                //    }
                //}

                if (conditions.Any())
                {
                    query = query + ("WHERE " + string.Join(" AND ", conditions) + " ");
                }

                query = query + @"
                GROUP BY 
    po.idPurchaseOrder, 
    po.PONumber,
    po.SupplierName,
    po.Status,
    po.DeliveryDate, 
    po.ReceivedDate,
    po.Note, 
    po.InvoiceName,
    po.DateAdd
ORDER BY 
    po.DateAdd DESC";

                var result = await _db.GetData<PurchaseOrderModel, dynamic>(query, new { });
                response.Result = result;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsError = true;
            }

            return response;
        }

    //    public async Task<ResponseModel> GetPurchaseOrder(string status, string supplierName, string brand, string poNumber)
    //    {
    //        ResponseModel response = new ResponseModel();
    //        List<string> conditions = new List<string>();
    //        try
    //        {
    //            string query =
    //            @$"SELECT po.idPurchaseOrder, pop.idPurchaseOrderProduct, po.PONumber, pop.MasterSKU, 
    //                pop.ItemName, pop.Brand, po.SupplierName, pop.Quantity, (pop.Quantity * pop.Price) AS Price, 
    //                po.Currency, pop.Exchange, po.DeliveryDate, po.Note, pop.Status, pop.ReceivedDate, pop.DamageCount, 
    //                pop.MissingCount, ISNULL(t.ReceivedCount, 0) AS ReceivedCount, pop.IssueDescription, po.InvoiceName,
    //                SUM(CASE WHEN sl.StockLocation = 'Amersham' THEN ISNULL(sl.Quantity,0) ELSE 0 END) AS AmershamQty,
    //                SUM(CASE WHEN sl.StockLocation = 'Watford' THEN ISNULL(sl.Quantity,0) ELSE 0 END) AS WatfordQty ,
    //                SUM(CASE WHEN sl.StockLocation IN ('Amersham','Watford') THEN ISNULL(sl.Quantity,0) ELSE 0 END) AS TotalQty	
    //            FROM tblPurchaseOrder po INNER JOIN tblPurchaseOrderProduct pop ON po.idPurchaseOrder = pop.idPurchaseOrder
    //            INNER JOIN tblMasterSKU m ON pop.MasterSKU = m.SKU
    //            LEFT JOIN tblStockLocation sl ON sl.idMasterSKU = m.idMasterSKU 
    //            OUTER APPLY (SELECT SUM(ISNULL(DamageCount, 0) + ISNULL(MissingCount, 0) + ISNULL(AmershamQuantity, 0) + 
    //            ISNULL(WatfordQuantity, 0)) AS ReceivedCount FROM tblPurchaseOrderProductHistory poph 
				//WHERE poph.idPurchaseOrderProduct = pop.idPurchaseOrderProduct) t ";

    //            if (status != "All")
    //            {
    //                conditions.Add($"pop.Status = '{status}' ");
    //            }

    //            if (poNumber != "All")
    //            {
    //                conditions.Add($"po.PONumber = '{poNumber}' ");
    //            }

    //            if (supplierName != "All")
    //            {
    //                conditions.Add($"po.SupplierName = '{supplierName}' ");
    //            }

    //            if (brand != "All")
    //            {
    //                if (brand.Contains(","))
    //                {
    //                    // Handle multiple brands
    //                    var brandNames = brand
    //                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
    //                        .Select(b => $"'{b.Trim().Replace("'", "''")}'"); // Escape each brand safely

    //                    string inClause = string.Join(",", brandNames);
    //                    conditions.Add($"pop.Brand IN ({inClause}) ");
    //                }
    //                else
    //                {
    //                    // Handle single brand
    //                    string escapedBrandName = brand.Replace("'", "''");
    //                    conditions.Add($"pop.Brand = '{escapedBrandName}' ");
    //                }
    //            }

    //            if (conditions.Any())
    //            {
    //                query = query + ("WHERE " + string.Join(" AND ", conditions) + " ");
    //            }

    //            query = query + @$" GROUP BY po.idPurchaseOrder, pop.idPurchaseOrderProduct, po.PONumber, pop.MasterSKU, 
    //                            pop.ItemName, pop.brand, po.SupplierName, pop.Quantity, pop.Price, po.Currency, 
    //                            pop.Exchange, po.DeliveryDate, po.Note, pop.Status, pop.ReceivedDate, pop.DamageCount, 
    //                            pop.MissingCount, pop.IssueDescription, po.InvoiceName, po.DateAdd , t.ReceivedCount
    //                            order by po.PONumber, po.DateAdd desc";

    //            var result = await _db.GetData<PurchaseOrderModel, dynamic>(query, new { });
    //            response.Result = result;
    //            response.IsError = false;
    //        }
    //        catch (Exception ex)
    //        {
    //            response.Message = ex.Message;
    //            response.IsError = true;
    //        }

    //        return response;
    //    }

        //    /* Purchase order List page - End */

        //    /* Update Purchase order page - Start */
        //    Method to fetch update purchase order item details.
        public PurchaseOrderModel GetUpdatePOItemDetails(string id)
        {
            string query = @"SELECT po.idPurchaseOrder, pop.idPurchaseOrderProduct, po.PONumber, po.SupplierName, 
                            pop.MasterSKU, pop.ItemName, pop.Quantity, pop.DamageCount, pop.MissingCount,
                            pop.ReceivedDate, pop.IssueDescription, ISNULL(t.ReceivedCount, 0) AS ReceivedCount from tblPurchaseOrder po 
                            INNER JOIN tblPurchaseOrderProduct pop ON po.idPurchaseOrder = pop.idPurchaseOrder
							OUTER APPLY (SELECT SUM(ISNULL(DamageCount, 0) + ISNULL(MissingCount, 0) 
							 + ISNULL(AmershamQuantity, 0) + ISNULL(WatfordQuantity, 0)) AS ReceivedCount
							FROM tblPurchaseOrderProductHistory poph 
							WHERE poph.idPurchaseOrderProduct = pop.idPurchaseOrderProduct) t
                            WHERE pop.idPurchaseOrderProduct = @Id";

            var result = _db.GetData<PurchaseOrderModel, dynamic>(query, new { Id = id })
                            .GetAwaiter().GetResult().FirstOrDefault();

            return result ?? new PurchaseOrderModel(); 
        }

        // To update purchase order item.
        public PurchaseOrderModel UpdatePurchaseOrder(PurchaseOrderModel model, string userName)
        {
            string query = string.Empty;
      
            query = @"UPDATE tblPurchaseOrderProduct SET ReceivedDate = @receivedDate, 
                    MissingCount = @missingCount, DamageCount = @damageCount, IssueDescription = @issueDescription,
                    AmershamQuantity = @amershamQuantity, WatfordQuantity = @watfordQuantity,
                    ModifyDate = GetDate() WHERE idPurchaseOrderProduct = @idPurchaseOrderProduct";
            _db.ExecuteDML(query, new 
            {
                idPurchaseOrderProduct = model.idPurchaseOrderProduct,
                status = model.Status,
                receivedDate = model.ReceivedDate,
                missingCount = model.MissingCount,
                damageCount = model.DamageCount,
                issueDescription = model.IssueDescription,
                amershamQuantity = model.AmershamQty,
                watfordQuantity = model.WatfordQty
            });

            // Function call
            if (model.AmershamQty != null || model.WatfordQty != null)
            {
                AddWarehouseQuantity(model.MasterSKU, model.AmershamQty, model.WatfordQty, userName).GetAwaiter().GetResult();
            }
            
            InsertPurchaseOrderProductHistory(model.idPurchaseOrderProduct, model.MasterSKU, model.Quantity,
                 model.Status, model.ReceivedDate, model.DamageCount, model.MissingCount, model.IssueDescription,
                 model.AmershamQty, model.WatfordQty).GetAwaiter().GetResult();

            return model;
        }

        // To Add delievered stock in warehouse stock.
        public async Task<ResponseModel> AddWarehouseQuantity(string sku, string? amershamQuantity, string? watfordQuantity,
            string user)
        {
            ResponseModel response = new ResponseModel();

            masterSKU = sku;
            userName = user;

            try
            {
                var existingStockLocations = await GetExistingStockLocations(masterSKU);
                idMasterSKU = GetMasterSKUId(masterSKU);

                // If no stock location avilable for master SKU add zero stock.
                if (!existingStockLocations.Any())
                {
                    await InsertStockLocation("Amersham", amershamQuantity ?? "0");
                    await InsertStockLocation("Watford", watfordQuantity ?? "0");
                    response.Message = "Warehouse quantity successfully added.";
                    return response;
                }

                await ProcessStockLocations(existingStockLocations, amershamQuantity, watfordQuantity);
                response.Message = "Warehouse quantity successfully added.";
                return response;
            }
            catch (Exception ex)
            {
                return HandleException(response, ex);
            }
        }

        // Process stock location.
        private async Task ProcessStockLocations(IEnumerable<StockLocationModel> existingStockLocations, string? amershamQuantity,
           string? watfordQuantity)
        {
            int currentAmershamQuantity = 0, currentWatfordQuantity = 0;

            var amershamData = existingStockLocations.FirstOrDefault(x => x.StockLocation == "Amersham");
            var watfordData = existingStockLocations.FirstOrDefault(x => x.StockLocation == "Watford");

            currentAmershamQuantity = int.Parse(amershamQuantity ?? "0");
            currentWatfordQuantity = int.Parse(watfordQuantity ?? "0");

            int existingAmershamQuantity = int.Parse(amershamData?.Quantity ?? "0");
            int existingWatfordQuantity = int.Parse(watfordData?.Quantity ?? "0");

            // Insert stock
            if (amershamData == null && !string.IsNullOrEmpty(amershamQuantity))
            {
                await InsertStockLocation("Amersham", amershamQuantity);
            }
            if (watfordData == null && !string.IsNullOrEmpty(watfordQuantity))
            {
                await InsertStockLocation("Watford", watfordQuantity);
            }

            // Update (Increase) stock in existing stock
            if (amershamData != null && currentAmershamQuantity > 0)
            {
                await IncreaseStockLocationQuantity("Amersham", currentAmershamQuantity, amershamData);

                var currentQuantity = currentAmershamQuantity + existingAmershamQuantity;
                var idStockLocation = amershamData.idStockLocation;
                await InsertStockLocationHistory(idStockLocation, amershamData.StockLocation, amershamQuantity, currentQuantity.ToString());
            }

            if (watfordData != null && currentWatfordQuantity > 0)
            {
                await IncreaseStockLocationQuantity("Watford", currentWatfordQuantity, watfordData);

                var currentQuantity = currentWatfordQuantity + existingWatfordQuantity;
                var idStockLocation = watfordData.idStockLocation;
                await InsertStockLocationHistory(idStockLocation, watfordData.StockLocation, watfordQuantity, currentQuantity.ToString());

            }
        }

        private async Task<IEnumerable<StockLocationModel>> GetExistingStockLocations(string sku)
        {
            string query = @"SELECT ms.idMasterSKU, sl.idStockLocation, sl.StockLocation, sl.Quantity FROM tblStockLocation sl 
                    JOIN tblMasterSKU ms ON ms.idMasterSKU = sl.idMasterSKU WHERE ms.SKU = @masterSKU";
            return (await _db.GetData<StockLocationModel, dynamic>(query, new { masterSKU = sku })).ToList();
        }

        // Get existing master SKU ID for the selected master SKU.
        private Guid GetMasterSKUId(string masterSKU)
        {
            var idMasterSKU = _db.GetSingleValue($"select idMasterSKU from tblMasterSKU where sku = '" + masterSKU + "'");
            return Guid.Parse(idMasterSKU);
        }

        // To insert stock location and quantity.
        private async Task InsertStockLocation(string stockLocation, string quantity)
        {
            string query =
            @"INSERT INTO tblStockLocation(idMasterSKU, StockLocation, Quantity, DateAdd, StockAddDate, Username, CompanyName) 
                VALUES(@idMasterSKU, @stockLocation, @quantity, @dateAdd, @stockAddDate, @userName, 'null')";

            var parameters = new
            {
                idMasterSKU,
                stockLocation,
                quantity,
                dateAdd = DateTime.Now,
                stockAddDate = DateTime.Now,
                userName
            };

            await _db.ExecuteDML(query, parameters);
            // After it trigger will execute to insert record on tblStockLocationHistory

            string logMessage = $"{userName} || {DateTime.Now:dd/MM/yyyy hh:mm tt} | {quantity} quantity of {stockLocation} have been added by the user from the website for Purchase order. Current stock quantity: {quantity}.";
            await AddLog(logMessage);
        }

        // To update incease stock location.
        private async Task IncreaseStockLocationQuantity(string stockLocation, int quantityInput, StockLocationModel stockLocationData)
        {
            int existingQuantity = string.IsNullOrEmpty(stockLocationData.Quantity) ? 0 : int.Parse(stockLocationData.Quantity);
            var increaseQuantity = existingQuantity + quantityInput;

            string updateQuery =
                @"UPDATE tblStockLocation SET Quantity = @quantity, StockAddDate = @stockAddDate, Username = @userName
                WHERE idMasterSKU = @idMasterSKU AND StockLocation = @stockLocation";

            await _db.ExecuteDML(updateQuery, new
            {
                quantity = increaseQuantity,
                idMasterSKU,
                stockLocation,
                modifyTime = DateTime.Now,
                stockAddDate = DateTime.Now,
                userName
            });

            string logMessage = $"{userName} || {DateTime.Now:dd/MM/yyyy hh:mm tt} | {quantityInput} quantity of {stockLocation} have been added by the user from the website for Purchase order. Current stock quantity: {increaseQuantity}.";
            await AddLog(logMessage);
        }

        // To insert stock details for the history.
        private async Task InsertStockLocationHistory(object idStockLocation, string stockLocation, string quantity,
            string currentQuantity)
        { 
            string historyQuery =
                @"INSERT INTO tblStockLocationHistory(idStockLocation, MasterSKU, StockLocation, Quantity, DateAdd, 
                TotalQuantity, Username, CompanyName) 
                VALUES(@idStockLocation, @masterSKU, @stockLocation, @quantity, @dateAdd, @totalQuantity, @userName, 'null')";

            await _db.ExecuteDML(historyQuery, new
            {
                idStockLocation,
                masterSKU,
                stockLocation,
                quantity,
                dateAdd = DateTime.Now,
                totalQuantity = currentQuantity,
                userName
            });
        }

        // To insert log message.
        public async Task AddLog(string logMessage)
        {
            string insertLogQuery = @$"INSERT INTO tblLog (idMasterSKU,LogMessage, DateAdd) Values(@idMasterSKU, @logMessage, @dateAdd)";
            await _db.ExecuteDML(insertLogQuery, new
            {
                idMasterSKU,
                logMessage,
                dateAdd = DateTime.Now
            });
        }

        private async Task InsertPurchaseOrderProductHistory(Guid idPurchaseOrderProduct, string masterSKU, int quantity,
                string? status, DateTime? receivedDate, int damageCount, int missingCount, string? issueDescription, 
                string? amershamQuantity, string? watfordQuantity)
        {
            string historyQuery = @"INSERT INTO tblPurchaseOrderProductHistory(idPurchaseOrderProduct, MasterSKU, Quantity, 
                Status, ReceivedDate, DamageCount, MissingCount, IssueDescription, AmershamQuantity, WatfordQuantity)
                VALUES(@idPurchaseOrderProduct, @masterSKU, @quantity, @status, @receivedDate, @damageCount, @missingCount, 
                @issueDescription, @amershamQuantity, @watfordQuantity)";

            await _db.ExecuteDML(historyQuery, new
            {
                idPurchaseOrderProduct,
                masterSKU,
                quantity,
                status,
                receivedDate,
                damageCount,
                missingCount,
                issueDescription, 
                amershamQuantity,
                watfordQuantity,
                dateAdd = DateTime.Now             
            });
        }

        private ResponseModel HandleException(ResponseModel response, Exception ex)
        {
            response.IsError = true;
            response.Message = ex.Message;
            return response;
        }
        /* Update Purchase order page - End */

        /* Inventory claim page - Start */
        // Method to fetch purchase order inventory claim data.
        public async Task<ResponseModel> GetInventoryClaim()
        {
            ResponseModel response = new ResponseModel();
            string query = string.Empty;
            try
            {
                query = @$"SELECT ic.idInventoryClaims, po.PONumber, ic.IssueType, ic.ClaimAmount, ic.Status, 
                        ic.SettlementDate, ic.Note FROM tblInventoryClaims ic 
                        INNER JOIN tblPurchaseOrder po ON ic.idPurchaseOrder = po.idPurchaseOrder";

                var result = await _db.GetData<PurchaseOrderModel, dynamic>(query, new { });
                response.Result = result;
                response.IsError = false;              
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsError = true;
            }            
            return response;
        }

        // Method to update purchase order inventory cliam details.
        public async Task<ResponseModel> UpdateInventoryClaim(PurchaseOrderModel model)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                string query = string.Empty;
                query = @"UPDATE tblInventoryClaims SET IssueType = @issueType, ClaimAmount = @claimAmount, 
                        Status = @status, SettlementDate = @settlementDate, Note = @note, ModifyDate = GetDate() 
                        WHERE idInventoryClaims = @idInventoryClaims";
                await _db.ExecuteDML(query, new
                {
                    idInventoryClaims = model.idInventoryClaims,
                    issueType = model.IssueType,
                    claimAmount = model.ClaimAmount,
                    status = model.Status,
                    settlementDate = model.SettlementDate,
                    note = model.Note
                });

                var data = await GetInventoryClaim();
                response.Result = data.Result;
                response.IsError = false;
                response.Message = "Inventory claim updated successfully.";
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.Message = ex.Message;
            }
            return response;
        }
        /* Inventory claim page - End */

        /* Stock receipt report page - Start */
        // Method to fetch purchase order stock receipt report data.
        public async Task<ResponseModel> GetStockReceiptReportData()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                string query = string.Empty;
                query = @$"SELECT po.PONumber, SUM(ISNULL(pop.DamageCount,0)) AS DamageCount, SUM(ISNULL(pop.MissingCount,0)) AS MissingCount,
                        SUM(pop.Quantity - (ISNULL(pop.MissingCount,0) + ISNULL(pop.DamageCount,0))) AS Quantity,  
                        MAX(pop.ReceivedDate) AS ReceivedDate FROM tblPurchaseOrder po 
                        INNER JOIN tblPurchaseOrderProduct pop ON po.idPurchaseOrder = pop.idPurchaseOrder
                        GROUP BY po.PONumber ORDER BY po.PONumber";

                var result = await _db.GetData<PurchaseOrderModel, dynamic>(query, new { });
                response.Result = result;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsError = true;
            }

            return response;
        }
        /* Stock receipt report page - End */

        /* Stock aging report page - Start */
        // Method to fetch purchase order stock aging report data.
        public async Task<ResponseModel> GetStockAgingReportData(string selectedFilter)
        {
            ResponseModel response = new ResponseModel();
            IEnumerable<PurchaseOrderModel> result = null;

            try
            {
                if (selectedFilter == "Supplier")
                {
                    string query = string.Empty;
                    query = @$"SELECT PO.SupplierName,
	                SUM(CASE WHEN DATEDIFF(DAY, SL.StockDate, GETDATE()) <= 30 THEN SL.Quantity ELSE 0 END) AS Aging_0_30,
                    SUM(CASE WHEN DATEDIFF(DAY, SL.StockDate, GETDATE()) BETWEEN 31 AND 60 THEN SL.Quantity ELSE 0 END) AS Aging_30_60,
                    SUM(CASE WHEN DATEDIFF(DAY, SL.StockDate, GETDATE()) BETWEEN 61 AND 90 THEN SL.Quantity ELSE 0 END) AS Aging_60_90,
                    SUM(CASE WHEN DATEDIFF(DAY, SL.StockDate, GETDATE()) > 90 THEN SL.Quantity ELSE 0 END) AS Aging_90_plus
                    FROM tblPurchaseOrder PO
                    INNER JOIN tblPurchaseOrderProduct POP ON PO.idPurchaseOrder = POP.idPurchaseOrder
                    LEFT JOIN tblStockLedger SL ON SL.MasterSKU = POP.MasterSKU 
                    WHERE SL.IsReduced = 0 AND po.SupplierName IS NOT NULL
                    GROUP BY PO.SupplierName ORDER BY PO.SupplierName";

                    result = await _db.GetData<PurchaseOrderModel, dynamic>(query, new { });
                }
                else if (selectedFilter == "PO Number")
                {
                    string query = string.Empty;
                    query = @$"SELECT PO.PONumber,
	                SUM(CASE WHEN DATEDIFF(DAY, SL.StockDate, GETDATE()) <= 30 THEN SL.Quantity ELSE 0 END) AS Aging_0_30,
                    SUM(CASE WHEN DATEDIFF(DAY, SL.StockDate, GETDATE()) BETWEEN 31 AND 60 THEN SL.Quantity ELSE 0 END) AS Aging_30_60,
                    SUM(CASE WHEN DATEDIFF(DAY, SL.StockDate, GETDATE()) BETWEEN 61 AND 90 THEN SL.Quantity ELSE 0 END) AS Aging_60_90,
                    SUM(CASE WHEN DATEDIFF(DAY, SL.StockDate, GETDATE()) > 90 THEN SL.Quantity ELSE 0 END) AS Aging_90_plus
                    FROM tblPurchaseOrder PO
                    INNER JOIN tblPurchaseOrderProduct POP ON PO.idPurchaseOrder = POP.idPurchaseOrder
                    LEFT JOIN tblStockLedger SL ON SL.MasterSKU = POP.MasterSKU 
                    WHERE SL.IsReduced = 0 AND po.PONumber IS NOT NULL
                    GROUP BY PO.PONumber ORDER BY PO.PONumber";

                    result = await _db.GetData<PurchaseOrderModel, dynamic>(query, new { });
                }

                response.Result = result;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsError = true;
            }

            return response;
        }
        /* Stock aging report page - End */


        // Retrieve Purchase order details.
        public async Task<PurchaseOrderModel> GetPurchaseOrderDetailsList(string id)
        {
            PurchaseOrderModel response = new PurchaseOrderModel();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    string purchaseOrderDetailsQuery = @"SELECT idPurchaseOrder,PONumber, SupplierName, DeliveryDate, Note, 
                        InvoiceName, Currency, CurrencyRate,ISNULL([Status],'Created') AS  [Status], ISNULL(ReceivedDate,GETDATE()) AS ReceivedDate FROM tblPurchaseOrder WHERE idPurchaseOrder = @id";

                    var purchaseOrderList = await _db.GetData<PurchaseOrderModel, dynamic>(purchaseOrderDetailsQuery,
                                            new { id });
                    response = purchaseOrderList.FirstOrDefault();

                    

                    if (response?.idPurchaseOrder != Guid.Empty)
                    {
                        // Get product details
                        string itemsQuery = @"SELECT 
	pop.idPurchaseOrderProduct,
	pop.idPurchaseOrder, 
	pop.GTIN,
	pop.MasterSKU,
	pop.ItemName,
	pop.Quantity,
	t.ReceivedCount,
	0 AmershamQuantity,
	0 WatfordQuantity,
	0 DamageCount, 
	0 MissingCount, 
	pop.IssueDescription 
	FROM
	tblPurchaseOrderProduct pop 
    INNER JOIN tblMasterSKU m ON pop.MasterSKU = m.SKU and pop.idPurchaseOrder = @idPurchaseOrder
        OUTER APPLY (
    SELECT 
        SUM(
        ISNULL(DamageCount, 0) + ISNULL(MissingCount, 0) + ISNULL(AmershamQuantity, 0) + ISNULL(WatfordQuantity, 0)
        ) AS ReceivedCount 
    FROM 
        tblPurchaseOrderProductHistory poph 
    WHERE 
        poph.idPurchaseOrderProduct = pop.idPurchaseOrderProduct
    ) t  ";

                        var items = await _db.GetData<PurchaseOrderItemListModel, dynamic>(itemsQuery,
                            new { idPurchaseOrder = response?.idPurchaseOrder });

                        response.ItemsList = items.ToList();
                    }
                  
                }
            }
            catch (Exception ex)
            {
                
            }
            return response;
        }

        public PurchaseOrderModel UpdatePurchaseOrderList(PurchaseOrderModel model, string userName)
        {
            if (model.idPurchaseOrder != Guid.Empty && model.ItemsList != null && model.ItemsList.Where(x=>x.IsValid).ToList().Count > 0)
            {
                /* Update Purchase Order  */
                string updatePOQuery = string.Empty;
                updatePOQuery = @"UPDATE tblPurchaseOrder SET Status = @status, ReceivedDate = @receivedDate,
                                ModifyDate = GETDATE() WHERE idPurchaseOrder = @idPurchaseOrder ";
                _db.ExecuteDML(updatePOQuery, new
                {
                    status = model.Status,
                    receivedDate = model.ReceivedDate,
                    idPurchaseOrder = model.idPurchaseOrder
                }).GetAwaiter().GetResult();

                /* Update Purchase Order List */
                foreach (var item in model.ItemsList.Where(x => x.IsValid).ToList())
                {
                    string query = string.Empty;

                    query = @"UPDATE tblPurchaseOrderProduct SET ReceivedDate = @receivedDate, 
                    MissingCount = @missingCount, DamageCount = @damageCount, IssueDescription = @issueDescription,
                    AmershamQuantity = @amershamQuantity, WatfordQuantity = @watfordQuantity,
                    ModifyDate = GetDate() WHERE idPurchaseOrderProduct = @idPurchaseOrderProduct";
                    _db.ExecuteDML(query, new
                    {
                        idPurchaseOrderProduct = item.idPurchaseOrderProduct,
                        status = model.Status,
                        receivedDate = model.ReceivedDate,
                        missingCount = item.MissingCount,
                        damageCount = item.DamageCount,
                        issueDescription = item.IssueDescription,
                        amershamQuantity = item.AmershamQuantity,
                        watfordQuantity = item.WatfordQuantity
                    });

                    // Function call
                    if (item.AmershamQuantity != null || item.WatfordQuantity != null)
                    {
                        AddWarehouseQuantity(item.MasterSKU, item.AmershamQuantity.ToString(), item.WatfordQuantity.ToString(), userName).GetAwaiter().GetResult();
                    }

                    InsertPurchaseOrderProductHistory(item.idPurchaseOrderProduct, item.MasterSKU, item.Quantity,
                         model.Status, model.ReceivedDate, item.DamageCount, item.MissingCount, item.IssueDescription,
                         item.AmershamQuantity.ToString(), item.WatfordQuantity.ToString()).GetAwaiter().GetResult();
                }
            }
            return model;
        }

    }
}
