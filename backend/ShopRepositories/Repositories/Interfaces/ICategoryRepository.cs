using Shop.Domain.Entities;

namespace Shop.Repositories.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken);
    }
}
