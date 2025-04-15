using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.GetCategories
{
    public class GetCategoriesCommand : IRequest<IEnumerable<Category>>
    {
    }
}