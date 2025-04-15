using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.AddCategory
{
    public class AddCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<AddCategoryCommand, Category>
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<Category> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                ParentCategoryId = request.ParentCategoryId
            };

            return await _categoryRepository.CreateCategoryAsync(category);
        }
    }
}