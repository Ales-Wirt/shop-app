using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;

public class ShopDbContext : DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Product>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Sku).HasMaxLength(64).IsRequired();
            e.HasIndex(x => x.Sku).IsUnique();

            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.Name);

            e.Property(x => x.Slug).HasMaxLength(200);
            e.HasIndex(x => x.Slug);

            e.Property(x => x.Description);

            e.Property(x => x.Price).HasPrecision(18, 2);
            e.HasIndex(x => x.Price);

            e.Property(x => x.InStock).HasDefaultValue(true);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasMany(x => x.Images)
             .WithOne(i => i.Product)
             .HasForeignKey(i => i.ProductId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(x => x.Categories)
             .WithMany(c => c.Products)
             .UsingEntity<Dictionary<string, object>>(
                "ProductCategory",
                right => right.HasOne<Category>()
                              .WithMany()
                              .HasForeignKey("CategoryId")
                              .OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<Product>()
                            .WithMany()
                            .HasForeignKey("ProductId")
                            .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("ProductCategory");
                    join.HasKey("ProductId", "CategoryId");
                    join.HasIndex("CategoryId");
                    join.HasIndex("ProductId");
                });
        });

        b.Entity<ProductImage>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.ProductId).IsRequired();

            e.Property(x => x.BlobName).HasMaxLength(300).IsRequired();
            e.Property(x => x.Container).HasMaxLength(63).IsRequired();
            e.Property(x => x.ContentType).HasMaxLength(127);
            e.Property(x => x.SizeBytes);
            e.Property(x => x.SortOrder).HasDefaultValue(0);
            e.Property(x => x.Alt).HasMaxLength(200);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(x => new { x.ProductId, x.SortOrder });
        });

        b.Entity<Category>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(150).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(150).IsRequired();
            e.HasIndex(x => x.Slug).IsUnique();
        });

        b.Entity<Order>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.OrderNumber).HasMaxLength(32).IsRequired();
            e.HasIndex(x => x.OrderNumber).IsUnique();

            e.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(32).IsRequired();
            e.Property(x => x.Email).HasMaxLength(256).IsRequired();

            e.Property(x => x.TotalCost).HasPrecision(18, 2);
            e.Property(x => x.Status).HasMaxLength(32).HasDefaultValue("New");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasMany(x => x.Items)
             .WithOne(i => i.Order)
             .HasForeignKey(i => i.OrderId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<OrderItem>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Quantity).IsRequired();
            e.Property(x => x.UnitPrice).HasPrecision(18, 2);
            e.Property(x => x.LineTotal).HasPrecision(18, 2);

            e.HasIndex(x => x.OrderId);

            e.HasOne(x => x.Product)
             .WithMany()
             .HasForeignKey(x => x.ProductId)
             .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
