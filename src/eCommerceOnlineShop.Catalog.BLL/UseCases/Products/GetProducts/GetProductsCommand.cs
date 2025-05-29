using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProducts
{
    public class GetProductsCommand : IRequest<IEnumerable<Product>>
    {
    }
}
