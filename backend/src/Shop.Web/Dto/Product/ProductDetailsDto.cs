namespace Shop.Web.Dto.Product
{
    public class ProductDetailsDto
    {
        public required Guid Id { get; init; }
        public required string Sku { get; init; }
        public required string Name { get; init; }
        public string? Slug { get; init; }
        public required decimal Price { get; init; }
        public required bool InStock { get; init; }
        public string? Description { get; init; }
        public required IReadOnlyList<string> Categories { get; init; }
        public required IReadOnlyList<ProductImageDto> Images { get; init; }
    }
}
