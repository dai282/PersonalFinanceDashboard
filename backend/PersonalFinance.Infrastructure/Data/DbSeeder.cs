using PersonalFinance.Core.Entities;

namespace PersonalFinance.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDefaultCategories(ApplicationDbContext context)
        {
            try
            {
                if (context.Categories.Any())
                    return; // Already seeded

                var defaultCategories = new List<Category>
            {
                // Expense categories
                new Category { Name = "Food & Dining", Type = "Expense", Icon = "🍔", UserId = null },
                new Category { Name = "Transportation", Type = "Expense", Icon = "🚗", UserId = null },
                new Category { Name = "Entertainment", Type = "Expense", Icon = "🎬", UserId = null },
                new Category { Name = "Shopping", Type = "Expense", Icon = "🛍️", UserId = null },
                new Category { Name = "Bills & Utilities", Type = "Expense", Icon = "💡", UserId = null },
                new Category { Name = "Healthcare", Type = "Expense", Icon = "🏥", UserId = null },
                new Category { Name = "Education", Type = "Expense", Icon = "📚", UserId = null },
                new Category { Name = "Housing", Type = "Expense", Icon = "🏠", UserId = null },
                new Category { Name = "Personal Care", Type = "Expense", Icon = "💇", UserId = null },
                new Category { Name = "Other Expenses", Type = "Expense", Icon = "📦", UserId = null },
                
                // Income categories
                new Category { Name = "Salary", Type = "Income", Icon = "💰", UserId = null },
                new Category { Name = "Freelance", Type = "Income", Icon = "💼", UserId = null },
                new Category { Name = "Investment", Type = "Income", Icon = "📈", UserId = null },
                new Category { Name = "Gift", Type = "Income", Icon = "🎁", UserId = null },
                new Category { Name = "Other Income", Type = "Income", Icon = "💵", UserId = null }
            };

                context.Categories.AddRange(defaultCategories);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log but don't throw - let app continue even if seeding fails
                Console.WriteLine($"Error seeding categories: {ex.Message}");
            }


        }
    }
}