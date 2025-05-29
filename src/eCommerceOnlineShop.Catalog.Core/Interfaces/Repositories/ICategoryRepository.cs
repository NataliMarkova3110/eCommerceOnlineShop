using eCommerceOnlineShop.Catalog.Core.Models;

namespace eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetCategoryAsync(int id);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
