using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.DeleteCategory
{
    public class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetCategoryAsync(request.CategoryId);
            if (category == null)
            {
                return false;
            }

            return await _categoryRepository.DeleteCategoryAsync(request.CategoryId);
        }
    }
}