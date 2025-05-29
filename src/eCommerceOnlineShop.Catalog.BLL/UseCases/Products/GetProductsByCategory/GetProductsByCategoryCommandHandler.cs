using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProductsByCategory
{
    public class GetProductsByCategoryCommandHandler(IProductRepository productRepository) : IRequestHandler<GetProductsByCategoryCommand, IEnumerable<Product>>
    {
        public async Task<IEnumerable<Product>> Handle(GetProductsByCategoryCommand request, CancellationToken cancellationToken)
        {
            return await productRepository.GetProductsByCategoryAsync(request.CategoryId);
        }
    }
}