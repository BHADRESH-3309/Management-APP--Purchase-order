using WebManagementApp.Models;
using WebManagementApp.DataAccess.DbAccess;
using WebManagementApp.DataAccess.Interfaces;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Hosting;
using System.Drawing;


namespace WebManagementApp.DataAccess.Services
{
    public class MasterInventoryService : IMasterInventoryService
    {
        private readonly ISqlDataAccess _db;
        Guid idMasterSKU = Guid.Empty;
        string masterSKU = string.Empty, userName = string.Empty;

        public MasterInventoryService(ISqlDataAccess db)
        {
            _db = db;
        }

        // Method to fetch master inventory data.
        public async Task<ResponseModel> GetMasterInventory(bool isDamagedQuantity = false, bool isMarketplaceQuantity = false)
        {
            ResponseModel response = new ResponseModel();

            var data = await _db.GetDataSP<MasterInventoryModel, dynamic>("spGetMasterInventoryData", 
                new { isDamagedQuantity = isDamagedQuantity, isMarketplaceQuantity = isMarketplaceQuantity });
            response.Result = data;

            return response;
        }
        //GetMasterInventoryBySKU

        public async Task<ResponseModel> GetMasterInventoryBySKU(string sku, bool isDamagedQuantity, bool isMarketplaceQuantity)
        {
            ResponseModel response = new ResponseModel();

            var data = await _db.GetDataSP<MasterInventoryModel, dynamic>("spGetMasterInventoryDataBySKU", 
                new { sku = sku, isDamagedQuantity = isDamagedQuantity, isMarketplaceQuantity = isMarketplaceQuantity });
            response.Result = data;

            return response;
        }
        //GetMasterInventoryBySKU

        public async Task<ResponseModel> GetMasterInventoryByMappingSKU(string marketSku, bool isDamagedQuantity, bool isMarketplaceQuantity)
        {
            ResponseModel response = new ResponseModel();

            var data = await _db.GetDataSP<MasterInventoryModel, dynamic>("spGetMasterInventoryByMappingSKU", 
                new { marketSku = marketSku, isDamagedQuantity = isDamagedQuantity, isMarketplaceQuantity = isMarketplaceQuantity });
            response.Result = data;

            return response;
        }

        // Method to fetch log message for a selected master SKU.
        public async Task<ResponseModel> GetMasterInventoryLog(string masterSKU)
        {
            ResponseModel response = new ResponseModel();
            string getExistingLogQuery = string.Empty;
            getExistingLogQuery =
                $@"SELECT TOP 20 LogMessage, DateAdd FROM tblLog 
				WHERE idMasterSKU = (SELECT idMasterSKU FROM tblMasterSKU WHERE SKU = @masterSKU)
				ORDER BY DateAdd DESC;";

            var data = await _db.GetData<StockLocationModel, dynamic>(getExistingLogQuery, new { masterSKU = masterSKU });

            response.Result = data;
            return response;
        }

        // Method
        public async Task<ResponseModel> AddWarehouseQuantity(string sku, string? amershamQuantity, string? watfordQuantity,
            decimal? productCost, string supplierName, string user)
        {
            ResponseModel response = new ResponseModel();

            masterSKU = sku;
            userName = user;
            string cost = productCost?.ToString() ?? "0";

            try
            {
                var existingStockLocations = await GetExistingStockLocations(masterSKU);
                idMasterSKU = GetMasterSKUId(masterSKU);

                if (!existingStockLocations.Any())
                {
                    await InsertStockLocationIfPresent("Amersham", amershamQuantity ?? "0", cost, supplierName);
                    await InsertStockLocationIfPresent("Watford", watfordQuantity ?? "0", cost, supplierName);                  
                    response.Message = "Inventory warehouse quantity successfully added.";
                    return response;
                }

                await ProcessStockLocations(existingStockLocations, amershamQuantity, watfordQuantity, cost, supplierName);
                response.Message = "Inventory warehouse quantity successfully added.";
                return response;
            }
            catch (Exception ex)
            {
                return HandleException(response, ex);
            }
        }

        // Method
        public async Task<ResponseModel> ReduceWarehouseQuantity(string sku, string? amershamQuantity, string? watfordQuantity, string reduceQuantityReason, string user)
        {
            ResponseModel response = new ResponseModel();
            masterSKU = sku;
            userName = user;

            try 
            {
                var existingStockLocations = await GetExistingStockLocations(masterSKU);
                idMasterSKU = GetMasterSKUId(masterSKU);

                bool isAnyLocationStockReduce = await ReduceStockForLocation("Amersham", amershamQuantity, existingStockLocations, reduceQuantityReason);
                isAnyLocationStockReduce |= await ReduceStockForLocation("Watford", watfordQuantity, existingStockLocations, reduceQuantityReason);

                if (!isAnyLocationStockReduce)
                {
                    response.IsError = true;
                    response.Message = "Stock cannot be reduced as store locations quantity does not exist or input quantity is greater than existing quantity.";
                }
                else
                {
                    response.Message = "Inventory quantity successfully reduced.";
                }
            }
            catch (Exception ex)
            {
                return HandleException(response, ex);
            }
            return response;
        }       

        // Method
        public async Task<ResponseModel> AddWarehouseDamagedQuantity(string sku, string? amershamDamagedQuantity,
            string? watfordDamagedQuantity, string user)
        {
            ResponseModel response = new ResponseModel();
            masterSKU = sku;
            userName = user;

            try
            {
                var existingStockLocations = await GetExistingStockLocations(masterSKU);
                idMasterSKU = GetMasterSKUId(masterSKU);

                if (!existingStockLocations.Any())
                {
                    response.IsError = true;
                    response.Message = "Damaged stock cannot be reduced as both store locations quantity does not exist in the system!";
                    return response;
                }

                bool isAnyLocationDamagedStockAdded = await UpdateDamagedStockForLocation("Amersham", amershamDamagedQuantity,
                existingStockLocations);
                isAnyLocationDamagedStockAdded |= await UpdateDamagedStockForLocation("Watford", watfordDamagedQuantity,
                    existingStockLocations);

                if (!isAnyLocationDamagedStockAdded)
                {
                    response.IsError = true;
                    response.Message = "Damaged stock cannot be add as store input location quantity does not exist in the system!";
                }
                else
                {
                    response.Message = "Inventory warehouse damaged quantity successfully added.";
                }
            }
            catch (Exception ex)
            {
                return HandleException(response, ex);
            }

            return response;
        }

        // Method
        public async Task<ResponseModel> ReduceWarehouseDamagedQuantity(string sku, string? amershamDamagedQuantity,
            string? watfordDamagedQuantity, string user)
        {
            ResponseModel response = new ResponseModel();
            masterSKU = sku;
            userName = user;

            try
            {
                var existingStockLocations = await GetExistingStockLocations(masterSKU);
                idMasterSKU = GetMasterSKUId(masterSKU);

                if (!existingStockLocations.Any())
                {
                    response.IsError = true;
                    response.Message = "Stock cannot be reduced as both store locations damaged quantity does not exist in the system!";
                    return response;
                }

                bool isDamagedQuantityAdded = await ReduceDamagedStockForLocation("Amersham", amershamDamagedQuantity, existingStockLocations);
                isDamagedQuantityAdded |= await ReduceDamagedStockForLocation("Watford", watfordDamagedQuantity, existingStockLocations);

                if (!isDamagedQuantityAdded)
                {
                    response.IsError = true;
                    response.Message = "Stock cannot be reduced as store locations damaged quantity does not exist or input quantity is greater than existing quantity.";
                }
                else
                {
                    response.Message = "Inventory warehouse damaged quantity successfully reduced.";
                }
            }
            catch (Exception ex)
            {
                return HandleException(response, ex);
            }

            return response;
        }

        private async Task InsertStockLocationIfPresent(string stockLocation, string? quantity, string costPrice, string supplierName)
        {
            /* Removed the below comment because of add 0 quantity entry with user-added cost price for all stock location. */
            //if (!string.IsNullOrEmpty(quantity))
            //{
            string query =
                   @"INSERT INTO tblStockLocation(idMasterSKU, StockLocation, Quantity, CostPrice, AvgCostPrice, SupplierName, 
                    DateAdd, StockAddDate, Username) VALUES(@idMasterSKU, @stockLocation, @quantity, @costPrice, @avgCostPrice, 
                    @supplierName, @dateAdd, @stockAddDate, @userName)";

            var parameters = new
            {
                idMasterSKU,
                stockLocation,
                quantity,
                costPrice,
                avgCostPrice = costPrice,
                supplierName,
                dateAdd = DateTime.Now,
                stockAddDate = DateTime.Now,
                userName
            };

            await _db.ExecuteDML(query, parameters);
            // After it trigger will execute to insert record on tblStockLocationHistory

            string logMessage = $"{userName} || {DateTime.Now:dd/MM/yyyy hh:mm tt} | {quantity} quantity of {stockLocation} have been added by the user from the website for {supplierName}. Current stock quantity: {quantity}.";
            await AddLog(logMessage);
            //}
        }

        private async Task<bool> ReduceStockForLocation(string location, string? quantity, IEnumerable<StockLocationModel> stockLocations, string reduceQuantityReason)
        {
            if (string.IsNullOrEmpty(quantity)) return false;

            var locationData = stockLocations.FirstOrDefault(x => x.StockLocation == location);
            if (locationData != null && locationData?.Quantity != null)
            {
                if (int.Parse(locationData.Quantity) <= 0) return false;

                var reduceQuantity = int.Parse(locationData.Quantity) - int.Parse(quantity);
                if (reduceQuantity < 0) return false;

                await ReduceStockLocationQuantity(quantity, reduceQuantityReason, locationData);
                return true;
            }
            return false;
        }

        private async Task<bool> UpdateDamagedStockForLocation(string location, string? damagedQuantity, IEnumerable<StockLocationModel> stockLocations)
        {
            var locationData = stockLocations.FirstOrDefault(x => x.StockLocation == location);
            if (!string.IsNullOrEmpty(damagedQuantity) && locationData != null)
            {
                await IncreaseStockLocationDamagedQuantity(damagedQuantity, locationData);
                return true;
            }
            return false;
        }

        private async Task<bool> ReduceDamagedStockForLocation(string location, string? damagedQuantity, IEnumerable<StockLocationModel> stockLocations)
        {
            if (string.IsNullOrEmpty(damagedQuantity)) return false;

            var locationData = stockLocations.FirstOrDefault(x => x.StockLocation == location);
            if (locationData != null && locationData?.DamagedQuantity != null)
            {
                if (int.Parse(locationData.DamagedQuantity) <= 0) return false;

                var reduceDamagedQuantity = int.Parse(locationData.DamagedQuantity) - int.Parse(damagedQuantity);
                if (reduceDamagedQuantity < 0) return false;

                await ReduceStockLocationDamagedQuantity(damagedQuantity, locationData);
                return true;
            }
            return false;
        }

        private async Task InsertStockLocation(string stockLocation, string quantity, string costPrice, string supplierName,
            string avgCostPrice)
        {
            string query =
                @"INSERT INTO tblStockLocation(idMasterSKU, StockLocation, Quantity, CostPrice, SupplierName, DateAdd, 
                AvgCostPrice, StockAddDate, Username) VALUES(@idMasterSKU, @stockLocation, @quantity, @costPrice, 
                @supplierName, @dateAdd, @avgCostPrice, @stockAddDate, @userName)";

            var parameters = new
            {
                idMasterSKU,
                stockLocation,
                quantity,
                costPrice,
                supplierName,
                dateAdd = DateTime.Now,
                AvgCostPrice = avgCostPrice,
                stockAddDate = DateTime.Now,
                userName
            };

            await _db.ExecuteDML(query, parameters);
            // After it trigger will execute to insert record on tblStockLocationHistory

            string logMessage = $"{userName} || {DateTime.Now:dd/MM/yyyy hh:mm tt} | {quantity} quantity of '{stockLocation}' have been added by the user from the website for {supplierName}. Current stock quantity: {quantity}.";
            await AddLog(logMessage);
        }

        private async Task IncreaseStockLocationQuantity(string stockLocation, int quantityInput, string costPrice, string supplierName,
            StockLocationModel stockLocationData)
        {
            int existingQuantity = string.IsNullOrEmpty(stockLocationData.Quantity) ? 0 : int.Parse(stockLocationData.Quantity);
            var increaseQuantity = existingQuantity + quantityInput;
            string damagedQuantity = stockLocationData.DamagedQuantity;

            string updateQuery =
                @"UPDATE tblStockLocation 
                SET Quantity = @quantity, DamagedQuantity = @damagedQuantity, CostPrice = @costPrice, ModifyTime = @modifyTime, 
                SupplierName = @supplierName, StockAddDate = @stockAddDate, Username = @userName
                WHERE idMasterSKU = @idMasterSKU AND StockLocation = @stockLocation";

            await _db.ExecuteDML(updateQuery, new
            {
                quantity = increaseQuantity,
                damagedQuantity,
                costPrice,
                idMasterSKU,
                stockLocation,
                modifyTime = DateTime.Now,
                supplierName,
                stockAddDate = DateTime.Now,
                userName
            });

            string logMessage = $"{userName} || {DateTime.Now:dd/MM/yyyy hh:mm tt} | {quantityInput} quantity of {stockLocation} have been added by the user from the website for {supplierName}. Current stock quantity: {increaseQuantity}.";
            await AddLog(logMessage);
        }

        private async Task IncreaseStockLocationDamagedQuantity(string damagedQuantityInput, StockLocationModel stockLocationData)
        {
            int existingDamagedQuantity = string.IsNullOrEmpty(stockLocationData.DamagedQuantity) ? 0 : int.Parse(stockLocationData.DamagedQuantity);
            var increaseDamagedQuantity = existingDamagedQuantity + int.Parse(damagedQuantityInput);

            string stockLocation = stockLocationData.StockLocation;

            string updateQuery =
                @"UPDATE tblStockLocation 
                SET DamagedQuantity = @damagedQuantity, ModifyTime = @modifyTime, Username = @userName
                WHERE idMasterSKU = @idMasterSKU AND StockLocation = @stockLocation";

            await _db.ExecuteDML(updateQuery, new
            {
                damagedQuantity = increaseDamagedQuantity,
                modifyTime = DateTime.Now,
                idMasterSKU,
                stockLocation,
                userName
            });

            string logMessage = $"{userName} || {DateTime.Now:dd/MM/yyyy hh:mm tt} | {damagedQuantityInput} damaged quantity of {stockLocation} have been added by the user from the website. Current damaged stock quantity: {increaseDamagedQuantity}.";
            await AddLog(logMessage);
        }

        private async Task ReduceStockLocationQuantity(string quantityInput, string reason, StockLocationModel stockLocationData)
        {
            int existingQuantity = string.IsNullOrEmpty(stockLocationData.Quantity) ? 0 : int.Parse(stockLocationData.Quantity);
            var reduceQuantity = existingQuantity - int.Parse(quantityInput); //Condition to manage negative

            string damagedQuantity = stockLocationData.DamagedQuantity;
            string stockLocation = stockLocationData.StockLocation;
            string costPrice = stockLocationData.CostPrice;
            string avgCostPrice = stockLocationData.AvgCostPrice;

            string updateQuery =
                @"UPDATE tblStockLocation 
                SET Quantity = @quantity, DamagedQuantity = @damagedQuantity, CostPrice = @costPrice, AvgCostPrice = @avgCostPrice, 
                ModifyTime = @modifyTime, Username = @userName 
                WHERE idMasterSKU = @idMasterSKU AND StockLocation = @stockLocation";

            await _db.ExecuteDML(updateQuery, new
            {
                quantity = reduceQuantity,
                damagedQuantity,
                costPrice,
                avgCostPrice,
                idMasterSKU,
                stockLocation,
                modifyTime = DateTime.Now,
                userName
            });

            var idStockLocation = _db.GetSingleValue($@"SELECT idStockLocation FROM tblStockLocation WHERE idMasterSKU = '{idMasterSKU}' AND StockLocation = '{stockLocation}'");
            await InsertStockLocationHistory(idStockLocation, stockLocation, quantityInput, damagedQuantity, costPrice, reason, null,
                stockLocationData.AvgCostPrice, reduceQuantity.ToString());

            string logMessage = $"{userName} || {DateTime.Now:dd/MM/yyyy hh:mm tt} | {quantityInput} quantity of {stockLocation} have been reduced by the user from the website. Current stock quantity: {reduceQuantity}.";
            await AddLog(logMessage);
        }

        private async Task ReduceStockLocationDamagedQuantity(string damagedQuantityInput, StockLocationModel stockLocationData)
        {
            int existingDamagedQuantity = string.IsNullOrEmpty(stockLocationData.DamagedQuantity) ? 0 : int.Parse(stockLocationData.DamagedQuantity);
            var reduceDamagedQuantity = existingDamagedQuantity - int.Parse(damagedQuantityInput);

            string stockLocation = stockLocationData.StockLocation;

            string updateQuery =
                @"UPDATE tblStockLocation 
                SET DamagedQuantity = @damagedQuantity, ModifyTime = @modifyTime , Username = @userName
                WHERE idMasterSKU = @idMasterSKU AND StockLocation = @stockLocation";

            await _db.ExecuteDML(updateQuery, new
            {
                damagedQuantity = reduceDamagedQuantity,
                modifyTime = DateTime.Now,
                idMasterSKU,
                stockLocation,
                userName
            });

            string logMessage = $"{userName} || {DateTime.Now:dd/MM/yyyy hh:mm tt} | {damagedQuantityInput} damaged quantity of {stockLocation} have been reduced by the user from the website. Current damaged stock quantity: {reduceDamagedQuantity}";
            await AddLog(logMessage);
        }

        private async Task ProcessStockLocations(IEnumerable<StockLocationModel> existingStockLocations, string? amershamQuantity,
            string? watfordQuantity, string costPrice, string supplierName)
        {
            // Calculate average cost
            async Task<string> ProcessLocation(string locationName, string? locationQuantity, StockLocationModel? locationData,
                float prevCalculatedAvgCost, int previousTotalQuantity)
            {
                string formattedAvgCost = string.Empty;

                // When both location quantity added than consider previous calculate avg cost as previousCost
                float previousAvgCost = prevCalculatedAvgCost > 0 ? prevCalculatedAvgCost : float.Parse(locationData?.AvgCostPrice ?? "0");

                int currentQuantity = int.Parse(locationQuantity ?? "0");
                float currentCost = float.Parse(costPrice ?? "0");

                float prevQuantityIntoAvgCost = previousTotalQuantity * previousAvgCost;
                float currentQuantityIntoCost = currentQuantity * currentCost;
                float previousTotalQuantityPlusCurrentQuantity = previousTotalQuantity + currentQuantity;

                //var avgCost = ((previousTotalQuantity * previousAvgCost) + (currentQuantity * currentCost) / (previousTotalQuantity + currentQuantity));
                var avgCost = ((prevQuantityIntoAvgCost + currentQuantityIntoCost) / previousTotalQuantityPlusCurrentQuantity);
                formattedAvgCost = avgCost.ToString("F2");

                if (locationData != null && currentQuantity > 0)
                    await IncreaseStockLocationQuantity(locationName, currentQuantity, costPrice, supplierName, locationData);

                return formattedAvgCost;
            }

            int currentAmershamQuantity = 0, currentWatfordQuantity = 0;
            string amershamAvgCost = string.Empty; string watfordAvgCost = string.Empty;

            var amershamData = existingStockLocations.FirstOrDefault(x => x.StockLocation == "Amersham");
            var watfordData = existingStockLocations.FirstOrDefault(x => x.StockLocation == "Watford");

            currentAmershamQuantity = int.Parse(amershamQuantity ?? "0");
            currentWatfordQuantity = int.Parse(watfordQuantity ?? "0");

            int existingAmershamQuantity = int.Parse(amershamData?.Quantity ?? "0");
            int existingWatfordQuantity = int.Parse(watfordData?.Quantity ?? "0");

            // Process Amersham and Watford stock locations
            float prevCalculatedAvgCost = 0;
            int previousTotalQuantity = existingAmershamQuantity + existingWatfordQuantity;

            amershamAvgCost = await ProcessLocation("Amersham", amershamQuantity, amershamData, prevCalculatedAvgCost, previousTotalQuantity);
            prevCalculatedAvgCost = float.Parse(amershamAvgCost);
            previousTotalQuantity += currentAmershamQuantity;   // Add current amersham quantity count on total

            watfordAvgCost = await ProcessLocation("Watford", watfordQuantity, watfordData, prevCalculatedAvgCost, previousTotalQuantity);

            if (amershamData == null && !string.IsNullOrEmpty(amershamQuantity))
            {
                await InsertStockLocation("Amersham", amershamQuantity, costPrice, supplierName, amershamAvgCost);
            }
            if (watfordData == null && !string.IsNullOrEmpty(watfordQuantity))
            {
                await InsertStockLocation("Watford", watfordQuantity, costPrice, supplierName, watfordAvgCost);
            }

            string updateQuery = @"UPDATE tblStockLocation SET AvgCostPrice = @avgCostPrice, ModifyTime = @modifyTime, 
                                   Username = @userName WHERE idMasterSKU = @idMasterSKU";
            if (amershamData != null && !string.IsNullOrEmpty(amershamQuantity))
            {
                var currentQuantity = currentAmershamQuantity + existingAmershamQuantity;
                var idStockLocation = amershamData.idStockLocation;
                await InsertStockLocationHistory(idStockLocation, amershamData.StockLocation, amershamQuantity, amershamData.DamagedQuantity, costPrice, null, supplierName, amershamAvgCost, currentQuantity.ToString());

                await _db.ExecuteDML(updateQuery, new
                {
                    avgCostPrice = amershamAvgCost,
                    modifyTime = DateTime.Now,
                    idMasterSKU,
                    userName
                });
            }
            if (watfordData != null && !string.IsNullOrEmpty(watfordQuantity))
            {
                var currentQuantity = currentWatfordQuantity + existingWatfordQuantity;
                var idStockLocation = watfordData.idStockLocation;
                await InsertStockLocationHistory(idStockLocation, watfordData.StockLocation, watfordQuantity, watfordData.DamagedQuantity, costPrice, null, supplierName, watfordAvgCost, currentQuantity.ToString());

                await _db.ExecuteDML(updateQuery, new
                {
                    avgCostPrice = watfordAvgCost,
                    modifyTime = DateTime.Now,
                    idMasterSKU,
                    userName
                });
            }
        }

        private async Task InsertStockLocationHistory(object idStockLocation, string stockLocation, string quantity,
            string? damagedQuantity, string costPrice, string? reduceReason, string? supplierName, string avgCostPrice,
            string currentQuantity)
        {
            string historyQuery =
                @"INSERT INTO tblStockLocationHistory(idStockLocation, MasterSKU, StockLocation, Quantity, DamagedQuantity, 
                CostPrice, DateAdd, ReduceReason, SupplierName, AvgCostPrice, TotalQuantity, Username) 
                VALUES(@idStockLocation, @masterSKU, @stockLocation, @quantity, @damagedQuantity, @costPrice, @dateAdd, 
                @reduceReason, @supplierName, @avgCostPrice, @totalQuantity, @userName)";

            await _db.ExecuteDML(historyQuery, new
            {
                idStockLocation,
                masterSKU,
                stockLocation,
                quantity,
                damagedQuantity,
                costPrice,
                dateAdd = DateTime.Now,
                reduceReason,
                supplierName,
                avgCostPrice,
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

        // Get existing master SKU ID for the selected master SKU.
        private Guid GetMasterSKUId(string masterSKU)
        {
            var idMasterSKU = _db.GetSingleValue($"select idMasterSKU from tblMasterSKU where sku = '" + masterSKU + "'");
            return Guid.Parse(idMasterSKU);
        }

        private async Task<IEnumerable<StockLocationModel>> GetExistingStockLocations(string sku)
        {
            string query = @"SELECT ms.idMasterSKU, sl.idStockLocation, sl.StockLocation, sl.Quantity, sl.DamagedQuantity, 
                    sl.CostPrice, sl.AvgCostPrice FROM tblStockLocation sl JOIN tblMasterSKU ms 
                    ON ms.idMasterSKU = sl.idMasterSKU WHERE ms.SKU = @masterSKU";
            return (await _db.GetData<StockLocationModel, dynamic>(query, new { masterSKU = sku })).ToList();
        }

        private ResponseModel HandleException(ResponseModel response, Exception ex)
        {
            response.IsError = true;
            response.Message = ex.Message;
            return response;
        }

        public async Task<ResponseModel> GetStockLocationHistoryData(string masterSKU)
        {
            ResponseModel response = new ResponseModel();

            string getExistingLogQuery = string.Empty;
            getExistingLogQuery =
                @"SELECT idStockLocationHistory, StockLocation, Quantity, CostPrice, SupplierName, DateAdd 
                FROM tblStockLocationHistory WHERE MasterSKU = @masterSKU AND IsOrderProcessHistory <> 1 
                ORDER BY DateAdd DESC";

            var data = await _db.GetData<StockLocationHistoryModel, dynamic>(getExistingLogQuery, new { masterSKU });

            response.Result = data;
            return response;
        }

        public async Task<string[]> GetSupplierNames()
        {
            string query = @"SELECT DISTINCT SupplierName 
                            FROM tblStockLocationHistory
                            WHERE SupplierName IS NOT NULL AND SupplierName <> ''";
            var data = await _db.GetData<StockLocationHistoryModel, dynamic>(query, new { });

            return data.Select(x => x.SupplierName).ToArray();
        }

        public async Task AddMasterInventoryShippingFees(string masterSKU, decimal shippingFees)
        {
            idMasterSKU = GetMasterSKUId(masterSKU);

            // Call SP to insert/update fees and insert fees history
            string sp = $"EXEC spAddUpdateMasterSKUShippingFee @idMasterSKU = '{idMasterSKU}', @ShippingFee = {shippingFees}";
            await _db.ExecuteDML(sp, new { });
        }

        // Method to add product cost.
        public async Task<ResponseModel> AddProductCost(string sku, string productCost, string? date, string user)
        {
            ResponseModel response = new ResponseModel();
            masterSKU = sku;
            userName = user;
            bool IsCostUpdated = true;
            int quantity = 0 ;

            try
            {
                idMasterSKU = GetMasterSKUId(masterSKU);
                var existingStockLocations = await GetExistingStockLocations(masterSKU);

                if (!existingStockLocations.Any())
                {                 
                    await InsertStockLocationIfCostPresent("Amersham",  productCost, date, "0");
                    await InsertStockLocationIfCostPresent("Watford", productCost, date, "0");
                }
                else
                {
                    var stockLocationList = (await _db.GetData<StockLocationModel, dynamic>(@"SELECT TOP 1  
                            idStockLocation, StockLocation from tblStockLocation
                            WHERE idMasterSKU = @idMasterSKU", new { idMasterSKU })).ToList();

                    string historyQuery = @"INSERT INTO tblStockLocationHistory(idStockLocation, MasterSKU, StockLocation, 
                        CostPrice, DateAdd, IsCostUpdated, Quantity, userName) 
                        VALUES(@idStockLocation, @masterSKU, @stockLocation, @costPrice, @dateAdd, @isCostUpdated, @quantity,
                        @userName)";

                    var item = stockLocationList.FirstOrDefault();
                    if (item != null) 
                    { 
                        await _db.ExecuteDML(historyQuery, new
                        {
                            idStockLocation = item.idStockLocation,
                            masterSKU,
                            stockLocation = item.StockLocation,
                            costPrice = productCost,
                            dateAdd = date,
                            isCostUpdated = IsCostUpdated,
                            quantity = quantity,
                            userName
                        });
                    }

                    string logMessage = $"{userName} || {DateTime.Now:dd/MM/yyyy hh:mm tt} | {productCost} product cost have been added by the user from the website. Current Average cost: {productCost}.";
                    await AddLog(logMessage);
                }       
                response.Message = "Inventory warehouse product cost successfully added.";
            }
            catch (Exception ex)
            {
                return HandleException(response, ex);
            }
            return response;
        }

        // Insert product cost.
        private async Task InsertStockLocationIfCostPresent(string stockLocation, string costPrice, string date, string? quantity)
        {
            string query =
                   @"INSERT INTO tblStockLocation(idMasterSKU, StockLocation, CostPrice, AvgCostPrice, DateAdd, Quantity, Username) 
                   VALUES(@idMasterSKU, @stockLocation, @costPrice, @avgCostPrice, @dateAdd, @quantity, @userName)";

            var parameters = new
            {
                idMasterSKU,
                stockLocation,
                costPrice,
                avgCostPrice = costPrice,
                dateAdd = date,
                quantity,
                userName
            };

            await _db.ExecuteDML(query, parameters);
            // After it trigger will execute to insert record on tblStockLocationHistory

            string logMessage = $"{userName} || {DateTime.Now:dd/MM/yyyy hh:mm tt} | {costPrice} product cost have been added by the user from the website. Current Average cost: {costPrice}.";
            await AddLog(logMessage);
        }

        public async Task<ResponseModel> UpdateCostAndDate(string idStockLocationHistory, decimal costprice, bool isDateChange, 
            DateTime newDate, string user)
        {
            ResponseModel response = new ResponseModel();
            userName = user;
            try
            {
                if (isDateChange)
                {
                    // Update Quantity and DateAdd both
                    string updateQuantityAndDateQuery = @"UPDATE tblStockLocationHistory SET CostPrice = @CostPrice, 
                            DateAdd = @DateAdd, ModifyTime = GETDATE(), IsCostUpdated = 1 , Username = @userName
                            WHERE idStockLocationHistory = @idStockLocationHistory";
                    await _db.ExecuteDML(updateQuantityAndDateQuery, 
                        new { CostPrice = costprice, DateAdd = newDate, idStockLocationHistory , userName});

                    response.Message = "Stock CostPrice & Date successfully updated.";
                    return response;
                }

                // Update Quantity
                string updateQuantityQuery = @"UPDATE tblStockLocationHistory SET CostPrice = @CostPrice, 
                        ModifyTime = GETDATE(), IsCostUpdated = 1 , Username = @userName
                        WHERE idStockLocationHistory = @idStockLocationHistory";
                await _db.ExecuteDML(updateQuantityQuery, new { CostPrice = costprice, idStockLocationHistory, userName });
                response.Message = "Stock CostPrice successfully updated.";
                return response;
            }
            catch (Exception ex)
            {
                return HandleException(response, ex);
            }
        }

    }
}
