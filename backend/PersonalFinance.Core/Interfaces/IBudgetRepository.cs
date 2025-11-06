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
        Task<IEnumerable<Budget>> GetAllAsync(int userId, int? month = null, int? year = null);
        Task<Budget?> GetByIdAsync(int id, int userId);
        Task<Budget> CreateAsync(Budget budget);
        Task<Budget?> UpdateAsync(Budget budget);
        Task<bool> DeleteAsync(int id, int userId);
        Task<decimal> GetActualSpendingAsync(int userId, int categoryId, int month, int year);
    }
}
