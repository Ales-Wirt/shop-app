namespace Shop.Web.Dto.Product
{
    public class ProductListItemDto
    {
        public required Guid Id { get; init; }
        public required string Sku { get; init; }
        public required string Name { get; init; }
        public string? Slug { get; init; }
        public required decimal Price { get; init; }
        public required bool InStock { get; init; }
        public string? ThumbnailUrl { get; init; }
    }
}
