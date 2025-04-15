using FluentValidation;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProductsByCategory
{
    public class GetProductsByCategoryCommandValidator : AbstractValidator<GetProductsByCategoryCommand>
    {
        public GetProductsByCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category ID must be greater than 0");
        }
    }
}