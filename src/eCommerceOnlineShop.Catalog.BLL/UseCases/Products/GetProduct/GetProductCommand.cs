using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProduct
{
    public class GetProductCommand : IRequest<Product?>
    {
        public int ProductId { get; set; }
    }
}
