using Microsoft.EntityFrameworkCore;
using PersonalFinance.Core.Interfaces;
using PersonalFinance.Infrastructure.Data;

namespace PersonalFinance.Infrastructure.Repositories
{
    public class ReportsRepository : IReportsRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(decimal TotalIncome, decimal TotalExpenses, decimal Balance, int TransactionCount)> GetMonthlySummaryAsync(
            string userId, int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId
                    && t.TransactionDate >= startDate
                    && t.TransactionDate <= endDate)
                .ToListAsync();

            var totalIncome = transactions
                .Where(t => t.Type == "Income")
                .Sum(t => t.Amount);

            var totalExpenses = transactions
                .Where(t => t.Type == "Expense")
                .Sum(t => t.Amount);

            var balance = totalIncome - totalExpenses;
            var count = transactions.Count;

            return (totalIncome, totalExpenses, balance, count);
        }

        public async Task<IEnumerable<(string CategoryName, string CategoryIcon, decimal Amount, decimal Percentage)>> GetSpendingByCategoryAsync(
            string userId, DateTime startDate, DateTime endDate)
        {
            var expenses = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId
                    && t.Type == "Expense"
                    && t.TransactionDate >= startDate
                    && t.TransactionDate <= endDate)
                .GroupBy(t => new { t.Category.Name, t.Category.Icon })
                .Select(g => new
                {
                    CategoryName = g.Key.Name,
                    CategoryIcon = g.Key.Icon ?? "",
                    Amount = g.Sum(t => t.Amount)
                })
                .OrderByDescending(x => x.Amount)
                .ToListAsync();

            var totalExpenses = expenses.Sum(e => e.Amount);

            var result = expenses.Select(e => (
                e.CategoryName,
                e.CategoryIcon,
                e.Amount,
                totalExpenses > 0 ? Math.Round((e.Amount / totalExpenses) * 100, 2) : 0
            ));

            return result;
        }

        public async Task<IEnumerable<(int Month, int Year, decimal Income, decimal Expenses)>> GetIncomeVsExpenseTrendsAsync(
            string userId, DateTime startDate, DateTime endDate)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId
                    && t.TransactionDate >= startDate
                    && t.TransactionDate <= endDate)
                .ToListAsync();

            var trends = transactions
                .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Income = g.Where(t => t.Type == "Income").Sum(t => t.Amount),
                    Expenses = g.Where(t => t.Type == "Expense").Sum(t => t.Amount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            return trends.Select(t => (t.Month, t.Year, t.Income, t.Expenses));
        }
    }
}