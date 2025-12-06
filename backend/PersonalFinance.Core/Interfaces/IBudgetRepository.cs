using PersonalFinance.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinance.Core.Interfaces
{
    public interface IBudgetRepository
    {
        Task<IEnumerable<Budget>> GetAllAsync(string userId, int? month = null, int? year = null);
        Task<Budget?> GetByIdAsync(int id, string userId);
        Task<Budget> CreateAsync(Budget budget);
        Task<Budget?> UpdateAsync(Budget budget);
        Task<bool> DeleteAsync(int id, string userId);
        Task<decimal> GetActualSpendingAsync(string userId, int categoryId, int month, int year);
    }
}