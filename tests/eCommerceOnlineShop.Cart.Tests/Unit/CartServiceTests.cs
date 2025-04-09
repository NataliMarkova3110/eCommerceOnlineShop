using Moq;
using eCommerceOnlineShop.Cart.BLL.Services;
using eCommerceOnlineShop.Cart.Core.Interfaces;
using eCommerceOnlineShop.Cart.Core.Models;

namespace eCommerceOnlineShop.Cart.Tests.Unit
{
    public class CartServiceTests
    {
        private readonly Mock<ICartRepository> _mockRepository;
        private readonly CartService _cartService;

        public CartServiceTests()
        {
            _mockRepository = new Mock<ICartRepository>();
            _cartService = new CartService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetCartItemsAsync_WhenCartExists_ReturnsItems()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            List<CartItem> expectedItems = [new() { Id = 1, Name = "Test Item", Price = 10.99m, Quantity = 2 }];
            var cart = new CartEntity { Id = cartId, Items = expectedItems };

            _mockRepository.Setup(r => r.GetCartAsync(cartId))
                .ReturnsAsync(cart);

            // Act
            var result = await _cartService.GetCartItemsAsync(cartId);

            // Assert
            Assert.Equal(expectedItems, result);
        }

        [Fact]
        public async Task GetCartItemsAsync_WhenCartDoesNotExist_ReturnsEmptyList()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetCartAsync(cartId))
                .ReturnsAsync((CartEntity)null);

            // Act
            var result = await _cartService.GetCartItemsAsync(cartId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddItemToCartAsync_WhenCartDoesNotExist_CreatesNewCart()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var newItem = new CartItem { Id = 1, Name = "New Item", Price = 15.99m, Quantity = 1 };

            _mockRepository.Setup(r => r.GetCartAsync(cartId))
                .ReturnsAsync((CartEntity)null);

            _mockRepository.Setup(r => r.CreateCartAsync(It.IsAny<CartEntity>()))
                .ReturnsAsync((CartEntity cart) => cart);

            // Act
            var result = await _cartService.AddItemToCartAsync(cartId, newItem);

            // Assert
            Assert.Equal(newItem, result);
            _mockRepository.Verify(r => r.CreateCartAsync(It.Is<CartEntity>(c =>
                c.Id == cartId &&
                c.Items.Count == 1 &&
                c.Items[0] == newItem)), Times.Once);
        }

        [Fact]
        public async Task AddItemToCartAsync_WhenItemExists_UpdatesQuantity()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var existingItem = new CartItem { Id = 1, Name = "Existing Item", Price = 10.99m, Quantity = 2 };
            var cart = new CartEntity { Id = cartId, Items = [existingItem] };
            var newItem = new CartItem { Id = 1, Name = "Existing Item", Price = 10.99m, Quantity = 3 };

            _mockRepository.Setup(r => r.GetCartAsync(cartId))
                .ReturnsAsync(cart);

            _mockRepository.Setup(r => r.UpdateCartAsync(It.IsAny<CartEntity>()))
                .ReturnsAsync((CartEntity c) => c);

            // Act
            var result = await _cartService.AddItemToCartAsync(cartId, newItem);

            // Assert
            Assert.Equal(newItem, result);
            _mockRepository.Verify(r => r.UpdateCartAsync(It.Is<CartEntity>(c =>
                c.Id == cartId &&
                c.Items.Count == 1 &&
                c.Items[0].Quantity == 5)), Times.Once);
        }

        [Fact]
        public async Task RemoveItemFromCartAsync_WhenItemExists_RemovesItem()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var itemId = 1;
            var itemToRemove = new CartItem { Id = itemId, Name = "Item to Remove", Price = 10.99m, Quantity = 1 };
            var cart = new CartEntity { Id = cartId, Items = [itemToRemove] };

            _mockRepository.Setup(r => r.GetCartAsync(cartId))
                .ReturnsAsync(cart);

            _mockRepository.Setup(r => r.UpdateCartAsync(It.IsAny<CartEntity>()))
                .ReturnsAsync((CartEntity c) => c);

            // Act
            var result = await _cartService.RemoveItemFromCartAsync(cartId, itemId);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.UpdateCartAsync(It.Is<CartEntity>(c =>
                c.Id == cartId &&
                c.Items.Count == 0)), Times.Once);
        }

        [Fact]
        public async Task RemoveItemFromCartAsync_WhenItemDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var itemId = 2;
            var cart = new CartEntity { Id = cartId, Items = [] };

            _mockRepository.Setup(r => r.GetCartAsync(cartId))
                .ReturnsAsync(cart);

            // Act
            var result = await _cartService.RemoveItemFromCartAsync(cartId, itemId);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.UpdateCartAsync(It.IsAny<CartEntity>()), Times.Never);
        }
    }
}