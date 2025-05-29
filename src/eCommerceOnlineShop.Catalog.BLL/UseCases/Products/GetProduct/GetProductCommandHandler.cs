using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProduct
{
    public class GetProductCommandHandler(IProductRepository productRepository) : IRequestHandler<GetProductCommand, Product?>
    {
        public async Task<Product?> Handle(GetProductCommand request, CancellationToken cancellationToken)
        {
            return await productRepository.GetProductAsync(request.ProductId);
        }
    }
}