using Microsoft.AspNetCore.Mvc;
using eCommerceOnlineShop.Cart.Core.Interfaces;
using eCommerceOnlineShop.Cart.Core.Models;
using Asp.Versioning;

namespace eCommerceOnlineShop.Cart.Controllers
{
    /// <summary>
    /// Controller for managing shopping cart operations
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class CartController(ICartService cartService) : ControllerBase
    {
        private readonly ICartService _cartService = cartService;

        /// <summary>
        /// Retrieves a cart by its key (Version 1.0)
        /// </summary>
        /// <param name="cartKey">The unique identifier of the cart</param>
        /// <returns>The cart entity with all its items</returns>
        /// <response code="200">Returns the requested cart</response>
        /// <response code="404">If the cart is not found</response>
        [HttpGet("{cartKey}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(CartEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CartEntity>> GetCartV1(string cartKey)
        {
            var cart = await _cartService.GetCartAsync(cartKey);
            if (cart == null)
            {
                return NotFound();
            }

            return Ok(cart);
        }

        /// <summary>
        /// Retrieves cart items by cart key (Version 2.0)
        /// </summary>
        /// <param name="cartKey">The unique identifier of the cart</param>
        /// <returns>A list of cart items</returns>
        /// <response code="200">Returns the list of cart items</response>
        /// <response code="404">If the cart is not found</response>
        [HttpGet("{cartKey}")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(IEnumerable<CartItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartV2(string cartKey)
        {
            var items = await _cartService.GetCartItemsAsync(cartKey);
            if (items == null)
            {
                return NotFound();
            }

            return Ok(items);
        }

        /// <summary>
        /// Adds an item to the cart
        /// </summary>
        /// <param name="cartKey">The unique identifier of the cart</param>
        /// <param name="item">The cart item to add</param>
        /// <returns>The added cart item</returns>
        /// <response code="200">Returns the added cart item</response>
        /// <response code="400">If the item or cart key is invalid</response>
        [HttpPost("{cartKey}/items")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(CartItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CartItem>> AddItemToCart(string cartKey, [FromBody] CartItem item)
        {
            if (item == null)
            {
                return BadRequest("Cart item cannot be null");
            }

            if (string.IsNullOrWhiteSpace(cartKey))
            {
                return BadRequest("Cart key cannot be empty");
            }

            var addedItem = await _cartService.AddItemToCartAsync(cartKey, item);
            return Ok(addedItem);
        }

        /// <summary>
        /// Removes an item from the cart
        /// </summary>
        /// <param name="cartKey">The unique identifier of the cart</param>
        /// <param name="itemId">The ID of the item to remove</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the item was successfully removed</response>
        /// <response code="404">If the cart or item is not found</response>
        [HttpDelete("{cartKey}/items/{itemId}")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveItemFromCart(string cartKey, int itemId)
        {
            var result = await _cartService.RemoveItemFromCartAsync(cartKey, itemId);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}