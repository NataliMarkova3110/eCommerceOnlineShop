using eCommerceOnlineShop.Cart.Core.Interfaces;
using eCommerceOnlineShop.Cart.Core.Models;

namespace eCommerceOnlineShop.Cart.BLL.Services
{
    public class CartService(ICartRepository cartRepository) : ICartService
    {
        private readonly ICartRepository _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(Guid cartId)
        {
            var cart = await _cartRepository.GetCartAsync(cartId);
            return cart?.Items ?? [];
        }

        public async Task<CartItem> AddItemToCartAsync(Guid cartId, CartItem item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var cart = await _cartRepository.GetCartAsync(cartId);

            if (cart == null)
            {
                cart = new CartEntity { Id = cartId };
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

        public async Task<bool> RemoveItemFromCartAsync(Guid cartId, int itemId)
        {
            var cart = await _cartRepository.GetCartAsync(cartId);
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