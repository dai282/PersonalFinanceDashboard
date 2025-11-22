using Microsoft.EntityFrameworkCore;
using PersonalFinance.Core.Entities;
using PersonalFinance.Core.Interfaces;
using PersonalFinance.Infrastructure.Data;

namespace PersonalFinance.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Transaction> Transactions, int TotalCount)> GetAllAsync(
            int userId,
            int pageNumber,
            int pageSize,
            int? categoryId = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId);

            // Apply filters
            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);

            if (startDate.HasValue)
                query = query.Where(t => t.TransactionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.TransactionDate <= endDate.Value);

            var totalCount = await query.CountAsync();

            var transactions = await query
                .OrderByDescending(t => t.TransactionDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (transactions, totalCount);
        }

        public async Task<Transaction?> GetByIdAsync(int id, int userId)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Load the category
            await _context.Entry(transaction).Reference(t => t.Category).LoadAsync();
            return transaction;
        }

        public async Task<Transaction?> UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();

            await _context.Entry(transaction).Reference(t => t.Category).LoadAsync();
            return transaction;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null)
                return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(decimal TotalIncome, decimal TotalExpenses, decimal Balance)> GetStatisticsAsync(
            int userId,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _context.Transactions
                .Where(t => t.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(t => t.TransactionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.TransactionDate <= endDate.Value);

            var totalIncome = await query
                .Where(t => t.Type == "Income")
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            var totalExpenses = await query
                .Where(t => t.Type == "Expense")
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            var balance = totalIncome - totalExpenses;

            return (totalIncome, totalExpenses, balance);
        }
    }
}