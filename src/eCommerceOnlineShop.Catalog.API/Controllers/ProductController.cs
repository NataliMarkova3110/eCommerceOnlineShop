using eCommerceOnlineShop.Catalog.API.Models;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.AddProduct;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.DeleteProduct;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProduct;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProducts;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.UpdateProduct;
using eCommerceOnlineShop.Catalog.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceOnlineShop.Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IMediator mediator, LinkGenerator linkGenerator) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ResourceResponse<IEnumerable<Product>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResourceResponse<IEnumerable<Product>>>> GetProductsAsync(
            [FromQuery] int? categoryId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var products = await mediator.Send(new GetProductsQuery
            {
                CategoryId = categoryId,
                Page = page,
                PageSize = pageSize
            });

            var response = new ResourceResponse<IEnumerable<Product>>
            {
                Data = products,
                Links =
                [
                    new() { Href = linkGenerator.GetPathByAction(nameof(GetProductsAsync), "Product") ?? "/api/Product", Rel = "self", Method = "GET" },
                    new() { Href = linkGenerator.GetPathByAction(nameof(AddProductAsync), "Product") ?? "/api/Product", Rel = "create-product", Method = "POST" }
                ]
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResourceResponse<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponse<Product>>> GetProductAsync(int id)
        {
            var product = await mediator.Send(new GetProductCommand { ProductId = id });
            if (product == null)
            {
                return NotFound();
            }

            var response = new ResourceResponse<Product>
            {
                Data = product,
                Links =
                [
                    new() { Href = linkGenerator.GetPathByAction(nameof(GetProductAsync), "Product", new { id }) ?? $"/api/Product/{id}", Rel = "self", Method = "GET" },
                    new() { Href = linkGenerator.GetPathByAction(nameof(UpdateProductAsync), "Product", new { id }) ?? $"/api/Product/{id}", Rel = "update-product", Method = "PUT" },
                    new() { Href = linkGenerator.GetPathByAction(nameof(DeleteProductAsync), "Product", new { id }) ?? $"/api/Product/{id}", Rel = "delete-product", Method = "DELETE" }
                ]
            };

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResourceResponse<Product>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<ResourceResponse<Product>>> AddProductAsync(AddProductCommand command)
        {
            var product = await mediator.Send(command);

            var response = new ResourceResponse<Product>
            {
                Data = product,
                Links =
                {
                    new() { Href = linkGenerator.GetPathByAction(nameof(GetProductAsync), "Product", new { id = product.Id }) ?? $"/api/Product/{product.Id}", Rel = "self", Method = "GET" },
                    new() { Href = linkGenerator.GetPathByAction(nameof(UpdateProductAsync), "Product", new { id = product.Id }) ?? $"/api/Product/{product.Id}", Rel = "update-product", Method = "PUT" },
                    new() { Href = linkGenerator.GetPathByAction(nameof(DeleteProductAsync), "Product", new { id = product.Id }) ?? $"/api/Product/{product.Id}", Rel = "delete-product", Method = "DELETE" }
                }
            };

            return CreatedAtAction(nameof(GetProductAsync), new { id = product.Id }, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResourceResponse<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<ResourceResponse<Product>>> UpdateProductAsync(int id, UpdateProductCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Product ID mismatch");
            }

            var product = await mediator.Send(command);
            if (product == null)
            {
                return NotFound();
            }

            var response = new ResourceResponse<Product>
            {
                Data = product,
                Links =
                {
                    new() { Href = linkGenerator.GetPathByAction(nameof(GetProductAsync), "Product", new { id }) ?? $"/api/Product/{id}", Rel = "self", Method = "GET" },
                    new() { Href = linkGenerator.GetPathByAction(nameof(UpdateProductAsync), "Product", new { id }) ?? $"/api/Product/{id}", Rel = "update-product", Method = "PUT" },
                    new() { Href = linkGenerator.GetPathByAction(nameof(DeleteProductAsync), "Product", new { id }) ?? $"/api/Product/{id}", Rel = "delete-product", Method = "DELETE" }
                }
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var result = await mediator.Send(new DeleteProductCommand { ProductId = id });
            return !result ? NotFound() : NoContent();
        }
    }
}