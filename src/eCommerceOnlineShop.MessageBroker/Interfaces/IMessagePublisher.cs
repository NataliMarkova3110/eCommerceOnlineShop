
namespace eCommerceOnlineShop.MessageBroker.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string topicName) where T : class;
    }
}
