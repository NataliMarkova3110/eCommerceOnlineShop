using eCommerceOnlineShop.Catalog.Core.Models;
using eCommerceOnlineShop.Catalog.DAL.Data;
using eCommerceOnlineShop.Catalog.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace eCommerceOnlineShop.Catalog.Tests.Unit.DAL.Repositories
{
    public class CategoryRepositoryTests : IDisposable
    {
        private readonly CatalogDbContext _context;
        private readonly CategoryRepository _repository;

        public CategoryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CatalogDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new CatalogDbContext(options);
            _repository = new CategoryRepository(_context);
        }

        [Fact]
        public async Task GetCategoryAsync_ReturnsCategory_WhenExistsAsync()
        {
            // Arrange
            var category = new Category { Name = "Test Category" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetCategoryAsync(category.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category.Name, result.Name);
        }

        [Fact]
        public async Task GetCategoryAsync_ReturnsNull_WhenNotExistsAsync()
        {
            // Act
            var result = await _repository.GetCategoryAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsAllCategoriesAsync()
        {
            // Arrange
            var categories = new List<Category>
            {
                new() { Name = "Category 1" },
                new() { Name = "Category 2" }
            };
            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetCategoriesAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "Category 1");
            Assert.Contains(result, c => c.Name == "Category 2");
        }

        [Fact]
        public async Task CreateCategoryAsync_CreatesCategoryAsync()
        {
            // Arrange
            var category = new Category { Name = "New Category" };

            // Act
            var result = await _repository.CreateCategoryAsync(category);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category.Name, result.Name);
            var savedCategory = await _context.Categories.FindAsync(result.Id);
            Assert.NotNull(savedCategory);
            Assert.Equal(category.Name, savedCategory.Name);
        }

        [Fact]
        public async Task UpdateCategoryAsync_UpdatesCategoryAsync()
        {
            // Arrange
            var category = new Category { Name = "Original Name" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            category.Name = "Updated Name";

            // Act
            var result = await _repository.UpdateCategoryAsync(category);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
            var updatedCategory = await _context.Categories.FindAsync(category.Id);
            Assert.NotNull(updatedCategory);
            Assert.Equal("Updated Name", updatedCategory.Name);
        }

        [Fact]
        public async Task DeleteCategoryAsync_DeletesCategoryAndRelatedProductsAsync()
        {
            // Arrange
            var category = new Category { Name = "Test Category" };
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.99m,
                Category = category,
                Amount = 100,
                CategoryId = category.Id
            };

            await _context.Categories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteCategoryAsync(category.Id);

            // Assert
            Assert.True(result);
            Assert.Null(await _context.Categories.FindAsync(category.Id));
            Assert.Empty(await _context.Products.Where(p => p.CategoryId == category.Id).ToListAsync());
        }

        [Fact]
        public async Task DeleteCategoryAsync_ReturnsFalse_WhenCategoryNotFoundAsync()
        {
            // Act
            var result = await _repository.DeleteCategoryAsync(999);

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
