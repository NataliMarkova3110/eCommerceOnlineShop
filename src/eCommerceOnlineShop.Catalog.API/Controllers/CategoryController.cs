using eCommerceOnlineShop.Catalog.API.Models;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.AddCategory;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.DeleteCategory;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.GetCategories;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.GetCategory;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.UpdateCategory;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceOnlineShop.Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController(IMediator mediator, LinkGenerator linkGenerator) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ResourceResponse<IEnumerable<Category>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResourceResponse<IEnumerable<Category>>>> GetCategoriesAsync()
        {
            var categories = await mediator.Send(new GetCategoriesCommand());

            var response = new ResourceResponse<IEnumerable<Category>>
            {
                Data = categories,
                Links =
                {
                    new() { Href = linkGenerator.GetPathByAction(nameof(GetCategoriesAsync), "Category") ?? "/api/Category", Rel = "self", Method = "GET" },
                    new() { Href = linkGenerator.GetPathByAction(nameof(AddCategoryAsync), "Category") ?? "/api/Category", Rel = "create-category", Method = "POST" }
                }
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResourceResponse<Category>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponse<Category>>> GetCategoryAsync(int id)
        {
            var category = await mediator.Send(new GetCategoryCommand { CategoryId = id });
            if (category == null)
            {
                return NotFound();
            }

            var response = new ResourceResponse<Category>
            {
                Data = category,
                Links =
                {
                    new() { Href = linkGenerator.GetPathByAction(nameof(GetCategoryAsync), "Category", new { id }) ?? $"/api/Category/{id}", Rel = "self", Method = "GET" },
                    new() { Href = linkGenerator.GetPathByAction(nameof(UpdateCategoryAsync), "Category", new { id }) ?? $"/api/Category/{id}", Rel = "update-category", Method = "PUT" },
                    new() { Href = linkGenerator.GetPathByAction(nameof(DeleteCategoryAsync), "Category", new { id }) ?? $"/api/Category/{id}", Rel = "delete-category", Method = "DELETE" }
                }
            };

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResourceResponse<Category>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<ResourceResponse<Category>>> AddCategoryAsync(AddCategoryCommand command)
        {
            var category = await mediator.Send(command);

            var response = new ResourceResponse<Category>
            {
                Data = category,
                Links =
                {
                    new() { Href = linkGenerator.GetPathByAction(nameof(GetCategoryAsync), "Category", new { id = category.Id }) ?? $"/api/Category/{category.Id}", Rel = "self", Method = "GET" },
                    new() { Href = linkGenerator.GetPathByAction(nameof(UpdateCategoryAsync), "Category", new { id = category.Id }) ?? $"/api/Category/{category.Id}", Rel = "update-category", Method = "PUT" },
                    new() { Href = linkGenerator.GetPathByAction(nameof(DeleteCategoryAsync), "Category", new { id = category.Id }) ?? $"/api/Category/{category.Id}", Rel = "delete-category", Method = "DELETE" }
                }
            };

            return CreatedAtAction(nameof(GetCategoryAsync), new { id = category.Id }, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResourceResponse<Category>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<ResourceResponse<Category>>> UpdateCategoryAsync(int id, UpdateCategoryCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Category ID mismatch");
            }

            var category = await mediator.Send(command);
            if (category == null)
            {
                return NotFound();
            }

            var response = new ResourceResponse<Category>
            {
                Data = category,
                Links =
                {
                    new() { Href = linkGenerator.GetPathByAction(nameof(GetCategoryAsync), "Category", new { id }) ?? $"/api/Category/{id}", Rel = "self", Method = "GET" },
                    new() { Href = linkGenerator.GetPathByAction(nameof(UpdateCategoryAsync), "Category", new { id }) ?? $"/api/Category/{id}", Rel = "update-category", Method = "PUT" },
                    new() { Href = linkGenerator.GetPathByAction(nameof(DeleteCategoryAsync), "Category", new { id }) ?? $"/api/Category/{id}", Rel = "delete-category", Method = "DELETE" }
                }
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            var result = await mediator.Send(new DeleteCategoryCommand { CategoryId = id });
            return !result ? NotFound() : NoContent();
        }
    }
}