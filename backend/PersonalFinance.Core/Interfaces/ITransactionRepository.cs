using PersonalFinance.Core.Entities;

namespace PersonalFinance.Core.Interfaces
{
    public interface ITransactionRepository
    {
        Task<(IEnumerable<Transaction> Transactions, int TotalCount)> GetAllAsync(
            int userId,
            int pageNumber,
            int pageSize,
            int? categoryId = null,
            DateTime? startDate = null,
            DateTime? endDate = null);
        Task<Transaction?> GetByIdAsync(int id, int userId);
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<Transaction?> UpdateAsync(Transaction transaction);
        Task<bool> DeleteAsync(int id, int userId);

        Task<(decimal TotalIncome, decimal TotalExpenses, decimal Balance)> GetStatisticsAsync(
        int userId,
        DateTime? startDate = null,
        DateTime? endDate = null);
    }
}