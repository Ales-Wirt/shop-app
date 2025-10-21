using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using Shop.Web.Dto.Order;
using Shop.Web.Options;
using Shop.Web.Services;

namespace Shop.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class OrdersController(
        ShopDbContext db,
        IOrderNumberGenerator num,
        Microsoft.Extensions.Options.IOptions<ShopOptions> opt)
        : ControllerBase
    {
        private readonly ShopOptions _opt = opt.Value;

        // POST /api/orders
        [HttpPost]
        public async Task<ActionResult<OrderCreatedDto>> Create([FromBody] OrderCreateDto req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var ids = req.Items.Select(i => i.ProductId).Distinct().ToArray();
            var products = await db.Products
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, ct);

            if (products.Count != ids.Length)
                return BadRequest("Some products do not exist.");

            foreach (var item in req.Items)
            {
                var p = products[item.ProductId];
                if (!p.InStock)
                    return BadRequest($"Product '{p.Name}' is out of stock.");
                if (item.Quantity <= 0)
                    return BadRequest("Quantity must be positive.");
            }

            var totalCost = req.Items.Sum(i =>
            {
                var p = products[i.ProductId];
                return p.Price * i.Quantity;
            });

            var order = new Order
            {
                OrderNumber = num.Next(),
                FirstName = req.FirstName,
                LastName = req.LastName,
                Phone = req.Phone,
                Email = req.Email,
                TotalCost = totalCost,
                Status = "New",
                Items = req.Items.Select(i =>
                {
                    var p = products[i.ProductId];
                    return new OrderItem
                    {
                        ProductId = p.Id,
                        Quantity = i.Quantity,
                        UnitPrice = p.Price,
                        LineTotal = p.Price * i.Quantity
                    };
                }).ToList()
            };

            db.Orders.Add(order);

            try
            {
                await db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex)
            {
                return Problem("Failed to create order: " + ex.Message, statusCode: 500);
            }

            return Ok(new OrderCreatedDto
            {
                OrderNumber = order.OrderNumber,
                TotalCost = order.TotalCost,
                Status = order.Status
            });
        }

        // GET /api/orders/{orderNumber}
        [HttpGet("{orderNumber}")]
        public async Task<ActionResult<OrderCreatedDto>> GetByNumber(string orderNumber, CancellationToken ct)
        {
            var o = await db.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.OrderNumber == orderNumber, ct);
            if (o is null) return NotFound();

            return Ok(new OrderCreatedDto
            {
                OrderNumber = o.OrderNumber,
                TotalCost = o.TotalCost,
                Status = o.Status
            });
        }
    }
}
