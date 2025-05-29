namespace eCommerceOnlineShop.Cart.Core.Models
{
    /// <summary>
    /// Represents a shopping cart entity
    /// </summary>
    public class CartEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the cart
        /// </summary>
        public Guid? Id { get; set; }
        public required string CartKey { get; set; }
        /// <summary>
        /// Gets or sets the list of items in the cart
        /// </summary>
        public List<CartItem> Items { get; set; } = [];
    }
}
