using Microsoft.EntityFrameworkCore;
using PersonalFinance.Core.Entities;
using PersonalFinance.Core.Interfaces;
using PersonalFinance.Infrastructure.Data;

namespace PersonalFinance.Infrastructure.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly ApplicationDbContext _context;

        //dependency injection + dependency inversion principle
        public BudgetRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Budget>> GetAllAsync(string userId, int? month = null, int? year = null)
        {
            var query = _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == userId);

            if (month.HasValue)
                query = query.Where(b => b.Month == month.Value);

            if (year.HasValue)
                query = query.Where(b => b.Year == year.Value);

            return await query.OrderBy(b => b.Category.Name).ToListAsync();
        }

        public async Task<Budget?> GetByIdAsync(int id, string userId)
        {
            return await _context.Budgets
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
        }

        public async Task<Budget> CreateAsync(Budget budget)
        {
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();

            await _context.Entry(budget).Reference(b => b.Category).LoadAsync();
            return budget;
        }

        public async Task<Budget?> UpdateAsync(Budget budget)
        {
            _context.Budgets.Update(budget);
            await _context.SaveChangesAsync();

            await _context.Entry(budget).Reference(b => b.Category).LoadAsync();
            return budget;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (budget == null)
                return false;

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetActualSpendingAsync(string userId, int categoryId, int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var total = await _context.Transactions
                .Where(t => t.UserId == userId
                    && t.CategoryId == categoryId
                    && t.Type == "Expense"
                    && t.TransactionDate >= startDate
                    && t.TransactionDate <= endDate)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            return total;
        }
    }
}