using AutoMapper;
using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using eCommerceOnlineShop.Catalog.Core.Models;
using eCommerceOnlineShop.MessageBroker.Configuration;
using Microsoft.Extensions.Options;
using MediatR;
using eCommerceOnlineShop.MessageBroker.Interfaces;
using eCommerceOnlineShop.MessageBroker.Messages;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.UpdateProduct
{
    public class UpdateProductCommandHandler(
        IProductRepository productRepository,
        IMapper mapper,
        IMessagePublisher messagePublisher,
        IOptions<ServiceBusSettings> settings) : IRequestHandler<UpdateProductCommand, Product>
    {
        public async Task<Product> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = mapper.Map<Product>(request);
            var updatedProduct = await productRepository.UpdateProductAsync(product);

            var message = new ProductUpdateMessage
            {
                ProductId = updatedProduct.Id,
                Name = updatedProduct.Name,
                Price = updatedProduct.Price,
                Amount = updatedProduct.Amount
            };

            await messagePublisher.PublishAsync(message, settings.Value.TopicName);

            return updatedProduct;
        }
    }
}