using FluentValidation;
using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.AddCategory
{
    public class AddCategoryCommandValidator : AbstractValidator<AddCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;

        public AddCategoryCommandValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.ParentCategoryId)
                .MustAsync(async (parentId, cancellation) =>
                {
                    if (!parentId.HasValue) return true;
                    var parentCategory = await _categoryRepository.GetCategoryAsync(parentId.Value);
                    return parentCategory != null;
                })
                .WithMessage("Parent category does not exist");
        }
    }
}