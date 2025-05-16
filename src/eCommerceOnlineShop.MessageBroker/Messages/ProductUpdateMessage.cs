namespace eCommerceOnlineShop.MessageBroker.Messages
{
    public class ProductUpdateMessage
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
    }
}