using AutoMapper;
using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.AddCategory
{
    public class AddCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper) : IRequestHandler<AddCategoryCommand, Category>
    {
        public async Task<Category> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = mapper.Map<Category>(request);
            return await categoryRepository.CreateCategoryAsync(category);
        }
    }
}
