using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.AddProduct
{
    public class AddProductCommand : IRequest<Product>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public required int CategoryId { get; set; }
        public required int Amount { get; set; }
    }
}