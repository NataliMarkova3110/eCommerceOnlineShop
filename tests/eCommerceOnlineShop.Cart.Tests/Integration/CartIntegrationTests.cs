using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using eCommerceOnlineShop.Cart.Core.Interfaces;
using eCommerceOnlineShop.Cart.Core.Models;
using eCommerceOnlineShop.Cart.DAL.Repositories;
using eCommerceOnlineShop.Cart.BLL.Services;

namespace eCommerceOnlineShop.Cart.Tests.Integration
{
    public class TestStartup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddSingleton<ICartRepository, CartRepository>();
            services.AddScoped<ICartService, CartService>();
        }

        public static void Configure(IApplicationBuilder app, IEndpointRouteBuilder endpoints)
        {
            app.UseHttpsRedirection();
            app.UseAuthorization();
            endpoints.MapControllers();
        }
    }

    public class CartIntegrationTests : IClassFixture<WebApplicationFactory<TestStartup>>, IDisposable
    {
        private readonly WebApplicationFactory<TestStartup> _factory;
        private readonly HttpClient _client;
        private readonly string _testDbPath;
        private readonly CartRepository _repository;

        public CartIntegrationTests(WebApplicationFactory<TestStartup> factory)
        {
            _testDbPath = Path.Combine(Path.GetTempPath(), $"test-cart-{Guid.NewGuid()}.db");
            var repository = new CartRepository(new ConfigurationBuilder()
                .AddInMemoryCollection(
                [
                    new KeyValuePair<string, string?>("ConnectionStrings:LiteDb", _testDbPath)
                ])
                .Build());

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.Remove(services.Single(d => d.ServiceType == typeof(ICartRepository)));
                    services.AddSingleton<ICartRepository>(repository);
                });
            });

            _client = _factory.CreateClient();
            _repository = repository;
        }

        public void Dispose()
        {
            if (File.Exists(_testDbPath))
            {
                File.Delete(_testDbPath);
            }
        }

        [Fact]
        public async Task CompleteFlow_AddItemToCart_ShouldWork()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var item = new CartItem
            {
                Id = 1,
                ProductId = 1,
                ProductName = "Test Product",
                Price = 10.99m,
                Quantity = 2
            };

            // Act - Add item to cart through API
            var response = await _client.PostAsJsonAsync($"/api/cart/{cartId}/items", item);
            response.EnsureSuccessStatusCode();
            var addedItem = await response.Content.ReadFromJsonAsync<CartItem>();

            // Assert - Verify item was added through API
            Assert.NotNull(addedItem);
            Assert.Equal(item.ProductName, addedItem.ProductName);
            Assert.Equal(item.Price, addedItem.Price);
            Assert.Equal(item.Quantity, addedItem.Quantity);

            // Verify cart exists in database
            var cart = await _repository.GetCartAsync(cartId.ToString());
            Assert.NotNull(cart);
            Assert.Single(cart.Items);
            Assert.Equal(item.Id, cart.Items[0].Id);
            Assert.Equal(item.ProductName, cart.Items[0].ProductName);
            Assert.Equal(item.Price, cart.Items[0].Price);
            Assert.Equal(item.Quantity, cart.Items[0].Quantity);
        }

        [Fact]
        public async Task CompleteFlow_GetCartItems_ShouldWork()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var item = new CartItem
            {
                Id = 1,
                ProductId = 1,
                ProductName = "Test Product",
                Price = 10.99m,
                Quantity = 2
            };

            // Create cart with item directly in database
            var cart = new CartEntity { Id = cartId, CartKey = cartId.ToString() };
            cart.Items.Add(item);
            await _repository.CreateCartAsync(cart);

            // Act - Get items through API
            var response = await _client.GetAsync($"/api/cart/{cartId}/items");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadFromJsonAsync<List<CartItem>>();

            // Assert
            Assert.NotNull(items);
            Assert.Single(items);
            Assert.Equal(item.Id, items[0].Id);
            Assert.Equal(item.ProductName, items[0].ProductName);
            Assert.Equal(item.Price, items[0].Price);
            Assert.Equal(item.Quantity, items[0].Quantity);
        }

        [Fact]
        public async Task CompleteFlow_RemoveItemFromCart_ShouldWork()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var item = new CartItem
            {
                Id = 1,
                ProductId = 1,
                ProductName = "Test Product",
                Price = 10.99m,
                Quantity = 2
            };

            // Create cart with item directly in database
            var cart = new CartEntity { Id = cartId, CartKey = cartId.ToString() };
            cart.Items.Add(item);
            await _repository.CreateCartAsync(cart);

            // Act - Remove item through API
            var response = await _client.DeleteAsync($"/api/cart/{cartId}/items/{item.Id}");
            response.EnsureSuccessStatusCode();

            // Assert - Verify item was removed from database
            var updatedCart = await _repository.GetCartAsync(cartId.ToString());
            Assert.NotNull(updatedCart);
            Assert.Empty(updatedCart.Items);
        }
    }
}
