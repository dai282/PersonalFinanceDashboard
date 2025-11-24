using FluentAssertions;
using PersonalFinance.Core.Entities;
using PersonalFinance.Infrastructure.Repositories;
using PersonalFinance.Tests.Helpers;

namespace PersonalFinance.Tests.Repositories
{
    public class ReportsRepositoryTests
    {

        [Fact]
        public async Task GetMonthlySummaryAsync_ShouldCalculateCorrectly()
        {
            // Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new ReportsRepository(context);

            var category = new Category { Name = "Food", Type = "Expense" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var income = new Transaction
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 3000,
                Type = "Income",
                TransactionDate = new DateTime(2024, 11, 5)
            };

            var expense1 = new Transaction
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 500,
                Type = "Expense",
                TransactionDate = new DateTime(2024, 11, 10)
            };

            var expense2 = new Transaction
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 300,
                Type = "Expense",
                TransactionDate = new DateTime(2024, 11, 20)
            };

            // Different month - should not be counted
            var decemberTransaction = new Transaction
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 1000,
                Type = "Expense",
                TransactionDate = new DateTime(2024, 12, 5)
            };

            context.Transactions.AddRange(income, expense1, expense2, decemberTransaction);
            await context.SaveChangesAsync();

            // Act
            var (totalIncome, totalExpenses, balance, count) =
                await repository.GetMonthlySummaryAsync(1, 11, 2024);

            // Assert
            totalIncome.Should().Be(3000);
            totalExpenses.Should().Be(800);
            balance.Should().Be(2200);
            count.Should().Be(3);
        }

        [Fact]
        public async Task GetSpendingByCategoryAsync_ShouldGroupCorrectly()
        {
            // Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new ReportsRepository(context);

            var foodCategory = new Category { Name = "Food", Type = "Expense", Icon = "🍔" };
            var transportCategory = new Category { Name = "Transport", Type = "Expense", Icon = "🚗" };
            context.Categories.AddRange(foodCategory, transportCategory);
            await context.SaveChangesAsync();

            var foodExpense1 = new Transaction
            {
                UserId = 1,
                CategoryId = foodCategory.Id,
                Amount = 100,
                Type = "Expense",
                TransactionDate = new DateTime(2024, 11, 5)
            };

            var foodExpense2 = new Transaction
            {
                UserId = 1,
                CategoryId = foodCategory.Id,
                Amount = 150,
                Type = "Expense",
                TransactionDate = new DateTime(2024, 11, 10)
            };

            var transportExpense = new Transaction
            {
                UserId = 1,
                CategoryId = transportCategory.Id,
                Amount = 50,
                Type = "Expense",
                TransactionDate = new DateTime(2024, 11, 15)
            };

            context.Transactions.AddRange(foodExpense1, foodExpense2, transportExpense);
            await context.SaveChangesAsync();

            var startDate = new DateTime(2024, 11, 1);
            var endDate = new DateTime(2024, 11, 30);

            // Act
            var result = await repository.GetSpendingByCategoryAsync(1, startDate, endDate);
            var resultList = result.ToList();

            // Assert
            resultList.Should().HaveCount(2);

            var foodSpending = resultList.First(r => r.CategoryName == "Food");
            foodSpending.Amount.Should().Be(250);
            foodSpending.Percentage.Should().BeApproximately(83.33m, 0.01m);

            var transportSpending = resultList.First(r => r.CategoryName == "Transport");
            transportSpending.Amount.Should().Be(50);
            transportSpending.Percentage.Should().BeApproximately(16.67m, 0.01m);
        }

        [Fact]
        public async Task GetIncomeVsExpenseTrendsAsync_NoTransactions_ShouldReturnEmpty()
        {
            // Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new ReportsRepository(context);

            var startDate = new DateTime(2024, 11, 1);
            var endDate = new DateTime(2024, 11, 30);

            // Act
            var result = await repository.GetIncomeVsExpenseTrendsAsync(1, startDate, endDate);

            // Assert
            result.Should().BeEmpty();
        }


        [Fact]
        public async Task GetIncomeVsExpenseTrendsAsync_ShouldOrderByDate()
        {
            // Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new ReportsRepository(context);

            var category = new Category { Name = "Food", Type = "Expense" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var decTransaction = new Transaction
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 100,
                Type = "Expense",
                TransactionDate = new DateTime(2024, 12, 5)
            };

            var novTransaction = new Transaction
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 200,
                Type = "Expense",
                TransactionDate = new DateTime(2024, 11, 5)
            };

            context.Transactions.AddRange(decTransaction, novTransaction);
            await context.SaveChangesAsync();

            var startDate = new DateTime(2024, 11, 1);
            var endDate = new DateTime(2024, 12, 31);

            // Act
            var result = await repository.GetIncomeVsExpenseTrendsAsync(1, startDate, endDate);
            var resultList = result.ToList();

            // Assert
            resultList.Should().HaveCount(2);
            resultList[0].Month.Should().Be(11); // November first
            resultList[1].Month.Should().Be(12); // December second
        }
    }
}
