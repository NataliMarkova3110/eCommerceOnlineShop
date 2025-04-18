using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProducts
{
    public class GetProductsCommandHandler(IProductRepository productRepository) : IRequestHandler<GetProductsCommand, IEnumerable<Product>>
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<IEnumerable<Product>> Handle(GetProductsCommand request, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProductsAsync();
        }
    }
}