using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using eCommerceOnlineShop.MessageBroker.Configuration;
using eCommerceOnlineShop.MessageBroker.Interfaces;

namespace eCommerceOnlineShop.MessageBroker.Services
{
    internal class MessageListener : IMessageListener
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ILogger<MessageListener> _logger;
        private readonly ServiceBusSettings _settings;
        private ServiceBusProcessor _processor;

        public MessageListener(
            IOptions<ServiceBusSettings> settings,
            ILogger<MessageListener> logger)
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
                var options = new ServiceBusProcessorOptions
                {
                    MaxConcurrentCalls = 1,
                    AutoCompleteMessages = false,
                    MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5),
                    PrefetchCount = 1
                };

                _processor = _serviceBusClient.CreateProcessor(topicName, subscriptionName, options);

                _processor.ProcessMessageAsync += async args =>
                {
                    try
                    {
                        var messageBody = Encoding.UTF8.GetString(args.Message.Body);
                        var message = JsonSerializer.Deserialize<T>(messageBody);

                        if (message != null)
                        {
                            var retryCount = args.Message.ApplicationProperties.ContainsKey("RetryCount")
                                ? (int)args.Message.ApplicationProperties["RetryCount"]
                                : 0;

                            try
                            {
                                await messageHandler(message);
                                await args.CompleteMessageAsync(args.Message);
                                _logger.LogInformation($"Successfully processed message {args.Message.MessageId}");
                            }
                            catch (Exception ex)
                            {
                                if (retryCount < _settings.MaxRetryAttempts)
                                {
                                    var retryMessage = new ServiceBusMessage(args.Message.Body)
                                    {
                                        MessageId = args.Message.MessageId,
                                        ApplicationProperties =
                                        {
                                            ["RetryCount"] = retryCount + 1
                                        }
                                    };

                                    await _serviceBusClient.CreateSender(topicName)
                                        .ScheduleMessageAsync(retryMessage, DateTimeOffset.UtcNow.AddSeconds(_settings.RetryDelayInSeconds));

                                    _logger.LogWarning(ex, $"Message {args.Message.MessageId} failed, scheduled for retry {retryCount + 1}/{_settings.MaxRetryAttempts}");
                                    await args.CompleteMessageAsync(args.Message);
                                }
                                else
                                {
                                    await args.DeadLetterMessageAsync(args.Message,
                                        "Max retry attempts exceeded",
                                        $"Failed after {_settings.MaxRetryAttempts} attempts. Last error: {ex.Message}");
                                    _logger.LogError(ex, $"Message {args.Message.MessageId} moved to dead-letter queue after {_settings.MaxRetryAttempts} failed attempts");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message");
                        await args.AbandonMessageAsync(args.Message);
                    }
                };

                _processor.ProcessErrorAsync += args =>
                {
                    _logger.LogError(args.Exception, "Error processing message");
                    return Task.CompletedTask;
                };

                await _processor.StartProcessingAsync();
                _logger.LogInformation($"Started listening to topic {topicName} with subscription {subscriptionName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error starting listener for topic {topicName}");
                throw;
            }
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