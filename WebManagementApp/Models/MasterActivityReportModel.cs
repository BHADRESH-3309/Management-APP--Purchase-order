using Microsoft.Extensions.Hosting;

namespace WebManagementApp.Models
{
    public class MasterActivityReportModel
    {
        public string MasterSKU { get; set; }
        public string SalesChannel { get; set; }
        public string MarketplaceSKU { get; set; }
        public string StockLocation { get; set; }
        public string User { get; set; }
        public string ActionPerformed { get; set; }
        public int OldQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public string Date { get; set; }
        public decimal CurrentCost { get; set; }
        public decimal OldCostPrice { get; set; }

        public List<MasterSKULists> masterSKULists { get; set; }
    }
}
