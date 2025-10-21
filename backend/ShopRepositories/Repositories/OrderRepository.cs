using Shop.Domain.Entities;
using Shop.Repositories.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Shop.Repositories.Repositories
{
    public class OrderRepository(ShopDbContext db) : IOrderRepository
    {
        public async Task AddAsync(Order order, CancellationToken ct) =>
        await db.Orders.AddAsync(order, ct);

        public Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct) =>
            db.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, ct);
    }
}
