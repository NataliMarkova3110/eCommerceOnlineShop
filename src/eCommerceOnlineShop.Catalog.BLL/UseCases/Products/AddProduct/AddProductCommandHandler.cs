using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.AddProduct
{
    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Product>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public AddProductCommandHandler(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<Product> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetCategoryAsync(request.CategoryId)
                ?? throw new ArgumentException("Category does not exist", nameof(request));

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CategoryId = request.CategoryId,
                Amount = request.Amount
            };

            return await _productRepository.CreateProductAsync(product);
        }
    }
}