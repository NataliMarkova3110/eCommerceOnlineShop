using LiteDB;
using Microsoft.Extensions.Configuration;
using Moq;
using eCommerceOnlineShop.Cart.DAL.Repositories;
using eCommerceOnlineShop.Cart.Core.Models;

namespace eCommerceOnlineShop.Cart.Tests.Unit
{
    public class CartRepositoryTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<LiteDatabase> _mockDatabase;
        private readonly Mock<ILiteCollection<CartEntity>> _mockCollection;
        private readonly CartRepository _repository;

        public CartRepositoryTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c.GetConnectionString("LiteDb")).Returns("test-connection-string");

            _mockDatabase = new Mock<LiteDatabase>("test-connection-string");
            _mockCollection = new Mock<ILiteCollection<CartEntity>>();
            _mockDatabase.Setup(d => d.GetCollection<CartEntity>("carts", BsonAutoId.ObjectId)).Returns(_mockCollection.Object);

            _repository = new CartRepository(_mockConfig.Object);
        }

        [Fact]
        public async Task GetCartAsync_WhenCartExists_ReturnsCart()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var expectedCart = new CartEntity { Id = cartId };
            _mockCollection.Setup(c => c.FindOne(It.IsAny<BsonExpression>())).Returns(expectedCart);

            // Act
            var result = await _repository.GetCartAsync(cartId);

            // Assert
            Assert.Equal(expectedCart, result);
            _mockCollection.Verify(c => c.FindOne(It.IsAny<BsonExpression>()), Times.Once);
        }

        [Fact]
        public async Task CreateCartAsync_ShouldInsertCart()
        {
            // Arrange
            var cart = new CartEntity { Id = Guid.NewGuid() };
            _mockCollection.Setup(c => c.Insert(cart));

            // Act
            var result = await _repository.CreateCartAsync(cart);

            // Assert
            Assert.Equal(cart, result);
            _mockCollection.Verify(c => c.Insert(cart), Times.Once);
        }

        [Fact]
        public async Task UpdateCartAsync_ShouldUpdateCart()
        {
            // Arrange
            var cart = new CartEntity { Id = Guid.NewGuid() };
            _mockCollection.Setup(c => c.Update(cart)).Returns(true);

            // Act
            var result = await _repository.UpdateCartAsync(cart);

            // Assert
            Assert.Equal(cart, result);
            _mockCollection.Verify(c => c.Update(cart), Times.Once);
        }

        [Fact]
        public async Task DeleteCartAsync_WhenCartExists_ReturnsTrue()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            _mockCollection.Setup(c => c.DeleteMany(It.IsAny<BsonExpression>())).Returns(1);

            // Act
            var result = await _repository.DeleteCartAsync(cartId);

            // Assert
            Assert.True(result);
            _mockCollection.Verify(c => c.DeleteMany(It.IsAny<BsonExpression>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCartAsync_WhenCartDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            _mockCollection.Setup(c => c.DeleteMany(It.IsAny<BsonExpression>())).Returns(0);

            // Act
            var result = await _repository.DeleteCartAsync(cartId);

            // Assert
            Assert.False(result);
            _mockCollection.Verify(c => c.DeleteMany(It.IsAny<BsonExpression>()), Times.Once);
        }
    }
}