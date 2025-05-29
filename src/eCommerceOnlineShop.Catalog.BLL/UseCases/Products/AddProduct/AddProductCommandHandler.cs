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
        public async Task<Product> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            _ = await categoryRepository.GetCategoryAsync(request.CategoryId)
                ?? throw new ArgumentException("Category does not exist", nameof(request));

            var product = mapper.Map<Product>(request);
            return await productRepository.CreateProductAsync(product);
        }
    }
}
