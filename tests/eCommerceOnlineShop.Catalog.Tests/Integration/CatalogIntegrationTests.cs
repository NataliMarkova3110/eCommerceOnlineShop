using eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.AddCategory;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.AddProduct;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.DeleteProduct;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProducts;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.UpdateProduct;
using eCommerceOnlineShop.Catalog.Core.Models;
using eCommerceOnlineShop.Catalog.DAL.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace eCommerceOnlineShop.Catalog.Tests.Integration
{
    public class CatalogIntegrationTests : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly CatalogDbContext _context;
        private readonly IMediator _mediator;

        public CatalogIntegrationTests()
        {
            var services = new ServiceCollection();

            services.AddDbContext<CatalogDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CatalogDb_IntegrationTest;Trusted_Connection=True;MultipleActiveResultSets=true"));

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddProductCommand).Assembly));

            services.AddScoped<IValidator<AddCategoryCommand>, AddCategoryCommandValidator>();
            services.AddScoped<IValidator<AddProductCommand>, AddProductCommandValidator>();
            services.AddScoped<IValidator<UpdateProductCommand>, UpdateProductCommandValidator>();
            services.AddScoped<IValidator<DeleteProductCommand>, DeleteProductCommandValidator>();

            _serviceProvider = services.BuildServiceProvider();
            _context = _serviceProvider.GetRequiredService<CatalogDbContext>();
            _mediator = _serviceProvider.GetRequiredService<IMediator>();

            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task CatalogOperations_CompleteFlow_Success()
        {
            var addCategoryCommand = new AddCategoryCommand
            {
                Name = "Test Category",
                Description = "Test Category Description"
            };

            var category = await _mediator.Send(addCategoryCommand);
            Assert.NotNull(category);
            Assert.Equal("Test Category", category.Name);

            var products = new List<Product>();
            for (int i = 1; i <= 3; i++)
            {
                var addProductCommand = new AddProductCommand
                {
                    Name = $"Product {i}",
                    Description = $"Description {i}",
                    Price = 10.99m * i,
                    CategoryId = category.Id
                };

                var product = await _mediator.Send(addProductCommand);
                Assert.NotNull(product);
                Assert.Equal(addProductCommand.Name, product.Name);
                products.Add(product);
            }

            var getProductsCommand = new GetProductsCommand();
            var allProducts = await _mediator.Send(getProductsCommand);
            Assert.Equal(3, allProducts.Count());

            var deleteProductCommand = new DeleteProductCommand { ProductId = products[0].Id };
            var deleteResult = await _mediator.Send(deleteProductCommand);
            Assert.True(deleteResult);

            allProducts = await _mediator.Send(getProductsCommand);
            Assert.Equal(2, allProducts.Count());
            Assert.DoesNotContain(allProducts, p => p.Id == products[0].Id);

            var lastProduct = products.Last();
            var updateProductCommand = new UpdateProductCommand
            {
                Id = lastProduct.Id,
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 99.99m,
                CategoryId = category.Id
            };

            var updatedProduct = await _mediator.Send(updateProductCommand);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Product", updatedProduct.Name);
            Assert.Equal("Updated Description", updatedProduct.Description);
            Assert.Equal(99.99m, updatedProduct.Price);

            allProducts = await _mediator.Send(getProductsCommand);
            Assert.Equal(2, allProducts.Count());
            var updatedProductInDb = allProducts.First(p => p.Id == lastProduct.Id);
            Assert.Equal("Updated Product", updatedProductInDb.Name);
            Assert.Equal("Updated Description", updatedProductInDb.Description);
            Assert.Equal(99.99m, updatedProductInDb.Price);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _serviceProvider.Dispose();
        }
    }
}