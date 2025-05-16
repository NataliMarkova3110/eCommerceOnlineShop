using eCommerceOnlineShop.Cart.Core.Interfaces;
using eCommerceOnlineShop.MessageBroker.Interfaces;
using eCommerceOnlineShop.MessageBroker.Messages;
using eCommerceOnlineShop.MessageBroker.Configuration;
using Microsoft.Extensions.Options;

namespace eCommerceOnlineShop.Cart.Handlers
{
    public class ProductUpdateHandler(
        IMessageListener messageListener,
        ICartRepository cartRepository,
        ILogger<ProductUpdateHandler> logger,
        IOptions<ServiceBusSettings> settings) : BackgroundService
    {
        private readonly IMessageListener _messageListener = messageListener;
        private readonly ICartRepository _cartRepository = cartRepository;
        private readonly ILogger<ProductUpdateHandler> _logger = logger;
        private readonly ServiceBusSettings _settings = settings.Value;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _messageListener.StartListeningAsync<ProductUpdateMessage>(
                _settings.TopicName,
                _settings.SubscriptionName,
                async message =>
                {
                    try
                    {
                        _logger.LogInformation($"Received product update: {message.ProductId}");

                        var carts = await _cartRepository.GetAllCartsAsync();
                        foreach (var cart in carts)
                        {
                            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == message.ProductId);
                            if (cartItem != null)
                            {
                                cartItem.ProductName = message.Name;
                                cartItem.Price = message.Price;

                                if (message.Amount == 0)
                                {
                                    cart.Items.Remove(cartItem);
                                }

                                await _cartRepository.UpdateCartAsync(cart);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing product update for product {message.ProductId}");
                        throw;
                    }
                });
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _messageListener.StopListeningAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}