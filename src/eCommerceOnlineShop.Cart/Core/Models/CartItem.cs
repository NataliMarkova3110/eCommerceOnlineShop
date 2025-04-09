namespace eCommerceOnlineShop.Cart.Core.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public CartItemImage? Image { get; set; }
    }
}