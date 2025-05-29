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
        public async Task GetCartAsync_WhenCartExists_ReturnsCartAsync()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var expectedCart = new CartEntity { Id = cartId, CartKey = cartId.ToString() };
            _mockCollection.Setup(c => c.FindOne(It.IsAny<BsonExpression>())).Returns(expectedCart);

            // Act
            await _repository.GetCartAsync(cartId.ToString());

            // Assert
            _mockCollection.Verify(c => c.FindOne(It.IsAny<BsonExpression>()), Times.Once);
        }

        [Fact]
        public async Task CreateCartAsync_ShouldInsertCartAsync()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var cart = new CartEntity { Id = cartId, CartKey = cartId.ToString() };
            _mockCollection.Setup(c => c.Insert(cart));

            // Act
            await _repository.CreateCartAsync(cart);

            // Assert
            _mockCollection.Verify(c => c.Insert(cart), Times.Once);
        }

        [Fact]
        public async Task UpdateCartAsync_ShouldUpdateCartAsync()
        {
            // Arrange
            var cart = new CartEntity { Id = Guid.NewGuid(), CartKey = Guid.NewGuid().ToString() };
            _mockCollection.Setup(c => c.Update(cart)).Returns(true);

            // Act
            await _repository.UpdateCartAsync(cart);

            // Assert
            _mockCollection.Verify(c => c.Update(cart), Times.Once);
        }

        [Fact]
        public async Task DeleteCartAsync_WhenCartExists_ReturnsTrueAsync()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            _mockCollection.Setup(c => c.DeleteMany(It.IsAny<BsonExpression>())).Returns(1);

            // Act
            var result = await _repository.DeleteCartAsync(cartId.ToString());

            // Assert
            Assert.True(result);
            _mockCollection.Verify(c => c.DeleteMany(It.IsAny<BsonExpression>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCartAsync_WhenCartDoesNotExist_ReturnsFalseAsync()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            _mockCollection.Setup(c => c.DeleteMany(It.IsAny<BsonExpression>())).Returns(0);

            // Act
            var result = await _repository.DeleteCartAsync(cartId.ToString());

            // Assert
            Assert.False(result);
            _mockCollection.Verify(c => c.DeleteMany(It.IsAny<BsonExpression>()), Times.Once);
        }
    }
}
