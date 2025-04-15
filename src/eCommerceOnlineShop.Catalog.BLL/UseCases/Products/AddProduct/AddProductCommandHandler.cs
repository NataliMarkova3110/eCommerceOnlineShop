using AutoMapper;
using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.AddProduct
{
    public class AddProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper) : IRequestHandler<AddProductCommand, Product>
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<Product> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            _ = await _categoryRepository.GetCategoryAsync(request.CategoryId)
                ?? throw new ArgumentException("Category does not exist", nameof(request));

            var product = _mapper.Map<Product>(request);
            return await _productRepository.CreateProductAsync(product);
        }
    }
}