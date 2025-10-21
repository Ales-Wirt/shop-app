namespace Shop.Web.Dto.Product
{
    public class ProductImageDto
    {
        public required Guid Id { get; init; }
        public required string Url { get; init; }
        public string? Alt { get; init; }
        public int SortOrder { get; init; }
    }
}
