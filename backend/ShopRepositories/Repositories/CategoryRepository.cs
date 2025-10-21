using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using Shop.Repositories.Repositories.Interfaces;

namespace Shop.Repositories.Repositories
{
    public class CategoryRepository(ShopDbContext db) : ICategoryRepository
    {
        public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct) =>
        await db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);
    }
}
