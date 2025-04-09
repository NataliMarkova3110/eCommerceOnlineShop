using eCommerceOnlineShop.Cart.Core.Models;

namespace eCommerceOnlineShop.Cart.Core.Interfaces
{
    public interface ICartRepository
    {
        Task<CartEntity> GetCartAsync(Guid cartId);
        Task<CartEntity> CreateCartAsync(CartEntity cart);
        Task<CartEntity> UpdateCartAsync(CartEntity cart);
        Task<bool> DeleteCartAsync(Guid cartId);
    }
}