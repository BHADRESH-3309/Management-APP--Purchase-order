using System.Data;

namespace WebManagementApp.DataAccess.DbAccess
{
    public interface ISqlDataAccess
    {
        public Task<IEnumerable<T>> GetData<T, U>(string sqlQuery, U parameters);
        public Task<IEnumerable<T>> GetDataSP<T, U>(string storeProcedure, U parameters);
        public Task ExecuteDML<T>(string sqlQuery, T parameters);

        public DataTable GET_DATATABLE(string sqlQuery);
        public string GetSingleValue(string sqlQuery);

        public void BulkInsertMasterSKU(DataTable dtTableData, string destinationTableName);
        public void BulkUpdateMasterSKU(DataTable dt_temp, string destinationTableName);

        public void BulkinsertMappingSKU(DataTable dt_temp, string TableName);
        public void BulkUpdateMappingSKU(DataTable dt_temp);

        public void BulkInsertBrand(DataTable dtTableData, string destinationTableName);
        public void BulkUpdateBrand(DataTable dt_temp, string destinationTableName);

        public void BulkInsertBundleSKU(DataTable dt_temp, string TableName);
        public void BulkUpdateBundleSKU(DataTable dt_temp);

        public void BulkInsertSyncSKU(DataTable dt_temp, string TableName);
    }
}
