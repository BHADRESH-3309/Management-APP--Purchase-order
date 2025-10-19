using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace WebManagementApp.DataAccess.DbAccess
{
    public class SqlDataAccess : ISqlDataAccess
    {
        private readonly string _connectionString;
        private readonly int _timeout;

        public SqlDataAccess(IConfiguration config)
        {

            _connectionString = config.GetConnectionString("CN-WebManagementApp");
            _timeout = 60 * 3; //Seconds 60 * 3 = 180 = 3 minutes
        }
        
        // Get data using dapper
        public async Task<IEnumerable<T>> GetData<T, U>(string sqlQuery, U parameters)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<T>(sqlQuery, parameters, commandTimeout: _timeout);
            }
        }

        // Get data using store procedure
        public async Task<IEnumerable<T>> GetDataSP<T, U>(string storeProcedure, U parameters)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<T>(storeProcedure, parameters, commandType: CommandType.StoredProcedure, commandTimeout: _timeout);
            }
        }

        // Execute dml statement using dapper
        public async Task ExecuteDML<T>(string sqlQuery, T parameters)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sqlQuery, parameters, commandTimeout: _timeout);
            }
        }

        // Retrive data in datatable
        public DataTable GET_DATATABLE(string sqlQuery)
        {
            SqlConnection connection = new SqlConnection(_connectionString);

            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            da.Fill(dt);
            da.Dispose();
            connection.Close();
            return dt;
        }

        // For Get single value
        public string GetSingleValue(string sqlQuery)
        {
            string retval = string.Empty;

            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            cmd.CommandText = sqlQuery;
            if (cmd.ExecuteScalar() == null)
                return retval;

            retval = cmd.ExecuteScalar().ToString().Trim();

            cmd.Dispose();
            connection.Close();

            return retval;
        }
        
        // For bulkinsert data
        public void BulkInsertMasterSKU(DataTable dtTableData, string destinationTableName)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection);

            // Set the timeout.
            sqlBulkCopy.BulkCopyTimeout = 700;

            //Set the database table name
            sqlBulkCopy.DestinationTableName = destinationTableName;
            sqlBulkCopy.ColumnMappings.Add("SKU", "SKU");
            sqlBulkCopy.ColumnMappings.Add("Title", "Title");
            sqlBulkCopy.ColumnMappings.Add("Note", "Note");
            sqlBulkCopy.ColumnMappings.Add("Description", "Description");
            sqlBulkCopy.ColumnMappings.Add("Weight", "Weight");
            sqlBulkCopy.ColumnMappings.Add("ProductSource", "ProductSource");
            sqlBulkCopy.ColumnMappings.Add("idBrand", "idBrand");
            sqlBulkCopy.ColumnMappings.Add("idCategory", "idCategory");
            sqlBulkCopy.ColumnMappings.Add("idShippingLabel", "idShippingLabel");
            sqlBulkCopy.ColumnMappings.Add("GTIN", "GTIN");
            sqlBulkCopy.ColumnMappings.Add("EAN", "EAN");
            sqlBulkCopy.ColumnMappings.Add("Barcode", "Barcode");
            sqlBulkCopy.ColumnMappings.Add("Width", "Width");
            sqlBulkCopy.ColumnMappings.Add("Length", "Length");
            sqlBulkCopy.ColumnMappings.Add("Height", "Height");

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            sqlBulkCopy.WriteToServer(dtTableData);
            connection.Close();
        }
        
        //BulkUpdateMasterSKU
        public void BulkUpdateMasterSKU(DataTable dt_temp, string TableName)
        {
            string query = string.Empty;


            SqlConnection connection = new SqlConnection(_connectionString);

            string tempTable = "TmpTableBulkUpdate" + TableName;
            //If same name table as this name so delete this name table
            query = $"IF OBJECT_ID('{tempTable}', 'U') IS NOT NULL   DROP TABLE {tempTable}";
            ExecuteDML(query, new { }).GetAwaiter().GetResult();

            query = string.Empty;//Create Table temporary table
            query = $@"CREATE TABLE {tempTable}([SKU] [nvarchar](300), [Title] [nvarchar](512),Description [nvarchar](max),
                    Note [nvarchar](max), Weight[decimal](10,2),idBrand [uniqueidentifier] null ,ProductSource [nvarchar](100), 
                    EAN [nvarchar](50), GTIN [nvarchar] (120), Barcode [nvarchar] (120), 
                    Width [decimal] (15,2),Length [decimal](15,2),Height [decimal](15,2),
                    idCategory [uniqueidentifier] null , idShippingLabel [uniqueidentifier] null )";


            ExecuteDML(query, new { }).GetAwaiter().GetResult();

            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection);

            // Set the timeout.
            sqlBulkCopy.BulkCopyTimeout = 5000;

            //Set the database table name
            sqlBulkCopy.DestinationTableName = tempTable;

            sqlBulkCopy.ColumnMappings.Add("SKU", "SKU");
            sqlBulkCopy.ColumnMappings.Add("Title", "Title");
            sqlBulkCopy.ColumnMappings.Add("Description", "Description");
            sqlBulkCopy.ColumnMappings.Add("Weight", "Weight");
            sqlBulkCopy.ColumnMappings.Add("ProductSource", "ProductSource");
            sqlBulkCopy.ColumnMappings.Add("idBrand", "idBrand");
            sqlBulkCopy.ColumnMappings.Add("idCategory", "idCategory");
            sqlBulkCopy.ColumnMappings.Add("idShippingLabel", "idShippingLabel");
            sqlBulkCopy.ColumnMappings.Add("GTIN", "GTIN");
            sqlBulkCopy.ColumnMappings.Add("EAN", "EAN");
            sqlBulkCopy.ColumnMappings.Add("Barcode", "Barcode");
            sqlBulkCopy.ColumnMappings.Add("Width", "Width");
            sqlBulkCopy.ColumnMappings.Add("Length", "Length");
            sqlBulkCopy.ColumnMappings.Add("Height", "Height");
            sqlBulkCopy.ColumnMappings.Add("Note", "Note");

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            sqlBulkCopy.WriteToServer(dt_temp);
            connection.Close();

            string updateBulkDataQuery = @$"UPDATE M SET M.Title = T.Title,M.Description = T.Description,
                                           M.Weight = T.Weight, M.ProductSource = T.ProductSource,M.Note = T.Note,
                                           M.idBrand = T.idBrand, M.GTIN = T.GTIN , M.EAN = T.EAN , M.Barcode = T.Barcode,
                                           M.Width = T.Width ,M.Length = T.Length,M.Height = T.Height, M.ModifyTime = getdate(),
                                           M.idCategory = T.idCategory, M.idShippingLabel = T.idShippingLabel                                           
                                           FROM tblMasterSKU AS M INNER JOIN {tempTable} AS T ON M.SKU = T.SKU";

            ExecuteDML(updateBulkDataQuery, new { }).GetAwaiter().GetResult();

            ExecuteDML($"DROP TABLE {tempTable}", new { }).GetAwaiter().GetResult();
        }        
        
        //BulkInsertBrand
        public void BulkInsertBrand(DataTable dtTableData, string destinationTableName)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection);

            // Set the timeout.
            sqlBulkCopy.BulkCopyTimeout = 700;

            //Set the database table name
            sqlBulkCopy.DestinationTableName = destinationTableName;
            sqlBulkCopy.ColumnMappings.Add("Brand", "Brand");

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            sqlBulkCopy.WriteToServer(dtTableData);

            connection.Close();
        }
        
        //BulkUpdateMasterSKU
        public void BulkUpdateBrand(DataTable dt_temp, string TableName)
        {
            string query = string.Empty;

            string updatequery = string.Empty;

            SqlConnection connection = new SqlConnection(_connectionString);

            string tempTable = "TmpTableBulkUpdate" + TableName;
            //If same name table as this name so delete this name table
            query = $"IF OBJECT_ID('{tempTable}', 'U') IS NOT NULL   DROP TABLE {tempTable}";
            ExecuteDML(query, new { }).GetAwaiter().GetResult();

            query = string.Empty;//Create Table temporary table
            query = $@"CREATE TABLE {tempTable}([Brand] [nvarchar](120))";


            ExecuteDML(query, new { }).GetAwaiter().GetResult();

            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection);

            // Set the timeout.
            sqlBulkCopy.BulkCopyTimeout = 5000;

            //Set the database table name
            sqlBulkCopy.DestinationTableName = tempTable;

            sqlBulkCopy.ColumnMappings.Add("Brand", "Brand");

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            sqlBulkCopy.WriteToServer(dt_temp);
            connection.Close();

            string updateBulkDataQuery = @$"UPDATE M SET M.Brand = T.Brand,M.ModifyTime = getdate()
                         FROM tblBrand AS M INNER JOIN {tempTable} AS T ON M.Brand = T.Brand";

            ExecuteDML(updateBulkDataQuery, new { }).GetAwaiter().GetResult();

            ExecuteDML($"DROP TABLE {tempTable}", new { }).GetAwaiter().GetResult();
        }
        
        public void BulkinsertMappingSKU(DataTable dt_temp, string TableName)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection);

            // Set the timeout.
            sqlBulkCopy.BulkCopyTimeout = 700;

            //Set the database table name
            sqlBulkCopy.DestinationTableName = TableName;
            sqlBulkCopy.ColumnMappings.Add("idMasterSKU", "idMasterSKU");
            sqlBulkCopy.ColumnMappings.Add("SKU", "SKU");
            sqlBulkCopy.ColumnMappings.Add("SalesChannel", "SalesChannel");
            sqlBulkCopy.ColumnMappings.Add("CompanyName", "CompanyName");
            sqlBulkCopy.ColumnMappings.Add("Quantity", "Quantity");
            sqlBulkCopy.ColumnMappings.Add("DateAdd", "DateAdd");

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            sqlBulkCopy.WriteToServer(dt_temp);
            connection.Close();

        }

        public void BulkUpdateMappingSKU(DataTable dt_temp)
        {
            string tempTable = "TmpTableMappingSkuBulkUpdate";
            SqlConnection connection = new SqlConnection(_connectionString);

            ExecuteDML($"IF OBJECT_ID('{tempTable}', 'U') IS NOT NULL DROP TABLE {tempTable}", new { }).GetAwaiter().GetResult();

            string query = @$"CREATE TABLE {tempTable}([idMappingSKU] [uniqueidentifier], [SKU] [nvarchar] (256) ,[SalesChannel][nvarchar](20), 
                [CompanyName][nvarchar](100), [Quantity][int])";

            ExecuteDML(query, new { }).GetAwaiter().GetResult();

            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection);

            // Set the timeout.
            sqlBulkCopy.BulkCopyTimeout = 700;

            //Set the database table name
            sqlBulkCopy.DestinationTableName = tempTable;
            sqlBulkCopy.ColumnMappings.Add("idMappingSKU", "idMappingSKU");
            sqlBulkCopy.ColumnMappings.Add("SKU", "SKU");
            sqlBulkCopy.ColumnMappings.Add("SalesChannel", "SalesChannel");
            sqlBulkCopy.ColumnMappings.Add("CompanyName", "CompanyName");
            sqlBulkCopy.ColumnMappings.Add("Quantity", "Quantity");
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            sqlBulkCopy.WriteToServer(dt_temp);
            connection.Close();

            string updatequery = @$"UPDATE M SET M.SKU = T.SKU, M.SalesChannel = T.SalesChannel, 
                                    M.CompanyName = T.CompanyName, M.Quantity = T.Quantity 
                                    FROM tblMappingSKU AS M INNER JOIN {tempTable} AS T ON M.idMappingSKU = T.idMappingSKU";

            ExecuteDML(updatequery, new { }).GetAwaiter().GetResult();

            ExecuteDML($"DROP TABLE {tempTable}", new { }).GetAwaiter().GetResult();
        }

        public void BulkInsertBundleSKU(DataTable dt_temp, string TableName)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection);

            // Set the timeout.
            sqlBulkCopy.BulkCopyTimeout = 700;

            //Set the database table name
            sqlBulkCopy.DestinationTableName = TableName;
            sqlBulkCopy.ColumnMappings.Add("idMasterSKU", "idMasterSKU");
            sqlBulkCopy.ColumnMappings.Add("MarketplaceSKU", "MarketplaceSKU");
            sqlBulkCopy.ColumnMappings.Add("SalesChannel", "SalesChannel");
            sqlBulkCopy.ColumnMappings.Add("CompanyName", "CompanyName");
            sqlBulkCopy.ColumnMappings.Add("ReduceQuantity", "ReduceQuantity");

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            sqlBulkCopy.WriteToServer(dt_temp);
            connection.Close();
        }

        public void BulkUpdateBundleSKU(DataTable dt_temp)
        {
            string tempTable = "TmpTableBundleSkuBulkUpdate";
            SqlConnection connection = new SqlConnection(_connectionString);

            ExecuteDML($"IF OBJECT_ID('{tempTable}', 'U') IS NOT NULL DROP TABLE {tempTable}", new { }).GetAwaiter().GetResult();

            string query = @$"CREATE TABLE {tempTable}([idBundle] [uniqueidentifier], [MarketplaceSKU] [nvarchar] (256),
                [SalesChannel][nvarchar](20), [CompanyName][nvarchar](100), [ReduceQuantity][int])";

            ExecuteDML(query, new { }).GetAwaiter().GetResult();

            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection);

            // Set the timeout.
            sqlBulkCopy.BulkCopyTimeout = 700;

            //Set the database table name
            sqlBulkCopy.DestinationTableName = tempTable;
            sqlBulkCopy.ColumnMappings.Add("idBundle", "idBundle");
            sqlBulkCopy.ColumnMappings.Add("MarketplaceSKU", "MarketplaceSKU");
            sqlBulkCopy.ColumnMappings.Add("SalesChannel", "SalesChannel");
            sqlBulkCopy.ColumnMappings.Add("CompanyName", "CompanyName");
            sqlBulkCopy.ColumnMappings.Add("ReduceQuantity", "ReduceQuantity");
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            sqlBulkCopy.WriteToServer(dt_temp);
            connection.Close();

            string updatequery = @$"UPDATE M SET M.ReduceQuantity = T.ReduceQuantity, M.ModifyTime = getdate()
                                    FROM tblBundle AS M INNER JOIN {tempTable} AS T ON M.idBundle = T.idBundle";

            ExecuteDML(updatequery, new { }).GetAwaiter().GetResult();

            ExecuteDML($"DROP TABLE {tempTable}", new { }).GetAwaiter().GetResult();
        }

        public void BulkInsertSyncSKU(DataTable dt_temp, string TableName)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection);

            // Set the timeout.
            sqlBulkCopy.BulkCopyTimeout = 700;

            //Set the database table name
            sqlBulkCopy.DestinationTableName = TableName;
            sqlBulkCopy.ColumnMappings.Add("idMasterSKU", "idMasterSKU");
            sqlBulkCopy.ColumnMappings.Add("idMappingSKU", "idMappingSKU");
            sqlBulkCopy.ColumnMappings.Add("SKU", "MarketplaceSKU");
            sqlBulkCopy.ColumnMappings.Add("SalesChannel", "SalesChannel");
            sqlBulkCopy.ColumnMappings.Add("CompanyName", "CompanyName");
            sqlBulkCopy.ColumnMappings.Add("IsSyncProcessEnable", "IsSyncProcessEnable");
            sqlBulkCopy.ColumnMappings.Add("IsUnSyncProcessEnable", "IsUnSyncProcessEnable");

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            sqlBulkCopy.WriteToServer(dt_temp);
            connection.Close();
        }      
    }

}
