using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.AddCategory
{
    public class AddCategoryCommand : IRequest<Category>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}