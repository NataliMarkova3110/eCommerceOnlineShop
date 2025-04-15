using FluentValidation;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProduct
{
    public class GetProductCommandValidator : AbstractValidator<GetProductCommand>
    {
        public GetProductCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product ID must be greater than 0");
        }
    }
}