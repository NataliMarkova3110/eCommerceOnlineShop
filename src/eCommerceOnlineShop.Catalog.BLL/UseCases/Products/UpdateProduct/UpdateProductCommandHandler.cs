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
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IMessagePublisher _messagePublisher = messagePublisher;
        private readonly ServiceBusSettings _settings = settings.Value;

        public async Task<Product> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(request);
            var updatedProduct = await _productRepository.UpdateProductAsync(product);

            var message = new ProductUpdateMessage
            {
                ProductId = updatedProduct.Id,
                Name = updatedProduct.Name,
                Price = updatedProduct.Price,
                Amount = updatedProduct.Amount
            };

            await _messagePublisher.PublishAsync(message, _settings.TopicName);

            return updatedProduct;
        }
    }
}