namespace Shop.Repositories
{
    public record ProductQuery (
        string? Q,
        IReadOnlyCollection<string> CategorySlugs,
        decimal? MinPrice,
        decimal? MaxPrice,
        bool? InStock,
        ProductSort Sort,
        int Page,
        int PageSize);
}
