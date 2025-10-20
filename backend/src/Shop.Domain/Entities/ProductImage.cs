using Shop.Domain.Entities;

public class ProductImage
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }

    public string BlobName { get; set; } = null!;
    public string Container { get; set; } = null!;

    public string? ContentType { get; set; }
    public long? SizeBytes { get; set; }
    public int SortOrder { get; set; }
    public string? Alt { get; set; }
    public DateTime CreatedAt { get; set; }

    public Product Product { get; set; } = null!;
}
