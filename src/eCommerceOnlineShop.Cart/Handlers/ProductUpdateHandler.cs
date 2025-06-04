using eCommerceOnlineShop.Cart.Core.Interfaces;
using eCommerceOnlineShop.MessageBroker.Interfaces;
using eCommerceOnlineShop.MessageBroker.Messages;
using eCommerceOnlineShop.MessageBroker.Configuration;
using Microsoft.Extensions.Options;
using eCommerceOnlineShop.Cart.Core.Exceptions;

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
                        logger.LogInformation("Received product update for product {ProductId}", message.ProductId);

                        var carts = await cartRepository.GetAllCartsAsync();
                        var updatedCarts = 0;
                        var removedItems = 0;

                        foreach (var cart in carts)
                        {
                            try
                            {
                                var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == message.ProductId);
                                if (cartItem != null)
                                {
                                    cartItem.ProductName = message.Name;
                                    cartItem.Price = message.Price;

                                    if (message.Amount == 0)
                                    {
                                        cart.Items.Remove(cartItem);
                                        removedItems++;
                                        logger.LogInformation(
                                            "Removed out-of-stock product {ProductId} from cart {CartKey}",
                                            message.ProductId,
                                            cart.CartKey);
                                    }

                                    await cartRepository.UpdateCartAsync(cart);
                                    updatedCarts++;
                                }
                            }
                            catch (Exception cartEx)
                            {
                                logger.LogError(
                                    cartEx,
                                    "Failed to update cart {CartKey} for product {ProductId}. Cart will be skipped.",
                                    cart.CartKey,
                                    message.ProductId);
                            }
                        }

                        logger.LogInformation(
                            "Product update completed for product {ProductId}. Updated {UpdatedCarts} carts, removed {RemovedItems} items.",
                            message.ProductId,
                            updatedCarts,
                            removedItems);
                    }
                    catch (Exception ex)
                    {
                        var errorContext = new
                        {
                            ProductId = message.ProductId,
                            ProductName = message.Name,
                            Price = message.Price,
                            Amount = message.Amount
                        };

                        logger.LogError(
                            ex,
                            "Failed to process product update. Context: {@ErrorContext}",
                            errorContext);

                        throw new ProductUpdateException(
                            $"Failed to process product update for product {message.ProductId}",
                            ex,
                            errorContext);
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
