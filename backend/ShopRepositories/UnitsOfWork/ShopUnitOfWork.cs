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

        public ShopUnitOfWork(ShopDbContext db)
        {
            _db = db;
            Products = new ProductRepository(db);
            Categories = new CategoryRepository(db);
            Orders = new OrderRepository(db);
        }

        public Task<int> SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);

        public ValueTask DisposeAsync() => _db.DisposeAsync();
    }
}
