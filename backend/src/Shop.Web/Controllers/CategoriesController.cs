using Microsoft.AspNetCore.Mvc;
using Shop.Repositories.UnitsOfWork.Interfaces;
using Shop.Web.Dto.CategoryDto;

namespace Shop.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController(IShopUnitOfWork uow) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken ct)
        {
            var list = await uow.Categories.GetAllAsync(ct);
            return Ok(list.Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Slug = c.Slug }).ToList());
        }
    }
}
