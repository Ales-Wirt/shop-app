using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shop.Domain.Entities;
using Shop.Web.Options;
using System.Text.RegularExpressions;

namespace Shop.Web;

public sealed class DataSeeder(
    ShopDbContext db,
    BlobServiceClient blobClient,
    IOptions<ShopOptions> optionsAccessor)   // ← вместо ShopOptions
{
    private readonly ShopDbContext _db = db;
    private readonly BlobServiceClient _blob = blobClient;
    private readonly ShopOptions _options = optionsAccessor.Value;
    private static readonly byte[] TinyPng = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAoMBgGmU3icAAAAASUVORK5CYII=");

    private static string Slugify(string input)
    {
        var s = input.ToLowerInvariant();
        s = Regex.Replace(s, @"\s+", "-");
        s = Regex.Replace(s, @"[^a-z0-9\-]", "");
        s = Regex.Replace(s, @"-+", "-").Trim('-');
        return s;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await db.Products.AsNoTracking().AnyAsync(ct)) return;

        var containerName = _options.ProductImagesContainer;
        var container = blobClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: ct);

        var cats = new[]
        {
            new Category { Id = Guid.NewGuid(), Name = "Smartphones", Slug = "phones" },
            new Category { Id = Guid.NewGuid(), Name = "Laptops",  Slug = "laptops" },
            new Category { Id = Guid.NewGuid(), Name = "Tablests",  Slug = "tablets" },
            new Category { Id = Guid.NewGuid(), Name = "Audio",     Slug = "audio" },
            new Category { Id = Guid.NewGuid(), Name = "Cameras",      Slug = "cameras" }
        };
        db.Categories.AddRange(cats);

        var names = new[]
        {
            "iPhone 15", "Samsung Galaxy S24", "Google Pixel 9", "OnePlus 12", "Xiaomi 14",
            "MacBook Pro 14", "MacBook Air 13", "Dell XPS 13", "Lenovo ThinkPad X1", "ASUS ROG Zephyrus",
            "iPad Pro 11", "Samsung Galaxy Tab S9", "Lenovo Tab P12", "iPad Air 10.9", "Xiaomi Pad 6",
            "Sony WH-1000XM5", "AirPods Pro 2", "JBL Charge 5", "Canon EOS R10", "Sony Alpha a7C II"
        };

        var rng = new Random(1234);
        var now = DateTime.UtcNow;

        foreach (var name in names.Select((n, i) => (n, i)))
        {
            var baseSlug = Slugify(name.n);
            var code = baseSlug.Replace("-", "").ToUpperInvariant();
            var prefix = code[..Math.Min(10, code.Length)];

            var sku = $"SKU-{name.i + 1:000}-{prefix}";
            var slug = Slugify(name.n);
            var price = Math.Round(100 + (decimal)rng.NextDouble() * 2900, 2);
            var inStock = rng.Next(0, 100) < 75;

            var p = new Product
            {
                Id = Guid.NewGuid(),
                Sku = sku,
                Name = name.n,
                Slug = slug,
                Price = price,
                InStock = inStock,
                Description = $"Description for {name.n}",
                CreatedAt = now,
                UpdatedAt = now
            };

            var assign = slug switch
            {
                var s when s.Contains("iphone") || s.Contains("galaxy") || s.Contains("pixel") || s.Contains("oneplus") || s.Contains("xiaomi") => new[] { "phones" },
                var s when s.Contains("macbook") || s.Contains("xps") || s.Contains("thinkpad") || s.Contains("zephyrus") => new[] { "laptops" },
                var s when s.Contains("ipad") || s.Contains("tab") || s.Contains("pad") => new[] { "tablets" },
                var s when s.Contains("wh-1000xm5") || s.Contains("airpods") || s.Contains("jbl") => new[] { "audio" },
                var s when s.Contains("eos") || s.Contains("alpha") => new[] { "cameras" },
                _ => new[] { "phones" }
            };
            foreach (var cslug in assign)
                p.Categories.Add(cats.First(c => c.Slug == cslug));

            for (int k = 1; k <= 3; k++)
            {
                var blobName = $"{slug}-{k}.png";
                var blob = container.GetBlobClient(blobName);

                try
                {
                    using var ms = new MemoryStream(TinyPng);
                    await blob.UploadAsync(ms, new BlobHttpHeaders { ContentType = "image/png" }, cancellationToken: ct);
                }
                catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
                {
                }

                p.Images.Add(new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ProductId = p.Id,
                    Container = containerName,
                    BlobName = blobName,
                    ContentType = "image/png",
                    SizeBytes = TinyPng.LongLength,
                    SortOrder = k - 1,
                    Alt = $"{p.Name} — photo {k}",
                    CreatedAt = now
                });
            }

            db.Products.Add(p);
        }

        await db.SaveChangesAsync(ct);
    }
}
