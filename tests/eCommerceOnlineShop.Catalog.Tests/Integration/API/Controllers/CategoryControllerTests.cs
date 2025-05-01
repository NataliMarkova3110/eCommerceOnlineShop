using eCommerceOnlineShop.Catalog.API.Controllers;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.DeleteCategory;
using eCommerceOnlineShop.Catalog.Core.Models;
using eCommerceOnlineShop.Catalog.DAL.Data;
using eCommerceOnlineShop.Catalog.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace eCommerceOnlineShop.Catalog.Tests.Integration.API.Controllers
{
    public class CategoryControllerTests : IDisposable
    {
        private readonly CatalogDbContext _context;
        private readonly CategoryRepository _categoryRepository;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            var options = new DbContextOptionsBuilder<CatalogDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new CatalogDbContext(options);
            _categoryRepository = new CategoryRepository(_context);
            _controller = new CategoryController(new MediatR.Mediator(type => null));
        }

        [Fact]
        public async Task DeleteCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Act
            var result = await _controller.DeleteCategory(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task DeleteCategory_DeletesCategoryAndRelatedProducts()
        {
            // Arrange
            var category = new Category { Name = "Test Category" };
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.99m,
                Category = category
            };

            await _context.Categories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteCategory(category.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.True((bool)okResult.Value!);
            Assert.Null(await _context.Categories.FindAsync(category.Id));
            Assert.Empty(await _context.Products.Where(p => p.CategoryId == category.Id).ToListAsync());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}