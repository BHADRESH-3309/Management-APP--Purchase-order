namespace WebManagementApp.Models
{
    public class DashboardData
    {
        public string Period { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal COGS { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal NetProfit { get; set; }
        public decimal Margin { get; set; }
        public int Sales { get; set; }
        public int UnitsSold { get; set; }
        public decimal Fees { get; set; }
        public decimal AvgCostPrice { get; set; }
        public decimal SalesPrice { get; set; }
        public int BundleQty { get; set; }
        public int Qty { get; set; }
        public string DisplayDate =>
           Period == "Today" || Period == "Yesterday"
           ? StartDate.ToString("MMM dd, yyyy") // Display only the date
           : $"{StartDate:MMM dd, yyyy} - {EndDate:MMM dd, yyyy}";
    }
}
