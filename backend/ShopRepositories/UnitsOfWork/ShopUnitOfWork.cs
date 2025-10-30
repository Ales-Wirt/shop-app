using Shop.Repositories.Repositories;
using Shop.Repositories.Repositories.Interfaces;
using Shop.Repositories.UnitsOfWork.Interfaces;

namespace Shop.Repositories.UnitsOfWork
{
    public class ShopUnitOfWork : IShopUnitOfWork, IAsyncDisposable
    {
        private readonly ShopDbContext _db;

        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }
        public IOrderRepository Orders { get; }

        public ShopUnitOfWork(
            ShopDbContext db,
            IProductRepository products,
            ICategoryRepository categories,
            IOrderRepository orders)
        {
            _db = db;
            Products = products;
            Categories = categories;
            Orders = orders;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);

        public ValueTask DisposeAsync() => _db.DisposeAsync();
    }
}
