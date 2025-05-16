namespace eCommerceOnlineShop.MessageBroker.Configuration
{
    public class ServiceBusSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string SubscriptionName { get; set; } = string.Empty;
        public int MaxRetryAttempts { get; set; } = 3;
        public int RetryDelayInSeconds { get; set; } = 30;
        public int MessageTimeToLiveInDays { get; set; } = 7;
    }
}