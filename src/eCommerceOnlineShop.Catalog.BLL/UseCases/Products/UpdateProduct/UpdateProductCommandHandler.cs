using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Product>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UpdateProductCommandHandler(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<Product> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetCategoryAsync(request.CategoryId)
                ?? throw new ArgumentException("Category does not exist", nameof(request));

            var existingProduct = await _productRepository.GetProductAsync(request.Id)
                ?? throw new ArgumentException("Product does not exist", nameof(request));

            existingProduct.Name = request.Name;
            existingProduct.Description = request.Description;
            existingProduct.Price = request.Price;
            existingProduct.CategoryId = request.CategoryId;
            existingProduct.Amount = request.Amount;

            return await _productRepository.UpdateProductAsync(existingProduct);
        }
    }
}