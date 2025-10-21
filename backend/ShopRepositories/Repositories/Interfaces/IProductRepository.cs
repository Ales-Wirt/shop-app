using Shop.Domain.Entities;

namespace Shop.Repositories.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<(IReadOnlyList<Product> Items, int Total)> SearchAsync(ProductQuery query, CancellationToken cancellationToken);
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken);
    }
}
