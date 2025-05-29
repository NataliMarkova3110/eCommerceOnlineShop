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
        public async Task GetCartItemsAsync_WhenCartExists_ReturnsItemsAsync()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            List<CartItem> expectedItems = [new() { Id = 1, ProductName = "Test Item", Price = 10.99m, Quantity = 2 }];
            var cart = new CartEntity { Id = cartId, CartKey = cartId.ToString(), Items = expectedItems };

            _mockRepository.Setup(r => r.GetCartAsync(cartId.ToString()))
                .ReturnsAsync(cart);

            // Act
            var result = await _cartService.GetCartItemsAsync(cartId.ToString());

            // Assert
            Assert.Equal(expectedItems, result);
        }

        [Fact]
        public async Task GetCartItemsAsync_WhenCartDoesNotExist_ReturnsEmptyListAsync()
        {
            // Arrange
            var cartId = Guid.NewGuid().ToString();
            _mockRepository.Setup(r => r.GetCartAsync(cartId))
                .ReturnsAsync((CartEntity)null);

            // Act
            var result = await _cartService.GetCartItemsAsync(cartId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddItemToCartAsync_WhenCartDoesNotExist_CreatesNewCartAsync()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var newItem = new CartItem { Id = 1, ProductName = "New Item", Price = 15.99m, Quantity = 1 };

            _mockRepository.Setup(r => r.GetCartAsync(cartId.ToString()))
                .ReturnsAsync((CartEntity)null);


            // Act
            var result = await _cartService.AddItemToCartAsync(cartId.ToString(), newItem);

            // Assert
            Assert.Equal(newItem, result);
            _mockRepository.Verify(r => r.CreateCartAsync(It.Is<CartEntity>(c =>
                c.Id == cartId &&
                c.Items.Count == 1 &&
                c.Items[0] == newItem)), Times.Once);
        }

        [Fact]
        public async Task AddItemToCartAsync_WhenItemExists_UpdatesQuantityAsync()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var cartKey = cartId.ToString();
            var existingItem = new CartItem { Id = 1, ProductName = "Existing Item", Price = 10.99m, Quantity = 2 };
            var cart = new CartEntity { Id = cartId, CartKey = cartKey, Items = [existingItem] };
            var newItem = new CartItem { Id = 1, ProductName = "Existing Item", Price = 10.99m, Quantity = 3 };

            _mockRepository.Setup(r => r.GetCartAsync(cartKey))
                .ReturnsAsync(cart);

            // Act
            var result = await _cartService.AddItemToCartAsync(cartKey, newItem);

            // Assert
            Assert.Equal(newItem, result);
            _mockRepository.Verify(r => r.UpdateCartAsync(It.Is<CartEntity>(c =>
                c.Id == cartId &&
                c.Items.Count == 1 &&
                c.Items[0].Quantity == 5)), Times.Once);
        }

        [Fact]
        public async Task RemoveItemFromCartAsync_WhenItemExists_RemovesItemAsync()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var cartKey = cartId.ToString();
            var itemId = 1;
            var itemToRemove = new CartItem { Id = itemId, ProductName = "Item to Remove", Price = 10.99m, Quantity = 1 };
            var cart = new CartEntity { Id = cartId, CartKey = cartKey, Items = [itemToRemove] };

            _mockRepository.Setup(r => r.GetCartAsync(cartKey))
                .ReturnsAsync(cart);

            // Act
            var result = await _cartService.RemoveItemFromCartAsync(cartKey, itemId);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.UpdateCartAsync(It.Is<CartEntity>(c =>
                c.Id == cartId &&
                c.Items.Count == 0)), Times.Once);
        }

        [Fact]
        public async Task RemoveItemFromCartAsync_WhenItemDoesNotExist_ReturnsFalseAsync()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var cartKey = cartId.ToString();
            var itemId = 2;
            var cart = new CartEntity { Id = cartId, CartKey = cartKey, Items = [] };

            _mockRepository.Setup(r => r.GetCartAsync(cartKey))
                .ReturnsAsync(cart);

            // Act
            var result = await _cartService.RemoveItemFromCartAsync(cartKey, itemId);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.UpdateCartAsync(It.IsAny<CartEntity>()), Times.Never);
        }
    }
}
