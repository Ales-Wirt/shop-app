using Microsoft.AspNetCore.Mvc;
using Shop.Repositories;
using Shop.Repositories.UnitsOfWork.Interfaces;
using Shop.Web.Dto.Product;
using Shop.Web.Services;

namespace Shop.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IShopUnitOfWork uow, IBlobUrlGenerator blob) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetProducts(
        [FromQuery] string? q,
        [FromQuery] string[]? categorySlugs,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? inStock,
        [FromQuery] ProductSort sort = ProductSort.NameAsc,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        {
            if (page < 1 || pageSize is < 1 or > 100) return BadRequest("Invalid paging");

            var (items, total) = await uow.Products.SearchAsync(
                new ProductQuery(
                    q, categorySlugs, minPrice, maxPrice, inStock, sort, page, pageSize), ct);

            var dtos = items.Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                Slug = p.Slug,
                Price = p.Price,
                InStock = p.InStock,
                ThumbnailUrl = p.Images
                    .OrderBy(i => i.SortOrder)
                    .Select(i => blob.GetReadUrl(i.Container, i.BlobName))
                    .FirstOrDefault()
            }).ToList();

            return Ok(new PagedResult<ProductListItemDto>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = dtos
            });
        }

        [HttpGet("{idOrSlug}")]
        public async Task<ActionResult<ProductDetailsDto>> GetProduct(string idOrSlug, CancellationToken ct)
        {
            var prod = Guid.TryParse(idOrSlug, out var id)
                ? await uow.Products.GetByIdAsync(id, ct)
                : await uow.Products.GetBySlugAsync(idOrSlug, ct);

            if (prod is null) return NotFound();

            var dto = new ProductDetailsDto
            {
                Id = prod.Id,
                Sku = prod.Sku,
                Name = prod.Name,
                Slug = prod.Slug,
                Price = prod.Price,
                InStock = prod.InStock,
                Description = prod.Description,
                Categories = prod.Categories.OrderBy(c => c.Name).Select(c => c.Name).ToList(),
                Images = prod.Images.OrderBy(i => i.SortOrder).Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    Url = blob.GetReadUrl(i.Container, i.BlobName),
                    Alt = i.Alt,
                    SortOrder = i.SortOrder
                }).ToList()
            };

            return Ok(dto);
        }
    }
}
