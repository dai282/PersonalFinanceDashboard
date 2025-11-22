namespace PersonalFinance.API.DTOs
{
    public class MonthlySummaryDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance { get; set; }
        public int TransactionCount { get; set; }
        public decimal SavingsRate { get; set; }
    }

    public class SpendingByCategoryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class SpendingByCategoryReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSpending { get; set; }
        public IEnumerable<SpendingByCategoryDto> Categories { get; set; } = new List<SpendingByCategoryDto>();
    }

    public class MonthlyTrendDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public decimal NetSavings { get; set; }
    }

    public class IncomeVsExpenseTrendsDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<MonthlyTrendDto> Trends { get; set; } = new List<MonthlyTrendDto>();
    }
}