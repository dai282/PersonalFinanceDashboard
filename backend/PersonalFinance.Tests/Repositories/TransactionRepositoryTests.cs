using FluentAssertions;
using PersonalFinance.Core.Entities;
using PersonalFinance.Infrastructure.Repositories;
using PersonalFinance.Tests.Helpers;
using Xunit;

namespace PersonalFinance.Tests.Repositories
{
    public class TransactionRepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnUserTransactionsOnly()
        {
            // Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new TransactionRepository(context);

            var category = new Category { Name = "Food", Type = "Expense" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var user1Transaction = new Transaction
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 50,
                Description = "User 1",
                TransactionDate = DateTime.UtcNow,
                Type = "Expense"
            };

            var user2Transaction = new Transaction
            {
                UserId = 2,
                CategoryId = category.Id,
                Amount = 100,
                Description = "User 2",
                TransactionDate = DateTime.UtcNow,
                Type = "Expense"
            };

            context.Transactions.AddRange(user1Transaction, user2Transaction);
            await context.SaveChangesAsync();

            // Act
            var (transactions, totalCount) = await repository.GetAllAsync(1, 1, 10);

            // Assert
            totalCount.Should().Be(1);
            transactions.Should().HaveCount(1);
            transactions.First().Description.Should().Be("User 1");
        }

        [Fact]
        public async Task GetStatisticsAsync_ShouldCalculateCorrectly()
        {
            // Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new TransactionRepository(context);

            var category = new Category { Name = "Food", Type = "Expense" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var income = new Transaction
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 3000,
                Type = "Income",
                TransactionDate = DateTime.UtcNow
            };

            var expense = new Transaction
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 500,
                Type = "Expense",
                TransactionDate = DateTime.UtcNow
            };

            context.Transactions.AddRange(income, expense);
            await context.SaveChangesAsync();

            // Act
            var (totalIncome, totalExpenses, balance) = await repository.GetStatisticsAsync(1);

            // Assert
            totalIncome.Should().Be(3000);
            totalExpenses.Should().Be(500);
            balance.Should().Be(2500);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddTransaction()
        {
            // Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new TransactionRepository(context);

            var category = new Category { Name = "Food", Type = "Expense" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var transaction = new Transaction
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 50,
                Description = "Lunch",
                TransactionDate = DateTime.UtcNow,
                Type = "Expense"
            };

            // Act
            var result = await repository.CreateAsync(transaction);

            // Assert
            result.Id.Should().BeGreaterThan(0);
            result.Amount.Should().Be(50);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveTransaction()
        {
            // Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new TransactionRepository(context);

            var category = new Category { Name = "Food", Type = "Expense" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var transaction = new Transaction
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 50,
                Type = "Expense",
                TransactionDate = DateTime.UtcNow
            };

            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.DeleteAsync(transaction.Id, 1);

            // Assert
            result.Should().BeTrue();
            var deleted = await context.Transactions.FindAsync(transaction.Id);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_WrongUser_ShouldReturnFalse()
        {
            // Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new TransactionRepository(context);

            var category = new Category { Name = "Food", Type = "Expense" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var transaction = new Transaction
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 50,
                Type = "Expense",
                TransactionDate = DateTime.UtcNow
            };

            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.DeleteAsync(transaction.Id, 2); // Different user

            // Assert
            result.Should().BeFalse();
            var stillExists = await context.Transactions.FindAsync(transaction.Id);
            stillExists.Should().NotBeNull();
        }
    }
}