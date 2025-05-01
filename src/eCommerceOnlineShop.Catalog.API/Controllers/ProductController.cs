using eCommerceOnlineShop.Catalog.API.Models;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.AddProduct;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.DeleteProduct;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProducts;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.UpdateProduct;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceOnlineShop.Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly LinkGenerator _linkGenerator;

        public ProductController(IMediator mediator, LinkGenerator linkGenerator)
        {
            _mediator = mediator;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResourceResponse<IEnumerable<Product>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResourceResponse<IEnumerable<Product>>>> GetProducts(
            [FromQuery] int? categoryId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var products = await _mediator.Send(new GetProductsQuery
            {
                CategoryId = categoryId,
                Page = page,
                PageSize = pageSize
            });

            var response = new ResourceResponse<IEnumerable<Product>>
            {
                Data = products,
                Links = new List<Link>
                {
                    new() { Href = _linkGenerator.GetPathByAction(nameof(GetProducts), "Product"), Rel = "self", Method = "GET" },
                    new() { Href = _linkGenerator.GetPathByAction(nameof(AddProduct), "Product"), Rel = "create-product", Method = "POST" }
                }
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResourceResponse<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponse<Product>>> GetProduct(int id)
        {
            var product = await _mediator.Send(new GetProductQuery { ProductId = id });
            if (product == null)
            {
                return NotFound();
            }

            var response = new ResourceResponse<Product>
            {
                Data = product,
                Links = new List<Link>
                {
                    new() { Href = _linkGenerator.GetPathByAction(nameof(GetProduct), "Product", new { id }), Rel = "self", Method = "GET" },
                    new() { Href = _linkGenerator.GetPathByAction(nameof(UpdateProduct), "Product", new { id }), Rel = "update-product", Method = "PUT" },
                    new() { Href = _linkGenerator.GetPathByAction(nameof(DeleteProduct), "Product", new { id }), Rel = "delete-product", Method = "DELETE" }
                }
            };

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResourceResponse<Product>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResourceResponse<Product>>> AddProduct(AddProductCommand command)
        {
            var product = await _mediator.Send(command);

            var response = new ResourceResponse<Product>
            {
                Data = product,
                Links = new List<Link>
                {
                    new() { Href = _linkGenerator.GetPathByAction(nameof(GetProduct), "Product", new { id = product.Id }), Rel = "self", Method = "GET" },
                    new() { Href = _linkGenerator.GetPathByAction(nameof(UpdateProduct), "Product", new { id = product.Id }), Rel = "update-product", Method = "PUT" },
                    new() { Href = _linkGenerator.GetPathByAction(nameof(DeleteProduct), "Product", new { id = product.Id }), Rel = "delete-product", Method = "DELETE" }
                }
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResourceResponse<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponse<Product>>> UpdateProduct(int id, UpdateProductCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Product ID mismatch");
            }

            var product = await _mediator.Send(command);
            if (product == null)
            {
                return NotFound();
            }

            var response = new ResourceResponse<Product>
            {
                Data = product,
                Links = new List<Link>
                {
                    new() { Href = _linkGenerator.GetPathByAction(nameof(GetProduct), "Product", new { id }), Rel = "self", Method = "GET" },
                    new() { Href = _linkGenerator.GetPathByAction(nameof(UpdateProduct), "Product", new { id }), Rel = "update-product", Method = "PUT" },
                    new() { Href = _linkGenerator.GetPathByAction(nameof(DeleteProduct), "Product", new { id }), Rel = "delete-product", Method = "DELETE" }
                }
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _mediator.Send(new DeleteProductCommand { ProductId = id });
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}