using FluentValidation;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.GetCategory
{
    public class GetCategoryCommandValidator : AbstractValidator<GetCategoryCommand>
    {
        public GetCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category ID must be greater than 0");
        }
    }
}
