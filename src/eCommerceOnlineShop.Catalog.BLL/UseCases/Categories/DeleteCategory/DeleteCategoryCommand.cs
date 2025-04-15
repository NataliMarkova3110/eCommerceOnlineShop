using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.DeleteCategory
{
    public class DeleteCategoryCommand : IRequest<bool>
    {
        public int CategoryId { get; set; }
    }
}