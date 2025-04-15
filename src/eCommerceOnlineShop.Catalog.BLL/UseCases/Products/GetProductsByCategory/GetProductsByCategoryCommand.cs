using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProductsByCategory
{
    public class GetProductsByCategoryCommand : IRequest<IEnumerable<Product>>
    {
        public int CategoryId { get; set; }
    }
}