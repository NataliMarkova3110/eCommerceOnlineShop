using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.GetCategory
{
    public class GetCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetCategoryCommand, Category?>
    {
        public async Task<Category?> Handle(GetCategoryCommand request, CancellationToken cancellationToken)
        {
            return await categoryRepository.GetCategoryAsync(request.CategoryId);
        }
    }
}