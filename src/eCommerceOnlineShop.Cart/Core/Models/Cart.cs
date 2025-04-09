namespace eCommerceOnlineShop.Cart.Core.Models
{
    public class CartEntity
    {
        public required Guid Id { get; set; }  // Client-side generated unique ID
        public List<CartItem> Items { get; set; } = [];
    }
}