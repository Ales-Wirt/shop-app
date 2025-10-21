using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using Shop.Repositories.Repositories.Interfaces;

namespace Shop.Repositories.Repositories
{
    internal class ProductRepository(ShopDbContext db) : IProductRepository
    {
        public async Task<(IReadOnlyList<Product> Items, int Total)> SearchAsync(ProductQuery q, CancellationToken ct)
        {
            IQueryable<Product> query = db.Products
                .AsNoTracking()
                .Include(p => p.Images)
                .Include(p => p.Categories);

            if (!string.IsNullOrWhiteSpace(q.Q))
            {
                var term = q.Q.Trim();
                query = query.Where(p => p.Name.Contains(term) || p.Sku.Contains(term));
            }
            if (q.CategorySlugs is { Count: > 0 })
            {
                var slugs = q.CategorySlugs!;
                query = query.Where(p => p.Categories.Any(c => slugs.Contains(c.Slug)));
            }
            if (q.MinPrice.HasValue) query = query.Where(p => p.Price >= q.MinPrice.Value);
            if (q.MaxPrice.HasValue) query = query.Where(p => p.Price <= q.MaxPrice.Value);
            if (q.InStock.HasValue) query = query.Where(p => p.InStock == q.InStock.Value);

            query = q.Sort switch
            {
                ProductSort.PriceAsc => query.OrderBy(p => p.Price).ThenBy(p => p.Name),
                ProductSort.PriceDesc => query.OrderByDescending(p => p.Price).ThenBy(p => p.Name),
                ProductSort.NameDesc => query.OrderByDescending(p => p.Name),
                _ => query.OrderBy(p => p.Name)
            };

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct) =>
            db.Products.AsNoTracking()
                .Include(p => p.Images).Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == id, ct);

        public Task<Product?> GetBySlugAsync(string slug, CancellationToken ct) =>
            db.Products.AsNoTracking()
                .Include(p => p.Images).Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Slug == slug, ct);
    }
}
