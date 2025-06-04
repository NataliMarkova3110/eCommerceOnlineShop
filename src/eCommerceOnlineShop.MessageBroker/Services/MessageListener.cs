using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using eCommerceOnlineShop.MessageBroker.Configuration;
using eCommerceOnlineShop.MessageBroker.Interfaces;
using eCommerceOnlineShop.MessageBroker.Exceptions;

namespace eCommerceOnlineShop.MessageBroker.Services
{
    internal class MessageListener : IMessageListener
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ILogger<MessageListener> _logger;
        private readonly ServiceBusSettings _settings;
        private ServiceBusProcessor? _processor;

        public MessageListener(
            IOptions<ServiceBusSettings> settings,
            ILogger<MessageListener> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            _serviceBusClient = new ServiceBusClient(_settings.ConnectionString);
        }

        public static async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }

        public async Task CloseAsync()
        {
            if (_processor != null)
            {
                await _processor.DisposeAsync();
            }
            await _serviceBusClient.DisposeAsync();
        }

        public async Task StartListeningAsync<T>(string topicName, string subscriptionName, Func<T, Task> messageHandler) where T : class
        {
            try
            {
                _processor = CreateProcessor(topicName, subscriptionName);
                ConfigureMessageProcessing<T>(messageHandler);
                await _processor.StartProcessingAsync();
                _logger.LogInformation("Started listening to topic {TopicName} with subscription {SubscriptionName}",
                    topicName, subscriptionName);
            }
            catch (Exception ex)
            {
                var errorContext = new
                {
                    TopicName = topicName,
                    SubscriptionName = subscriptionName,
                    MessageType = typeof(T).Name
                };

                _logger.LogError(ex, "Failed to start listener. Context: {@ErrorContext}", errorContext);
                throw new MessageListenerException(
                    $"Failed to start listener for topic {topicName} and subscription {subscriptionName}",
                    ex,
                    errorContext);
            }
        }

        private ServiceBusProcessor CreateProcessor(string topicName, string subscriptionName)
        {
            var options = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false,
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5),
                PrefetchCount = 1
            };

            return _serviceBusClient.CreateProcessor(topicName, subscriptionName, options);
        }

        private void ConfigureMessageProcessing<T>(Func<T, Task> messageHandler) where T : class
        {
            if (_processor == null)
            {
                throw new InvalidOperationException("Processor is not initialized");
            }

            _processor.ProcessMessageAsync += async args => await ProcessMessageAsync(args, messageHandler);
            _processor.ProcessErrorAsync += args =>
            {
                _logger.LogError(args.Exception, "Error processing message");
                return Task.CompletedTask;
            };
        }

        private async Task ProcessMessageAsync<T>(ProcessMessageEventArgs args, Func<T, Task> messageHandler) where T : class
        {
            try
            {
                var message = DeserializeMessage<T>(args.Message);
                if (message == null)
                {
                    _logger.LogWarning("Failed to deserialize message {MessageId}", args.Message.MessageId);
                    await args.DeadLetterMessageAsync(args.Message, "Deserialization failed", "Message could not be deserialized to the expected type");
                    return;
                }

                var retryCount = GetRetryCount(args.Message);
                await HandleMessageProcessingAsync(args, message, messageHandler, retryCount, args.Message.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message {MessageId}. Message will be abandoned", args.Message.MessageId);
                await args.AbandonMessageAsync(args.Message);
            }
        }

        private static T? DeserializeMessage<T>(ServiceBusReceivedMessage message) where T : class
        {
            var messageBody = Encoding.UTF8.GetString(message.Body);
            return JsonSerializer.Deserialize<T>(messageBody);
        }

        private static int GetRetryCount(ServiceBusReceivedMessage message)
        {
            return message.ApplicationProperties.ContainsKey("RetryCount")
                ? (int)message.ApplicationProperties["RetryCount"]
                : 0;
        }

        private async Task HandleMessageProcessingAsync<T>(
            ProcessMessageEventArgs args,
            T message,
            Func<T, Task> messageHandler,
            int retryCount,
            string messageId) where T : class
        {
            try
            {
                await messageHandler(message);
                await args.CompleteMessageAsync(args.Message);
                _logger.LogInformation("Successfully processed message {MessageId}", messageId);
            }
            catch (Exception ex)
            {
                await HandleMessageErrorAsync(args, ex, retryCount, messageId);
            }
        }

        private async Task HandleMessageErrorAsync(
            ProcessMessageEventArgs args,
            Exception ex,
            int retryCount,
            string messageId)
        {
            if (retryCount < _settings.MaxRetryAttempts)
            {
                await ScheduleRetryAsync(args, retryCount, messageId);
                await args.CompleteMessageAsync(args.Message);
            }
            else
            {
                await MoveToDeadLetterAsync(args, ex, messageId);
            }
        }

        private async Task ScheduleRetryAsync(ProcessMessageEventArgs args, int retryCount, string messageId)
        {
            var retryMessage = new ServiceBusMessage(args.Message.Body)
            {
                MessageId = messageId,
                ApplicationProperties =
                {
                    ["RetryCount"] = retryCount + 1
                }
            };

            await _serviceBusClient.CreateSender(args.FullyQualifiedNamespace)
                .ScheduleMessageAsync(retryMessage, DateTimeOffset.UtcNow.AddSeconds(_settings.RetryDelayInSeconds));

            _logger.LogWarning("Message {MessageId} failed, scheduled for retry {RetryCount}/{MaxRetries}",
                messageId, retryCount + 1, _settings.MaxRetryAttempts);
        }

        private async Task MoveToDeadLetterAsync(ProcessMessageEventArgs args, Exception ex, string messageId)
        {
            await args.DeadLetterMessageAsync(args.Message,
                "Max retry attempts exceeded",
                $"Failed after {_settings.MaxRetryAttempts} attempts. Last error: {ex.Message}");
            _logger.LogError(ex, "Message {MessageId} moved to dead-letter queue after {MaxRetries} failed attempts",
                messageId, _settings.MaxRetryAttempts);
        }

        public async Task StopListeningAsync()
        {
            if (_processor != null)
            {
                await _processor.StopProcessingAsync();
                _logger.LogInformation("Stopped listening to messages");
            }
        }
    }
}
