using eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.DeleteCategory;
using eCommerceOnlineShop.Catalog.Core.Models;
using eCommerceOnlineShop.Catalog.DAL.Data;
using eCommerceOnlineShop.Catalog.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace eCommerceOnlineShop.Catalog.Tests.Integration.BLL.UseCases.Categories.DeleteCategory
{
    public class DeleteCategoryCommandHandlerTests : IDisposable
    {
        private readonly CatalogDbContext _context;
        private readonly CategoryRepository _categoryRepository;
        private readonly DeleteCategoryCommandHandler _handler;

        public DeleteCategoryCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CatalogDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new CatalogDbContext(options);
            _categoryRepository = new CategoryRepository(_context);
            _handler = new DeleteCategoryCommandHandler(_categoryRepository);
        }

        [Fact]
        public async Task Handle_DeletesCategoryAndRelatedProducts()
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

            var command = new DeleteCategoryCommand { CategoryId = category.Id };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Null(await _context.Categories.FindAsync(category.Id));
            Assert.Empty(await _context.Products.Where(p => p.CategoryId == category.Id).ToListAsync());
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenCategoryNotFound()
        {
            // Arrange
            var command = new DeleteCategoryCommand { CategoryId = 999 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

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
