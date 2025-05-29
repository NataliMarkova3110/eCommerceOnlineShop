using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.DeleteCategory
{
    public class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<DeleteCategoryCommand, bool>
    {
        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await categoryRepository.GetCategoryAsync(request.CategoryId);
            return category != null && await categoryRepository.DeleteCategoryAsync(request.CategoryId);
        }
    }
}
