namespace eCommerceOnlineShop.Cart.Core.Exceptions
{
    public class ProductUpdateException(string message, Exception innerException, object errorContext) : Exception(message, innerException)
    {
        public object ErrorContext { get; } = errorContext;
    }
}