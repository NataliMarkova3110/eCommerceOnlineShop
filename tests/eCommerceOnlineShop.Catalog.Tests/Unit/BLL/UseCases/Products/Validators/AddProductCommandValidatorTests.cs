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
        public void Validate_ValidCommand_ShouldNotHaveValidationErrorAsync()
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

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_EmptyName_ShouldHaveValidationErrorAsync(string? name)
        {
            // Arrange
            var command = new AddProductCommand
            {
                Name = name!,
                Description = "Test Description",
                Price = 10.99m,
                CategoryId = 1,
                Amount = 100
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_InvalidPrice_ShouldHaveValidationErrorAsync(decimal price)
        {
            // Arrange
            var command = new AddProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = price,
                CategoryId = 1,
                Amount = 100
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Price);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_InvalidCategoryId_ShouldHaveValidationErrorAsync(int categoryId)
        {
            // Arrange
            var command = new AddProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.99m,
                CategoryId = categoryId,
                Amount = 100
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
        }
    }
}
