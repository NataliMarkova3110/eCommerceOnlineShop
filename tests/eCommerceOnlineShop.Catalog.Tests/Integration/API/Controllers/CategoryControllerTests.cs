using eCommerceOnlineShop.Catalog.API.Controllers;
using eCommerceOnlineShop.Catalog.API.Models;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.AddCategory;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.UpdateCategory;
using eCommerceOnlineShop.Catalog.Core.Models;
using eCommerceOnlineShop.Catalog.DAL.Data;
using eCommerceOnlineShop.Catalog.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using MediatR;

namespace eCommerceOnlineShop.Catalog.Tests.Integration.API.Controllers
{
    public class CategoryControllerTests : IDisposable
    {
        private readonly CatalogDbContext _context;
        private readonly CategoryRepository _categoryRepository;
        private readonly CategoryController _controller;
        private readonly Mock<LinkGenerator> _linkGeneratorMock;
        private readonly Mock<IMediator> _mediatorMock;

        public CategoryControllerTests()
        {
            var options = new DbContextOptionsBuilder<CatalogDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new CatalogDbContext(options);
            _categoryRepository = new CategoryRepository(_context);
            _linkGeneratorMock = new Mock<LinkGenerator>();
            _mediatorMock = new Mock<IMediator>();


            _controller = new CategoryController(
                _mediatorMock.Object,
                _linkGeneratorMock.Object);
        }

        [Fact]
        public async Task GetCategories_ReturnsCorrectLinks()
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
            var result = await _controller.GetCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ResourceResponse<IEnumerable<Category>>>(okResult.Value);

            Assert.Contains(response.Links, l => l.Rel == "self" && l.Method == "GET");
            Assert.Contains(response.Links, l => l.Rel == "create-category" && l.Method == "POST");
        }

        [Fact]
        public async Task GetCategory_ReturnsCorrectLinks()
        {
            // Arrange
            var category = new Category { Name = "Test Category" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetCategory(category.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ResourceResponse<Category>>(okResult.Value);

            Assert.Contains(response.Links, l => l.Rel == "self" && l.Method == "GET");
            Assert.Contains(response.Links, l => l.Rel == "update-category" && l.Method == "PUT");
            Assert.Contains(response.Links, l => l.Rel == "delete-category" && l.Method == "DELETE");
        }

        [Fact]
        public async Task AddCategory_ReturnsCorrectLinks()
        {
            // Arrange
            var command = new AddCategoryCommand
            {
                Name = "New Category",
                Description = "Test Description"
            };

            // Act
            var result = await _controller.AddCategory(command);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var response = Assert.IsType<ResourceResponse<Category>>(createdResult.Value);

            Assert.Contains(response.Links, l => l.Rel == "self" && l.Method == "GET");
            Assert.Contains(response.Links, l => l.Rel == "update-category" && l.Method == "PUT");
            Assert.Contains(response.Links, l => l.Rel == "delete-category" && l.Method == "DELETE");
        }

        [Fact]
        public async Task UpdateCategory_ReturnsCorrectLinks()
        {
            // Arrange
            var category = new Category { Name = "Test Category" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var command = new UpdateCategoryCommand
            {
                Id = category.Id,
                Name = "Updated Category",
                Description = "Updated Description"
            };

            // Act
            var result = await _controller.UpdateCategory(category.Id, command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ResourceResponse<Category>>(okResult.Value);

            Assert.Contains(response.Links, l => l.Rel == "self" && l.Method == "GET");
            Assert.Contains(response.Links, l => l.Rel == "update-category" && l.Method == "PUT");
            Assert.Contains(response.Links, l => l.Rel == "delete-category" && l.Method == "DELETE");
        }

        [Fact]
        public async Task DeleteCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Act
            var result = await _controller.DeleteCategory(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.ExecuteResultAsync);
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
            Assert.IsType<NoContentResult>(result.ExecuteResultAsync);
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