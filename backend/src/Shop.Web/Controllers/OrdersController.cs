using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Web.Options;
using Shop.Domain.Entities;
using Shop.Web.Dto.Order;
using Shop.Web.Services;
using Shop.Repositories.UnitsOfWork.Interfaces;

namespace Shop.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class OrdersController(
        IShopUnitOfWork uow,
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
            var products = await uow.Products.GetByIdsAsync(ids, ct);

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

            await uow.Orders.AddAsync(order, ct);
            try
            {
                await uow.SaveChangesAsync(ct);
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
            Console.WriteLine($"TAKING ORDER: {orderNumber}");
            try
            {
                var o = await uow.Orders.GetByOrderNumberAsync(orderNumber, ct);
                if (o is null) return NotFound();

                return Ok(new OrderCreatedDto
                {
                    OrderNumber = o.OrderNumber,
                    TotalCost = o.TotalCost,
                    Status = o.Status
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest(new
                {
                    ExceptionMessage = e.Message,
                });
            }
            finally
            {
                Console.WriteLine("<<<<<<< GetByNumber method was completed >>>>>>>");
            }
            
        }
    }
}
