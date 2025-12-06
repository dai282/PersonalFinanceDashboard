using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PersonalFinance.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Income" or "Expense"
        public string? Icon { get; set; }
        public string? UserId { get; set; } // Nullable for default categories
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        //public User? User { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    }
}