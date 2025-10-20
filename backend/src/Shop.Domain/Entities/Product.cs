namespace Shop.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = null!;
        public string Name { get; set; } = null!;

        public string? Slug { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public bool InStock { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
