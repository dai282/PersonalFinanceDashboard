namespace PersonalFinance.Core.Interfaces
{
    public interface IReportsRepository
    {
        Task<(decimal TotalIncome, decimal TotalExpenses, decimal Balance, int TransactionCount)> GetMonthlySummaryAsync(
            int userId, int month, int year);

        Task<IEnumerable<(string CategoryName, string CategoryIcon, decimal Amount, decimal Percentage)>> GetSpendingByCategoryAsync(
            int userId, DateTime startDate, DateTime endDate);

        Task<IEnumerable<(int Month, int Year, decimal Income, decimal Expenses)>> GetIncomeVsExpenseTrendsAsync(
            int userId, DateTime startDate, DateTime endDate);
    }
}