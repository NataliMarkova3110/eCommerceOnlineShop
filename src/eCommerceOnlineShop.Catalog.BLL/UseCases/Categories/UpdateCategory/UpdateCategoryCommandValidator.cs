using FluentValidation;
using eCommerceOnlineShop.Catalog.Core.Interfaces.Repositories;

namespace eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.UpdateCategory
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryCommandValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Category ID must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.ParentCategoryId)
                .MustAsync(async (command, parentId, cancellation) =>
                {
                    if (!parentId.HasValue) return true;
                    if (parentId.Value == command.Id) return false;
                    var parentCategory = await _categoryRepository.GetCategoryAsync(parentId.Value);
                    return parentCategory != null;
                })
                .WithMessage("Invalid parent category");
        }
    }
}