namespace Shop.Web.Dto.CategoryDto
{
    public sealed class CategoryDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Slug { get; init; }
    }
}
