using eCommerceOnlineShop.Cart.Core.Models;

namespace eCommerceOnlineShop.Cart.Core.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<CartItem>> GetCartItemsAsync(Guid cartId);
        Task<CartItem> AddItemToCartAsync(Guid cartId, CartItem item);
        Task<bool> RemoveItemFromCartAsync(Guid cartId, int itemId);
    }
}