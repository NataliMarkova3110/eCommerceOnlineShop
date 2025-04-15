using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProductsByCategory
{
    public class GetProductsByCategoryCommandHandler : IRequestHandler<GetProductsByCategoryCommand, IEnumerable<Product>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsByCategoryCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> Handle(GetProductsByCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProductsByCategoryAsync(request.CategoryId);
        }
    }
}