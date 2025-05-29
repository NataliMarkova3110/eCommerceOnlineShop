using System;
using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using eCommerceOnlineShop.MessageBroker.Configuration;
using eCommerceOnlineShop.MessageBroker.Interfaces;

namespace eCommerceOnlineShop.MessageBroker.Services
{
    internal class MessagePublisher : IMessagePublisher
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ILogger<MessagePublisher> _logger;
        private readonly ServiceBusSettings _settings;

        public MessagePublisher(
            IOptions<ServiceBusSettings> settings,
            ILogger<MessagePublisher> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            _serviceBusClient = new ServiceBusClient(_settings.ConnectionString);
        }

        public async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }

        public async Task CloseAsync()
        {
            await _serviceBusClient.DisposeAsync();
        }

        public async Task PublishAsync<T>(T message, string topicName) where T : class
        {
            var retryCount = 0;
            while (retryCount < _settings.MaxRetryAttempts)
            {
                try
                {
                    var sender = _serviceBusClient.CreateSender(topicName);
                    var jsonMessage = JsonSerializer.Serialize(message);
                    var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
                    {
                        MessageId = Guid.NewGuid().ToString(),
                        ContentType = "application/json",
                        TimeToLive = TimeSpan.FromDays(_settings.MessageTimeToLiveInDays),
                        ApplicationProperties =
                        {
                            ["RetryCount"] = 0
                        }
                    };

                    await sender.SendMessageAsync(serviceBusMessage);
                    _logger.LogInformation($"Message published to topic {topicName}: {jsonMessage}");
                    return;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount >= _settings.MaxRetryAttempts)
                    {
                        _logger.LogError(ex, $"Failed to publish message to topic {topicName} after {_settings.MaxRetryAttempts} attempts");
                        throw;
                    }
                    _logger.LogWarning(ex, $"Failed to publish message to topic {topicName}, attempt {retryCount}/{_settings.MaxRetryAttempts}");
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount))); // Exponential backoff
                }
            }
        }
    }
}
