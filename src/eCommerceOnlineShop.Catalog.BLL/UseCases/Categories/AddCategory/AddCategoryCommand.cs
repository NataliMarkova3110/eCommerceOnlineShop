using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.AddCategory
{
    public class AddCategoryCommand : IRequest<Category>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
