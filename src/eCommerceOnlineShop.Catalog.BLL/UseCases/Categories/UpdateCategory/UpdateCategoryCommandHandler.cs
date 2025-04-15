using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.UpdateCategory
{
    public class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<UpdateCategoryCommand, Category>
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<Category> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var existingCategory = await _categoryRepository.GetCategoryAsync(request.Id)
                ?? throw new ArgumentException("Category does not exist", nameof(request));

            existingCategory.Name = request.Name;
            existingCategory.Description = request.Description;
            existingCategory.ParentCategoryId = request.ParentCategoryId;

            return await _categoryRepository.UpdateCategoryAsync(existingCategory);
        }
    }
}