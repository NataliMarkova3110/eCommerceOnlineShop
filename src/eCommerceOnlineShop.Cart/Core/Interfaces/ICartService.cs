using eCommerceOnlineShop.Cart.Core.Models;

namespace eCommerceOnlineShop.Cart.Core.Interfaces
{
    public interface ICartService
    {
        Task<CartEntity?> GetCartAsync(string cartKey);
        Task<IEnumerable<CartItem>> GetCartItemsAsync(string cartKey);
        Task<CartItem> AddItemToCartAsync(string cartKey, CartItem item);
        Task<bool> RemoveItemFromCartAsync(string cartKey, int itemId);
    }
}
