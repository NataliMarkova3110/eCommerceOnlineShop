using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductAsync(request.ProductId);
            if (product == null)
            {
                return false;
            }

            return await _productRepository.DeleteProductAsync(request.ProductId);
        }
    }
}