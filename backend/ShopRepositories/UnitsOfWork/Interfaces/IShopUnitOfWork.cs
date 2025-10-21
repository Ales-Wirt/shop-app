using Shop.Repositories.Repositories.Interfaces;

namespace Shop.Repositories.UnitsOfWork.Interfaces
{
    public interface IShopUnitOfWork
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories{ get; }
        IOrderRepository Orders{ get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
