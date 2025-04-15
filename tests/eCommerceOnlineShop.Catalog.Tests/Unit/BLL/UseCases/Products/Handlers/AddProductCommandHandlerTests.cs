using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.AddProduct;
using eCommerceOnlineShop.Catalog.Core.Interfaces;
using eCommerceOnlineShop.Catalog.Core.Models;
using Moq;
using Xunit;

namespace eCommerceOnlineShop.Catalog.Tests.Unit.BLL.UseCases.Products.Handlers
{
    public class AddProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly AddProductCommandHandler _handler;

        public AddProductCommandHandlerTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _handler = new AddProductCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesProduct()
        {
            // Arrange
            var command = new AddProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.99m,
                CategoryId = 1
            };

            var expectedProduct = new Product
            {
                Id = 1,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                CategoryId = command.CategoryId
            };

            _mockRepository.Setup(r => r.CreateProductAsync(It.IsAny<Product>()))
                .ReturnsAsync(expectedProduct);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProduct.Id, result.Id);
            Assert.Equal(expectedProduct.Name, result.Name);
            Assert.Equal(expectedProduct.Description, result.Description);
            Assert.Equal(expectedProduct.Price, result.Price);
            Assert.Equal(expectedProduct.CategoryId, result.CategoryId);

            _mockRepository.Verify(r => r.CreateProductAsync(It.IsAny<Product>()), Times.Once);
        }
    }
}