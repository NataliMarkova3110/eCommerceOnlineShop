using eCommerceOnlineShop.Catalog.Core.Models;

namespace eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetProductAsync(int id);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
    }
}