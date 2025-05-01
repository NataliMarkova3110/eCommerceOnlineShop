using eCommerceOnlineShop.Cart.Core.Interfaces;
using eCommerceOnlineShop.Cart.Core.Models;

namespace eCommerceOnlineShop.Cart.BLL.Services
{
    public class CartService(ICartRepository cartRepository) : ICartService
    {
        private readonly ICartRepository _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));

        public async Task<CartEntity?> GetCartAsync(string cartKey)
        {
            return await _cartRepository.GetCartAsync(cartKey);
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string cartKey)
        {
            var cart = await _cartRepository.GetCartAsync(cartKey);
            return cart?.Items ?? [];
        }

        public async Task<CartItem> AddItemToCartAsync(string cartKey, CartItem item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var cart = await _cartRepository.GetCartAsync(cartKey);

            if (cart == null)
            {
                cart = new CartEntity
                {
                    Id = Guid.NewGuid(),
                    CartKey = cartKey,
                    Items = []
                };
                cart.Items.Add(item);
                await _cartRepository.CreateCartAsync(cart);
            }
            else
            {
                var existingItem = cart.Items.Find(i => i.Id == item.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity += item.Quantity;
                }
                else
                {
                    cart.Items.Add(item);
                }
                await _cartRepository.UpdateCartAsync(cart);
            }

            return item;
        }

        public async Task<bool> RemoveItemFromCartAsync(string cartKey, int itemId)
        {
            var cart = await _cartRepository.GetCartAsync(cartKey);
            if (cart == null)
                return false;

            var itemToRemove = cart.Items.Find(i => i.Id == itemId);
            if (itemToRemove == null)
                return false;

            cart.Items.Remove(itemToRemove);
            await _cartRepository.UpdateCartAsync(cart);
            return true;
        }
    }
}