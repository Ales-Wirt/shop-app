using Shop.Domain.Entities;

namespace Shop.Repositories.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order, CancellationToken cancellationToken);
        Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken);
    }
}
