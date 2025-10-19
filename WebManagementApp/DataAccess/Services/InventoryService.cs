using WebManagementApp.DataAccess.DbAccess;
using WebManagementApp.Models;

namespace WebManagementApp.DataAccess.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        public InventoryService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        public async Task<IEnumerable<AmazonInventoryVM>> GetAmazonInventoryData(string mappedType)
        {
            string query = string.Empty;
            if (mappedType == "MAP")
            {
                query = @"
                SELECT AI.SKU,AI.Title,AI.ASIN,AI.Quantity, AI.AfnWarehouseQuantity, AI.AfnFulfillableQuantity, AI.AfnUnsellableQuantity, 
                AI.AfnReservedQuantity, AI.AfnInboundReceivingQuantity, AI.FulfillmentBy,AI.Price, AI.Source,
                FORMAT(AI.RecordUpdateTime, 'dd/MM/yyyy hh:mm tt') AS RecordUpdateTime, tblMastSKU.SKU AS MasterSKU
                FROM (SELECT * FROM tblAmazonInventory WHERE IsActiveInventory = 1 AND CompanyName = 'Greenwize') as AI 
                INNER JOIN (SELECT * FROM tblMappingSKU WHERE LOWER(SalesChannel) = 'amazon' AND CompanyName = 'Greenwize') as tblMapSKU 
                ON AI.SKU = tblMapSKU.SKU INNER JOIN tblMasterSKU as tblMastSKU ON tblMapSKU.idMasterSKU = tblMastSKU.idMasterSKU
                WHERE AI.Quantity > 0;";
            }
            else if (mappedType == "UNMAP")
            {
                query = @"
                SELECT AI.SKU,AI.Title,AI.ASIN,AI.Quantity, AI.AfnWarehouseQuantity, AI.AfnFulfillableQuantity, AI.AfnUnsellableQuantity, 
				AI.AfnReservedQuantity, AI.AfnInboundReceivingQuantity, AI.FulfillmentBy,AI.Price, AI.Source,
                FORMAT(AI.RecordUpdateTime, 'dd/MM/yyyy hh:mm tt') AS RecordUpdateTime,COALESCE(tblMapSKU.SKU, '') AS MasterSKU
                FROM (SELECT * FROM tblAmazonInventory WHERE IsActiveInventory = 1 AND CompanyName = 'Greenwize') as AI
                LEFT JOIN (SELECT * FROM tblMappingSKU WHERE LOWER(SalesChannel) = 'amazon' AND CompanyName = 'Greenwize') as tblMapSKU 
                ON AI.SKU = tblMapSKU.SKU WHERE tblMapSKU.SKU IS NULL AND AI.Quantity > 0;";
            }
            else
            {
                query = @"
                SELECT  AI.SKU,AI.Title,AI.ASIN,AI.Quantity, AI.AfnWarehouseQuantity, AI.AfnFulfillableQuantity, AI.AfnUnsellableQuantity, 
				AI.AfnReservedQuantity, AI.AfnInboundReceivingQuantity, AI.FulfillmentBy,AI.Price, AI.Source,
                FORMAT(AI.RecordUpdateTime, 'dd/MM/yyyy hh:mm tt') AS RecordUpdateTime,COALESCE(tblMasSKU.SKU, '') AS MasterSKU
                FROM (SELECT * FROM tblAmazonInventory WHERE IsActiveInventory = 1 AND CompanyName = 'Greenwize') as AI
                LEFT JOIN (SELECT * FROM tblMappingSKU WHERE LOWER(SalesChannel) = 'amazon' AND CompanyName = 'Greenwize') as tblMapSKU 
                ON AI.SKU = tblMapSKU.SKU LEFT JOIN tblMasterSKU as tblMasSKU ON tblMapSKU.idMasterSKU = tblMasSKU.idMasterSKU
                WHERE AI.Quantity > 0;";
            }
            var result = await _sqlDataAccess.GetData<AmazonInventoryVM, dynamic>(query, new { });
            return result;
        }

        public async Task<IEnumerable<AmazonInventoryVM>> GetAmazonAVASuppliesInventoryData(string mappedType)
        {
            string query = string.Empty;
            if (mappedType == "MAP")
            {
                query = @"
                SELECT AI.SKU,AI.Title,AI.ASIN,AI.Quantity, AI.AfnWarehouseQuantity, AI.AfnFulfillableQuantity, AI.AfnUnsellableQuantity, 
                AI.AfnReservedQuantity, AI.AfnInboundReceivingQuantity, AI.FulfillmentBy,AI.Price, AI.Source,
                FORMAT(AI.RecordUpdateTime, 'dd/MM/yyyy hh:mm tt') AS RecordUpdateTime, tblMastSKU.SKU AS MasterSKU
                FROM (SELECT * FROM tblAmazonInventory WHERE IsActiveInventory = 1 AND CompanyName = 'AVA Supplies') as AI 
                INNER JOIN (SELECT * FROM tblMappingSKU WHERE LOWER(SalesChannel) = 'amazon' AND CompanyName = 'AVA Supplies') as tblMapSKU 
                ON AI.SKU = tblMapSKU.SKU INNER JOIN tblMasterSKU as tblMastSKU ON tblMapSKU.idMasterSKU = tblMastSKU.idMasterSKU
                WHERE AI.Quantity > 0;";
            }
            else if (mappedType == "UNMAP")
            {
                query = @"
                SELECT AI.SKU,AI.Title,AI.ASIN,AI.Quantity, AI.AfnWarehouseQuantity, AI.AfnFulfillableQuantity, AI.AfnUnsellableQuantity, 
				AI.AfnReservedQuantity, AI.AfnInboundReceivingQuantity, AI.FulfillmentBy,AI.Price, AI.Source,
                FORMAT(AI.RecordUpdateTime, 'dd/MM/yyyy hh:mm tt') AS RecordUpdateTime,COALESCE(tblMapSKU.SKU, '') AS MasterSKU
                FROM (SELECT * FROM tblAmazonInventory WHERE IsActiveInventory = 1 AND CompanyName = 'AVA Supplies') as AI
                LEFT JOIN (SELECT * FROM tblMappingSKU WHERE LOWER(SalesChannel) = 'amazon' AND CompanyName = 'AVA Supplies') as tblMapSKU 
                ON AI.SKU = tblMapSKU.SKU WHERE tblMapSKU.SKU IS NULL AND AI.Quantity > 0;";
            }
            else
            {
                query = @"
                SELECT  AI.SKU,AI.Title,AI.ASIN,AI.Quantity, AI.AfnWarehouseQuantity, AI.AfnFulfillableQuantity, AI.AfnUnsellableQuantity, 
				AI.AfnReservedQuantity, AI.AfnInboundReceivingQuantity, AI.FulfillmentBy,AI.Price, AI.Source,
                FORMAT(AI.RecordUpdateTime, 'dd/MM/yyyy hh:mm tt') AS RecordUpdateTime,COALESCE(tblMasSKU.SKU, '') AS MasterSKU
                FROM (SELECT * FROM tblAmazonInventory WHERE IsActiveInventory = 1 AND CompanyName = 'AVA Supplies') as AI
                LEFT JOIN (SELECT * FROM tblMappingSKU WHERE LOWER(SalesChannel) = 'amazon' AND CompanyName = 'AVA Supplies') as tblMapSKU  
                ON AI.SKU = tblMapSKU.SKU LEFT JOIN tblMasterSKU as tblMasSKU ON tblMapSKU.idMasterSKU = tblMasSKU.idMasterSKU
                WHERE AI.Quantity > 0;";
            }
            var result = await _sqlDataAccess.GetData<AmazonInventoryVM, dynamic>(query, new { });
            return result;
        }

        public async Task<IEnumerable<EbayInventoryVM>> GetEbayInventoryData(string mappedType)
        {
            string query = string.Empty;
            if (mappedType == "MAP")
            {
                query = @"SELECT tblEbayInv.SKU,tblEbayInv.Title,tblEbayInv.ItemId,tblEbayInv.Quantity,tblEbayInv.Price,
                        FORMAT(tblEbayInv.RecordUpdateTime, 'dd/MM/yyyy hh:mm tt') AS RecordUpdateTime, tblMasSKU.SKU AS MasterSKU
                        FROM (SELECT * FROM tblEbayInventory)  as tblEbayInv
                        INNER JOIN (SELECT * FROM tblMappingSKU WHERE LOWER(SalesChannel) = 'ebay') as tblMapSKU ON tblEbayInv.SKU = tblMapSKU.SKU
                        INNER JOIN tblMasterSKU as tblMasSKU ON tblMapSKU.idMasterSKU = tblMasSKU.idMasterSKU
                        WHERE CONVERT(date, tblEbayInv.RecordUpdateTime) >= CONVERT(date, DATEADD(DAY, -1, GETDATE()))
                        AND tblEbayInv.Quantity > 0;";
            }
            else if (mappedType == "UNMAP")
            {
                query = @"SELECT tblEbayInv.SKU,tblEbayInv.Title,tblEbayInv.ItemId,tblEbayInv.Quantity,tblEbayInv.Price,
                        FORMAT(tblEbayInv.RecordUpdateTime, 'dd/MM/yyyy hh:mm tt') AS RecordUpdateTime,COALESCE(tblMapSKU.SKU, '') AS MasterSKU
                        FROM (SELECT * FROM tblEbayInventory) as tblEbayInv
                        LEFT JOIN (SELECT * FROM tblMappingSKU WHERE LOWER(SalesChannel) = 'ebay') as tblMapSKU ON tblEbayInv.SKU = tblMapSKU.SKU
                        WHERE tblMapSKU.SKU IS NULL AND CONVERT(date, tblEbayInv.RecordUpdateTime) >= CONVERT(date, DATEADD(DAY, -1, GETDATE()))
                        AND tblEbayInv.Quantity > 0;";
            }
            else
            {
                query = @"SELECT  tblEbayInv.SKU,tblEbayInv.Title,tblEbayInv.ItemId,tblEbayInv.Quantity,tblEbayInv.Price,
                        FORMAT(tblEbayInv.RecordUpdateTime, 'dd/MM/yyyy hh:mm tt') AS RecordUpdateTime,COALESCE(tblMasSKU.SKU, '') AS MasterSKU
                        FROM (SELECT * FROM tblEbayInventory) as tblEbayInv
                        LEFT JOIN (SELECT * FROM tblMappingSKU WHERE LOWER(SalesChannel) = 'ebay') as tblMapSKU ON tblEbayInv.SKU = tblMapSKU.SKU
                        LEFT JOIN tblMasterSKU as tblMasSKU ON tblMapSKU.idMasterSKU = tblMasSKU.idMasterSKU
                        WHERE CONVERT(date, tblEbayInv.RecordUpdateTime) >= CONVERT(date, DATEADD(DAY, -1, GETDATE()))
                        AND tblEbayInv.Quantity > 0;";
            }
            var result = await _sqlDataAccess.GetData<EbayInventoryVM, dynamic>(query, new { });
            return result;
        }
        public async Task<IEnumerable<OnBuyInventoryVM>> GetOnBuyInventoryData(string mappedType)
        {
            string query = string.Empty;
            if (mappedType == "MAP")
            {
                query = @"SELECT tblOnBuyInv.SKU,tblOnBuyInv.Title,tblOnBuyInv.ProductURL,tblOnBuyInv.Quantity,tblOnBuyInv.Price,
                         FORMAT(tblOnBuyInv.RecordUpdateTime, 'dd/MM/yyyy hh:mm tt') AS RecordUpdateTime, tblMastSKU.SKU AS MasterSKU
                         FROM (SELECT * FROM tblOnbuyInventory)  as tblOnBuyInv
                         INNER JOIN (SELECT * FROM tblMappingSKU WHERE LOWER(SalesChannel) = 'onbuy') as tblMapSKU ON tblOnBuyInv.SKU = tblMapSKU.SKU
                         INNER JOIN tblMasterSKU as tblMastSKU ON tblMapSKU.idMasterSKU = tblMastSKU.idMasterSKU
                         WHERE CONVERT(date, tblOnBuyInv.RecordUpdateTime) >= CONVERT(date, DATEADD(DAY, -1, GETDATE()))
                         AND tblOnBuyInv.Quantity > 0;";
            }
            else if (mappedType == "UNMAP")
            {
                query = @"SELECT tblOnBuyInv.SKU,tblOnBuyInv.Title,tblOnBuyInv.ProductURL,tblOnBuyInv.Quantity,tblOnBuyInv.Price,
                        FORMAT(tblOnBuyInv.RecordUpdateTime, 'dd/MM/yyyy hh:mm tt') AS RecordUpdateTime,COALESCE(tblMapSKU.SKU, '') AS MasterSKU
                        FROM (SELECT * FROM tblOnbuyInventory) as tblOnBuyInv
                        LEFT JOIN (SELECT * FROM tblMappingSKU WHERE LOWER(SalesChannel) = 'onbuy') as tblMapSKU ON tblOnBuyInv.SKU = tblMapSKU.SKU
                        WHERE tblMapSKU.SKU IS NULL AND CONVERT(date, tblOnBuyInv.RecordUpdateTime) >= CONVERT(date, DATEADD(DAY, -1, GETDATE()))
                        AND tblOnBuyInv.Quantity > 0;";
            }
            else
            {
                query = @"SELECT  tblOnBuyInv.SKU,tblOnBuyInv.Title,tblOnBuyInv.ProductURL,tblOnBuyInv.Quantity,tblOnBuyInv.Price,
                        FORMAT(tblOnBuyInv.RecordUpdateTime, 'dd/MM/yyyy hh:mm tt') AS RecordUpdateTime,COALESCE(tblMasSKU.SKU, '') AS MasterSKU
                        FROM (SELECT * FROM tblOnbuyInventory) as tblOnBuyInv
                        LEFT JOIN (SELECT * FROM tblMappingSKU WHERE LOWER(SalesChannel) = 'onbuy') as tblMapSKU ON tblOnBuyInv.SKU = tblMapSKU.SKU
                        LEFT JOIN tblMasterSKU as tblMasSKU ON tblMapSKU.idMasterSKU = tblMasSKU.idMasterSKU
                        WHERE CONVERT(date, tblOnBuyInv.RecordUpdateTime) >= CONVERT(date, DATEADD(DAY, -1, GETDATE()))
                        AND tblOnBuyInv.Quantity > 0;";
            }
            var result = await _sqlDataAccess.GetData<OnBuyInventoryVM, dynamic>(query, new { });
            return result;
        }
        public async Task<IEnumerable<ShopifyInventoryVM>> GetShopifyInventoryData(string mappedType)
        {
            string query = string.Empty;
            if (mappedType == "MAP")
            {
                query = @"SELECT tblShopifyInv.SKU,tblShopifyInv.Title,tblShopifyInv.VariantId,tblShopifyInv.Handle,tblShopifyInv.Quantity,tblShopifyInv.Price,
                         FORMAT(tblShopifyInv.RecordUpdateTime, 'dd/MM/yyyy hh:mm tt') AS RecordUpdateTime, tblMastSKU.SKU AS MasterSKU
                         FROM (SELECT * FROM tblShopifyInventory)  as tblShopifyInv
                         INNER JOIN (SELECT * FROM tblMappingSKU WHERE LOWER(SalesChannel) = 'shopify') as tblMapSKU ON tblShopifyInv.SKU = tblMapSKU.SKU
                         INNER JOIN tblMasterSKU as tblMastSKU ON tblMapSKU.idMasterSKU = tblMastSKU.idMasterSKU
                         WHERE CONVERT(date, tblShopifyInv.RecordUpdateTime) >= CONVERT(date, DATEADD(DAY, -1, GETDATE()))
                         AND tblShopifyInv.Quantity > 0;";
            }
            else if (mappedType == "UNMAP")
            {
                query = @"SELECT tblShopifyInv.SKU,tblShopifyInv.Title,tblShopifyInv.VariantId,tblShopifyInv.Handle,tblShopifyInv.Quantity,tblShopifyInv.Price,
                        FORMAT(tblShopifyInv.RecordUpdateTime, 'dd/MM/yyyy hh:mm tt') AS RecordUpdateTime,COALESCE(tblMapSKU.SKU, '') AS MasterSKU
                         FROM (SELECT * FROM tblShopifyInventory)  as tblShopifyInv
                        LEFT JOIN (SELECT * FROM tblMappingSKU WHERE LOWER(SalesChannel) = 'shopify') as tblMapSKU ON tblShopifyInv.SKU = tblMapSKU.SKU
                        WHERE tblMapSKU.SKU IS NULL AND CONVERT(date, tblShopifyInv.RecordUpdateTime) >= CONVERT(date, DATEADD(DAY, -1, GETDATE()))
                        AND tblShopifyInv.Quantity > 0;";
            }
            else
            {
                query = @"SELECT tblShopifyInv.SKU,tblShopifyInv.Title,tblShopifyInv.VariantId,tblShopifyInv.Handle,tblShopifyInv.Quantity,tblShopifyInv.Price,
                        FORMAT(tblShopifyInv.RecordUpdateTime, 'dd/MM/yyyy hh:mm tt') AS RecordUpdateTime,COALESCE(tblMasSKU.SKU, '') AS MasterSKU
                         FROM (SELECT * FROM tblShopifyInventory)  as tblShopifyInv
                        LEFT JOIN (SELECT * FROM tblMappingSKU WHERE LOWER(SalesChannel) = 'shopify') as tblMapSKU ON tblShopifyInv.SKU = tblMapSKU.SKU
                        LEFT JOIN tblMasterSKU as tblMasSKU ON tblMapSKU.idMasterSKU = tblMasSKU.idMasterSKU
                        WHERE CONVERT(date, tblShopifyInv.RecordUpdateTime) >= CONVERT(date, DATEADD(DAY, -1, GETDATE()))
                        AND tblShopifyInv.Quantity > 0;";
            }
            var result = await _sqlDataAccess.GetData<ShopifyInventoryVM, dynamic>(query, new { });
            return result;
        }

        // Update master sku and bundle quantity and Sync toggle
        public async Task<ResponseModel> UpdateUnmappedSKU(string marketplaceSKU, string? masterSKU, string? bundleQty,
            bool isSyncProcessEnable, string salesChannelName)
        {
            ResponseModel response = new ResponseModel();
            string salesChannel = string.Empty;
            string companyName = string.Empty;
            Guid idMasterSKU = Guid.Empty;
            string fulfillmentBy = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(marketplaceSKU) && !string.IsNullOrEmpty(salesChannelName))
                {
                    if (!string.IsNullOrEmpty(masterSKU))
                    {
                        salesChannel = salesChannelName.ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(salesChannel))
                        {
                            var parts = salesChannel.Trim().Split(' ', 2);

                            salesChannel = parts[0].ToLower();
                            companyName = parts.Length > 1 ? parts[1] : "";
                        }

                        if (salesChannel.ToLower() == "amazon")
                        {
                            fulfillmentBy = _sqlDataAccess.GetSingleValue($@"SELECT FulfillmentBy from tblAmazonInventory 
                                Where SKU = '{marketplaceSKU}' AND CompanyName = '{companyName}' ");
                        }

                        // Add Mapping SKU
                        string masterSKUId = _sqlDataAccess.GetSingleValue($@"SELECT idMasterSKU from tblMasterSKU where SKU = '{masterSKU}'");
                        idMasterSKU = string.IsNullOrEmpty(masterSKUId) ? Guid.Empty : Guid.Parse(masterSKUId);

                        string addMappingSKU = @"INSERT INTO tblMappingSKU(idMasterSKU, SKU, SalesChannel, CompanyName, 
                                Quantity) VALUES(@idMasterSKU, @sku, @salesChannel, @companyName, @quantity)";

                        var parameters = new
                        {
                            idMasterSKU,
                            sku = marketplaceSKU,
                            salesChannel,
                            companyName,
                            quantity = bundleQty
                        };

                        await _sqlDataAccess.ExecuteDML(addMappingSKU, parameters);

                        // Add Bundle quantity
                        if (int.Parse(bundleQty) > 1)
                        {
                            string addBundleSKU = @"INSERT INTO tblBundle(idMasterSKU, MarketplaceSKU, SalesChannel, 
                                    CompanyName, ReduceQuantity) VALUES(@idMasterSKU, @marketplaceSKU, @salesChannel, 
                                    @companyName, @reduceQuantity)";

                            var parameter = new
                            {
                                idMasterSKU,
                                marketplaceSKU,
                                salesChannel,
                                companyName,
                                reduceQuantity = bundleQty
                            };

                            await _sqlDataAccess.ExecuteDML(addBundleSKU, parameter);
                        }

                        // Add Sync inventory
                        if (fulfillmentBy != "FBA")
                        {
                            string mappingSKUId = _sqlDataAccess.GetSingleValue($@"SELECT idMappingSKU from tblMappingSKU WHERE
                                SKU = '{marketplaceSKU}'  AND idMasterSKU = '{idMasterSKU}' AND SalesChannel = '{salesChannel}' 
                                AND CompanyName = '{companyName}' AND Quantity = '{bundleQty}' ");
                            Guid idMappingSKU = string.IsNullOrEmpty(mappingSKUId) ? Guid.Empty : Guid.Parse(mappingSKUId);

                            string addSyncSKU = @"INSERT INTO tblSyncInventory(idMappingSKU, idMasterSKU, MarketplaceSKU, 
                                SalesChannel, CompanyName, IsSyncProcessEnable) VALUES(@idMappingSKU, @idMasterSKU, 
                                @marketplaceSKU, @salesChannel, @companyName, @isSyncProcessEnable)";

                            var syncParameters = new
                            {
                                idMappingSKU,
                                idMasterSKU,
                                marketplaceSKU,
                                salesChannel,
                                companyName,
                                isSyncProcessEnable
                            };

                            await _sqlDataAccess.ExecuteDML(addSyncSKU, syncParameters);
                        }

                        response.IsError = false;
                        response.Message = "Unmapped SKU details updated successfully.";
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Please enter master SKU";
                        return response;
                    }
                }
                else
                {
                    response.IsError = true;
                    response.Message = "Missing Marketplace SKU or Sales Channel";
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsError = true;
            }
            return response;
        }
    }
}
