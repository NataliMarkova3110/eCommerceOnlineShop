using eCommerceOnlineShop.Cart.Core.Models;

namespace eCommerceOnlineShop.Cart.Core.Interfaces
{
    public interface ICartRepository
    {
        Task<CartEntity?> GetCartAsync(string cartKey);
        Task<IEnumerable<CartEntity>> GetAllCartsAsync();
        Task CreateCartAsync(CartEntity cart);
        Task UpdateCartAsync(CartEntity cart);
        Task<bool> DeleteCartAsync(string cartKey);
    }
}