namespace eCommerceOnlineShop.MessageBroker.Interfaces
{
    public interface IMessageListener
    {
        Task StartListeningAsync<T>(string topicName, string subscriptionName, Func<T, Task> messageHandler) where T : class;
        Task StopListeningAsync();
    }
}