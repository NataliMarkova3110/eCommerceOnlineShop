using eCommerceOnlineShop.Catalog.Core.Models;
using eCommerceOnlineShop.Catalog.DAL.Data;
using eCommerceOnlineShop.Catalog.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace eCommerceOnlineShop.Catalog.Tests.Unit.DAL.Repositories
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly CatalogDbContext _context;
        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CatalogDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new CatalogDbContext(options);
            _repository = new ProductRepository(_context);
        }

        [Fact]
        public async Task GetProductAsync_ReturnsProduct_WhenExists()
        {
            // Arrange
            var category = new Category { Name = "Test Category" };
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.99m,
                CategoryId = category.Id,
                Amount = 100
            };
            await _context.Categories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetProductAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(product.Description, result.Description);
            Assert.Equal(product.Price, result.Price);
            Assert.Equal(category.Id, result.CategoryId);
        }

        [Fact]
        public async Task GetProductAsync_ReturnsNull_WhenNotExists()
        {
            // Act
            var result = await _repository.GetProductAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsAllProducts()
        {
            // Arrange
            var category = new Category { Name = "Test Category" };
            var products = new List<Product>
            {
                new() { Name = "Product 1", CategoryId = category.Id, Price = 10.99m, Amount = 100 },
                new() { Name = "Product 2", CategoryId = category.Id, Price = 20.99m, Amount = 100 }
            };
            await _context.Categories.AddAsync(category);
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetProductsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Name == "Product 1");
            Assert.Contains(result, p => p.Name == "Product 2");
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ReturnsProductsForCategory()
        {
            // Arrange
            var category1 = new Category { Name = "Category 1" };
            var category2 = new Category { Name = "Category 2" };
            var products = new List<Product>
            {
                new() { Name = "Product 1", CategoryId = category1.Id, Price = 10.99m, Amount = 100 },
                new() { Name = "Product 2", CategoryId = category1.Id, Price = 20.99m, Amount = 100 },
                new() { Name = "Product 3", CategoryId = category2.Id, Price = 30.99m, Amount = 100 }
            };
            await _context.Categories.AddRangeAsync(category1, category2);
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetProductsByCategoryAsync(category1.Id);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Name == "Product 1");
            Assert.Contains(result, p => p.Name == "Product 2");
            Assert.DoesNotContain(result, p => p.Name == "Product 3");
        }

        [Fact]
        public async Task CreateProductAsync_CreatesProduct()
        {
            // Arrange
            var category = new Category { Name = "Test Category" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product
            {
                Name = "New Product",
                Description = "New Description",
                Price = 19.99m,
                CategoryId = category.Id,
                Amount = 50
            };

            // Act
            var result = await _repository.CreateProductAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(product.Description, result.Description);
            Assert.Equal(product.Price, result.Price);
            Assert.Equal(category.Id, result.CategoryId);
            var savedProduct = await _context.Products.FindAsync(result.Id);
            Assert.NotNull(savedProduct);
            Assert.Equal(product.Name, savedProduct.Name);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdatesProduct()
        {
            // Arrange
            var category = new Category { Name = "Test Category" };
            var product = new Product
            {
                Name = "Original Product",
                Description = "Original Description",
                Price = 10.99m,
                CategoryId = category.Id,
                Amount = 100
            };
            await _context.Categories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            product.Name = "Updated Product";
            product.Description = "Updated Description";
            product.Price = 15.99m;

            // Act
            var result = await _repository.UpdateProductAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Product", result.Name);
            Assert.Equal("Updated Description", result.Description);
            Assert.Equal(15.99m, result.Price);
            var updatedProduct = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Product", updatedProduct.Name);
        }

        [Fact]
        public async Task DeleteProductAsync_DeletesProduct()
        {
            // Arrange
            var category = new Category { Name = "Test Category" };
            var product = new Product { Name = "To Delete", CategoryId = category.Id, Price = 10.99m, Amount = 100 };
            await _context.Categories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteProductAsync(product.Id);

            // Assert
            Assert.True(result);
            var deletedProduct = await _context.Products.FindAsync(product.Id);
            Assert.Null(deletedProduct);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
