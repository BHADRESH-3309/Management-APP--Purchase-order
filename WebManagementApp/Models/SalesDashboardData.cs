namespace WebManagementApp.Models
{
    public class SalesDashboardData
    {
        public decimal TotalGrossProfitToday { get; set; }
        public decimal TotalCOGSToday { get; set; }
        public decimal TotalExpensesToday { get; set; }
        public decimal TotalRevenueToday { get; set; }
        public decimal FeesToday { get; set; }
        public decimal AvgCostPriceToday { get; set; }
        public decimal SalesPriceToday { get; set; }
        public int BundleQtyToday { get; set; }
        public int SalesToday { get; set; }
        public int UnitsSoldToday { get; set; }

        public decimal TotalGrossProfitYesterday { get; set; }
        public decimal TotalCOGSYesterday { get; set; }
        public decimal TotalExpensesYesterday { get; set; }
        public decimal TotalRevenueYesterday { get; set; }
        public decimal FeesYesterday { get; set; }
        public decimal AvgCostPriceYesterday { get; set; }
        public decimal SalesPriceYesterday { get; set; }
        public int BundleQtyYesterday { get; set; }
        public int SalesYesterday { get; set; }
        public int UnitsSoldYesterday { get; set; }

        public decimal TotalGrossProfitLast7Days { get; set; }
        public decimal TotalCOGSLast7Days { get; set; }
        public decimal TotalExpensesLast7Days { get; set; }
        public decimal TotalRevenueLast7Days { get; set; }
        public decimal FeesLast7Days { get; set; }
        public decimal AvgCostPriceLast7Days { get; set; }
        public decimal SalesPriceLast7Days { get; set; }
        public int BundleQtyLast7Days { get; set; }
        public int SalesLast7Days { get; set; }
        public int UnitsSoldLast7Days { get; set; }

        public decimal TotalGrossProfitMonth { get; set; }
        public decimal TotalCOGSMonth { get; set; }
        public decimal TotalExpensesMonth { get; set; }
        public decimal TotalRevenueMonth { get; set; }
        public decimal FeesMonth { get; set; }
        public decimal AvgCostPriceMonth { get; set; }
        public decimal SalesPriceMonth { get; set; }
        public int BundleQtyLastMonth { get; set; }
        public int SalesMonth { get; set; }
        public int UnitsSoldMonth { get; set; }

        public decimal TotalGrossProfit { get; set; }
        public decimal TotalCOGS { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalFees { get; set; }
        public decimal TotalAvgCostPrice { get; set; }
        public decimal TotalSalesPrice { get; set; }
        public int TotalBundleQty { get; set; }
        public int TotalQuantity { get; set; }
        public int Sales { get; set; }
        public int UnitsSold { get; set; }
    }

}
