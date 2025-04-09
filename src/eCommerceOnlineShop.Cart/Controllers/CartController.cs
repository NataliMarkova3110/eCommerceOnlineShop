using Microsoft.AspNetCore.Mvc;
using eCommerceOnlineShop.Cart.Core.Interfaces;
using eCommerceOnlineShop.Cart.Core.Models;

namespace eCommerceOnlineShop.Cart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController(ICartService cartService) : ControllerBase
    {
        private readonly ICartService _cartService = cartService;

        [HttpGet("{cartId}/items")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems(Guid cartId)
        {
            var items = await _cartService.GetCartItemsAsync(cartId);
            return Ok(items);
        }

        [HttpPost("{cartId}/items")]
        public async Task<ActionResult<CartItem>> AddItemToCart(Guid cartId, [FromBody] CartItem item)
        {
            var addedItem = await _cartService.AddItemToCartAsync(cartId, item);
            return Ok(addedItem);
        }

        [HttpDelete("{cartId}/items/{itemId}")]
        public async Task<ActionResult> RemoveItemFromCart(Guid cartId, int itemId)
        {
            var result = await _cartService.RemoveItemFromCartAsync(cartId, itemId);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}