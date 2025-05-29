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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await messageListener.StartListeningAsync<ProductUpdateMessage>(
                settings.Value.TopicName,
                settings.Value.SubscriptionName,
                async message =>
                {
                    try
                    {
                        logger.LogInformation("Received product update: {ProductId}", message.ProductId);

                        var carts = await cartRepository.GetAllCartsAsync();
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

                                await cartRepository.UpdateCartAsync(cart);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Error processing product update for product {message.ProductId}");
                        throw;
                    }
                });
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await messageListener.StopListeningAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
