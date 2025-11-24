using FluentAssertions;
using PersonalFinance.Core.Entities;
using PersonalFinance.Infrastructure.Repositories;
using PersonalFinance.Tests.Helpers;

namespace PersonalFinance.Tests.Repositories
{
    public class BudgetRepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnDefaultBudgets()
        {
            //Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new BudgetRepository(context);

            var category = new Category { Name = "Food", Type = "Expense" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();


            var budget1 = new Budget { UserId = 1, CategoryId = category.Id, Amount = 500, Month = 11, Year = 2025 };
            var budget2 = new Budget { UserId = 1, CategoryId = category.Id, Amount = 700, Month = 11, Year = 2025 };
            var otherBudget = new Budget { UserId = 2, CategoryId = category.Id, Amount = 900, Month = 11, Year = 2025 };

            context.Budgets.AddRange(budget1, budget2, otherBudget);
            await context.SaveChangesAsync();

            // Act
            //Get all Budgets for user with ID 1
            var result = await repository.GetAllAsync(1);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(c => c.Month == 11);
            result.Should().Contain(c => c.Year == 2025);
            result.Should().NotContain(c => c.Amount == 900);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddBudgetToDatabase()
        {
            //Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new BudgetRepository(context);
            var category = new Category { Name = "Food", Type = "Expense" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var Budget = new Budget
            {
                CategoryId = category.Id,
                Amount = 900,
                Month = 11,
                Year = 2025
            };

            //Act
            var result = await repository.CreateAsync(Budget);

            //Assert

            //Make sure the Budget is not null
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Amount.Should().Be(900);

            //Check if the Budget is in the database
            var savedBudget = await context.Budgets.FindAsync(Budget.Id);
            savedBudget.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectBudget()
        {
            // Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new BudgetRepository(context);
            var category = new Category { Name = "Food", Type = "Expense" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var budget1 = new Budget { UserId = 1, CategoryId = category.Id, Amount = 500, Month = 11, Year = 2015 };

            context.Budgets.Add(budget1);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(budget1.Id, 1);

            // Assert
            result.Should().NotBeNull();
            result!.Year.Should().Be(2015);
        }

        public async Task DeleteAsync_ShouldRemoveBudget()
        {
            // Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new BudgetRepository(context);

            var category = new Category { Name = "Food", Type = "Expense" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var budget = new Budget
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 500,
                Month = 11,
                Year = 2024
            };

            context.Budgets.Add(budget);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.DeleteAsync(budget.Id, 1);

            // Assert
            result.Should().BeTrue();
            var deleted = await context.Budgets.FindAsync(budget.Id);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task GetActualSpendingAsync_ShouldCalculateCorrectly()
        {
            // Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new BudgetRepository(context);

            var category = new Category { Name = "Food", Type = "Expense" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var transaction1 = new Transaction
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 100,
                Type = "Expense",
                TransactionDate = new DateTime(2024, 11, 5)
            };

            var transaction2 = new Transaction
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 150,
                Type = "Expense",
                TransactionDate = new DateTime(2024, 11, 15)
            };

            // Different month - should not be counted
            var transaction3 = new Transaction
            {
                UserId = 1,
                CategoryId = category.Id,
                Amount = 200,
                Type = "Expense",
                TransactionDate = new DateTime(2024, 12, 5)
            };

            context.Transactions.AddRange(transaction1, transaction2, transaction3);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetActualSpendingAsync(1, category.Id, 11, 2024);

            // Assert
            result.Should().Be(250); // Only Nov transactions
        }
    }
}