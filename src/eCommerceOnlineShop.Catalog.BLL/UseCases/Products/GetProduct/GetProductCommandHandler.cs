using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProduct
{
    public class GetProductCommandHandler(IProductRepository productRepository) : IRequestHandler<GetProductCommand, Product?>
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<Product?> Handle(GetProductCommand request, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProductAsync(request.ProductId);
        }
    }
}