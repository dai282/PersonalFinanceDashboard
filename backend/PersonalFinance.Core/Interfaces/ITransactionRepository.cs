using PersonalFinance.Core.Entities;

namespace PersonalFinance.Core.Interfaces
{
    public interface ITransactionRepository
    {
        Task<(IEnumerable<Transaction> Transactions, int TotalCount)> GetAllAsync(
            string userId,
            int pageNumber,
            int pageSize,
            int? categoryId = null,
            DateTime? startDate = null,
            DateTime? endDate = null);
        Task<Transaction?> GetByIdAsync(int id, string userId);
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<Transaction?> UpdateAsync(Transaction transaction);
        Task<bool> DeleteAsync(int id, string userId);

        Task<(decimal TotalIncome, decimal TotalExpenses, decimal Balance)> GetStatisticsAsync(
        string userId,
        DateTime? startDate = null,
        DateTime? endDate = null);
    }
}