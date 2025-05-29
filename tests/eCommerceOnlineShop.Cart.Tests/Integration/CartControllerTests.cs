using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Text;
using System.Text.Json;

namespace eCommerceOnlineShop.Cart.Tests.Integration
{
    public class CartControllerTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory = factory;
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task GetCart_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/cart");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task AddItemToCart_ValidItem_ReturnsOk()
        {
            // Arrange
            var item = new
            {
                ProductId = 1,
                Quantity = 2
            };
            var content = new StringContent(
                JsonSerializer.Serialize(item),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/cart/items", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CartItemResponse>(responseContent);
            Assert.NotNull(result);
            Assert.Equal(item.ProductId, result.ProductId);
            Assert.Equal(item.Quantity, result.Quantity);
        }

        [Fact]
        public async Task AddItemToCart_InvalidItem_ReturnsBadRequest()
        {
            // Arrange
            var invalidItem = new
            {
                ProductId = -1, // Invalid product ID
                Quantity = 0    // Invalid quantity
            };
            var content = new StringContent(
                JsonSerializer.Serialize(invalidItem),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/cart/items", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RemoveItemFromCart_ValidItem_ReturnsOk()
        {
            // Arrange
            var productId = 1;

            // Act
            var response = await _client.DeleteAsync($"/api/v1/cart/items/{productId}");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RemoveItemFromCart_NonExistentItem_ReturnsNotFound()
        {
            // Arrange
            var nonExistentProductId = 999;

            // Act
            var response = await _client.DeleteAsync($"/api/v1/cart/items/{nonExistentProductId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private class CartItemResponse
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
