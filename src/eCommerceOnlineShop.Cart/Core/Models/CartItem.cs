namespace eCommerceOnlineShop.Cart.Core.Models
{
    /// <summary>
    /// Represents an item in a shopping cart
    /// </summary>
    public class CartItem
    {
        /// <summary>
        /// Gets or sets the unique identifier of the product
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the product
        /// </summary>
        public required int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the name of the product
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the price of the product
        /// </summary>
        public required decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product in the cart
        /// </summary>
        public required int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the image of the product
        /// </summary>
        public CartItemImage? Image { get; set; }
    }
}
