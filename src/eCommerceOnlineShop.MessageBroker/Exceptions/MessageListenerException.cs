namespace eCommerceOnlineShop.MessageBroker.Exceptions
{
    public class MessageListenerException(string message, Exception innerException, object errorContext) : Exception(message, innerException)
    {
        public object ErrorContext { get; } = errorContext;
    }
}
