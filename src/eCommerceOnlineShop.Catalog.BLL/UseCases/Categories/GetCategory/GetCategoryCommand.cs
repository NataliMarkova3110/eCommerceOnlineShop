using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.GetCategory
{
    public class GetCategoryCommand : IRequest<Category?>
    {
        public int CategoryId { get; set; }
    }
}