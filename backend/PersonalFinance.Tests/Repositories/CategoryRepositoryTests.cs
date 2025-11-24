using FluentAssertions;
using PersonalFinance.Core.Entities;
using PersonalFinance.Infrastructure.Repositories;
using PersonalFinance.Tests.Helpers;

namespace PersonalFinance.Tests.Repositories
{
    public class CategoryRepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnDefaultCategories()
        {
            //Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new CategoryRepository(context);


            var defaultCategory = new Category { Name = "Food", Type = "Expense", UserId = null };
            var userCategory = new Category { Name = "Custom", Type = "Expense", UserId = 1 };
            var otherUserCategory = new Category { Name = "Other", Type = "Expense", UserId = 2 };

            context.Categories.AddRange(defaultCategory, userCategory, otherUserCategory);
            await context.SaveChangesAsync();

            // Act
            //Get all categories for user with ID 1
            var result = await repository.GetAllAsync(1);

            // Assert
            result.Should().HaveCount(2); // Default + user's custom
            result.Should().Contain(c => c.Name == "Food");
            result.Should().Contain(c => c.Name == "Custom");
            result.Should().NotContain(c => c.Name == "Other");
        }

        [Fact]
        public async Task CreateAsync_ShouldAddCategoryToDatabase()
        {
            //Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new CategoryRepository(context);

            var category = new Category
            {
                Name = "Potato",
                Type = "Income",
                UserId = 1
            };

            //Act
            var result = await repository.CreateAsync(category);

            //Assert

            //Make sure the category is not null
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Potato");

            //Check if the category is in the database
            var savedCategory = await context.Categories.FindAsync(category.Id);
            savedCategory.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectCategory()
        {
            // Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new CategoryRepository(context);

            var category = new Category { Name = "Food", Type = "Expense", UserId = null };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(category.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Food");
        }

        [Fact]
        public async Task DeleteAsync_DefaultCategory_ShouldReturnFalse()
        {
            // Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new CategoryRepository(context);

            var defaultCategory = new Category { Name = "Food", Type = "Expense", UserId = null };
            context.Categories.Add(defaultCategory);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.DeleteAsync(defaultCategory.Id);

            // Assert
            //Default categories should not be deletable
            result.Should().BeFalse();
            var stillExists = await context.Categories.FindAsync(defaultCategory.Id);
            stillExists.Should().NotBeNull(); // Should not be deleted
        }

        [Fact]
        public async Task DeleteAsync_UserCategory_ShouldReturnTrue()
        {
            // Arrange
            var context = TestDbContext.CreateInMemoryContext();
            var repository = new CategoryRepository(context);

            var userCategory = new Category { Name = "Custom", Type = "Expense", UserId = 1 };
            context.Categories.Add(userCategory);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.DeleteAsync(userCategory.Id);

            // Assert
            result.Should().BeTrue();
            var deleted = await context.Categories.FindAsync(userCategory.Id);
            deleted.Should().BeNull();
        }

    }
}
