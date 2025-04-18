using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.AddProduct;
using FluentValidation.TestHelper;
using Xunit;

namespace eCommerceOnlineShop.Catalog.Tests.Unit.BLL.UseCases.Products.Validators
{
    public class AddProductCommandValidatorTests
    {
        private readonly AddProductCommandValidator _validator;

        public AddProductCommandValidatorTests()
        {
            _validator = new AddProductCommandValidator();
        }

        [Fact]
        public void Validate_ValidCommand_ShouldNotHaveValidationError()
        {
            // Arrange
            var command = new AddProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.99m,
                CategoryId = 1
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_EmptyName_ShouldHaveValidationError(string name)
        {
            // Arrange
            var command = new AddProductCommand
            {
                Name = name,
                Description = "Test Description",
                Price = 10.99m,
                CategoryId = 1
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_InvalidPrice_ShouldHaveValidationError(decimal price)
        {
            // Arrange
            var command = new AddProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = price,
                CategoryId = 1
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Price);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_InvalidCategoryId_ShouldHaveValidationError(int categoryId)
        {
            // Arrange
            var command = new AddProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.99m,
                CategoryId = categoryId
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
        }
    }
}