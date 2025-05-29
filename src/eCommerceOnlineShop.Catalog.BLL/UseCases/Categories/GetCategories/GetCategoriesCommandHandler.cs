using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.GetCategories
{
    public class GetCategoriesCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetCategoriesCommand, IEnumerable<Category>>
    {
        public async Task<IEnumerable<Category>> Handle(GetCategoriesCommand request, CancellationToken cancellationToken)
        {
            return await categoryRepository.GetCategoriesAsync();
        }
    }
}
