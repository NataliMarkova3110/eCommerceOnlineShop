using eCommerceOnlineShop.MessageBroker.Configuration;
using eCommerceOnlineShop.MessageBroker.Services;
using eCommerceOnlineShop.MessageBroker.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace eCommerceOnlineShop.MessageBroker.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageBroker(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ServiceBusSettings>(configuration.GetSection("AzureServiceBus"));

            services.AddSingleton<IMessagePublisher, MessagePublisher>();
            services.AddSingleton<IMessageListener, MessageListener>();

            return services;
        }
    }
}