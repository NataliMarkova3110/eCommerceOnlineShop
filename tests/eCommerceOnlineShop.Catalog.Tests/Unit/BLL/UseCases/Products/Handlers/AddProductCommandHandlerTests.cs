using AutoMapper;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.AddProduct;
using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using eCommerceOnlineShop.Catalog.Core.Models;
using Moq;
using Xunit;

namespace eCommerceOnlineShop.Catalog.Tests.Unit.BLL.UseCases.Products.Handlers
{
    public class AddProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AddProductCommandHandler _handler;

        public AddProductCommandHandlerTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new AddProductCommandHandler(_mockProductRepository.Object, _mockCategoryRepository.Object, _mockMapper.Object);
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
                CategoryId = 1,
                Amount = 100
            };

            var expectedProduct = new Product
            {
                Id = 1,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                CategoryId = command.CategoryId,
                Amount = command.Amount
            };

            _mockProductRepository.Setup(r => r.CreateProductAsync(It.IsAny<Product>()))
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

            _mockProductRepository.Verify(r => r.CreateProductAsync(It.IsAny<Product>()), Times.Once);
        }
    }
}
