using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Products.DeleteProduct
{
    public class DeleteProductCommand : IRequest<bool>
    {
        public int ProductId { get; set; }
    }
}
