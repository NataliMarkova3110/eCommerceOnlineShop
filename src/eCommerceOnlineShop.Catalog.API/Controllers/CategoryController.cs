using eCommerceOnlineShop.Catalog.API.Models;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.AddCategory;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.DeleteCategory;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.GetCategories;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.GetCategory;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Categories.UpdateCategory;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceOnlineShop.Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController(IMediator mediator, LinkGenerator linkGenerator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly LinkGenerator _linkGenerator = linkGenerator;

        [HttpGet]
        [ProducesResponseType(typeof(ResourceResponse<IEnumerable<Category>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResourceResponse<IEnumerable<Category>>>> GetCategories()
        {
            var categories = await _mediator.Send(new GetCategoriesCommand());

            var response = new ResourceResponse<IEnumerable<Category>>
            {
                Data = categories,
                Links =
                {
                    new() { Href = _linkGenerator.GetPathByAction(nameof(GetCategories), "Category"), Rel = "self", Method = "GET" },
                    new() { Href = _linkGenerator.GetPathByAction(nameof(AddCategory), "Category"), Rel = "create-category", Method = "POST" }
                }
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResourceResponse<Category>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponse<Category>>> GetCategory(int id)
        {
            var category = await _mediator.Send(new GetCategoryCommand { CategoryId = id });
            if (category == null)
            {
                return NotFound();
            }

            var response = new ResourceResponse<Category>
            {
                Data = category,
                Links =
                {
                    new() { Href = _linkGenerator.GetPathByAction(nameof(GetCategory), "Category", new { id }), Rel = "self", Method = "GET" },
                    new() { Href = _linkGenerator.GetPathByAction(nameof(UpdateCategory), "Category", new { id }), Rel = "update-category", Method = "PUT" },
                    new() { Href = _linkGenerator.GetPathByAction(nameof(DeleteCategory), "Category", new { id }), Rel = "delete-category", Method = "DELETE" }
                }
            };

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResourceResponse<Category>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResourceResponse<Category>>> AddCategory(AddCategoryCommand command)
        {
            var category = await _mediator.Send(command);

            var response = new ResourceResponse<Category>
            {
                Data = category,
                Links =
                {
                    new() { Href = _linkGenerator.GetPathByAction(nameof(GetCategory), "Category", new { id = category.Id }), Rel = "self", Method = "GET" },
                    new() { Href = _linkGenerator.GetPathByAction(nameof(UpdateCategory), "Category", new { id = category.Id }), Rel = "update-category", Method = "PUT" },
                    new() { Href = _linkGenerator.GetPathByAction(nameof(DeleteCategory), "Category", new { id = category.Id }), Rel = "delete-category", Method = "DELETE" }
                }
            };

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResourceResponse<Category>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponse<Category>>> UpdateCategory(int id, UpdateCategoryCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Category ID mismatch");
            }

            var category = await _mediator.Send(command);
            if (category == null)
            {
                return NotFound();
            }

            var response = new ResourceResponse<Category>
            {
                Data = category,
                Links =
                {
                    new() { Href = _linkGenerator.GetPathByAction(nameof(GetCategory), "Category", new { id }), Rel = "self", Method = "GET" },
                    new() { Href = _linkGenerator.GetPathByAction(nameof(UpdateCategory), "Category", new { id }), Rel = "update-category", Method = "PUT" },
                    new() { Href = _linkGenerator.GetPathByAction(nameof(DeleteCategory), "Category", new { id }), Rel = "delete-category", Method = "DELETE" }
                }
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand { CategoryId = id });
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}