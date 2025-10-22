using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shop.Web.Services;
using Shop.Domain.Entities;
using System.Data.Common;
using Shop.Repositories.UnitsOfWork.Interfaces;
using Shop.Repositories.UnitsOfWork;
using Shop.Web.Options;

public sealed class TestWebAppFactory : WebApplicationFactory<Program>
{
    private DbConnection _conn = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            _conn = new SqliteConnection("Filename=:memory:");
            _conn.Open();

            services.RemoveAll(typeof(DbContextOptions<ShopDbContext>));
            services.AddDbContext<ShopDbContext>(opt => opt.UseSqlite(_conn));

            services.RemoveAll(typeof(IShopUnitOfWork));
            services.AddScoped<IShopUnitOfWork, ShopUnitOfWork>();

            services.RemoveAll(typeof(IBlobUrlGenerator));
            services.AddSingleton<IBlobUrlGenerator, TestBlobUrlGenerator>();

            services.Configure<ShopOptions>(o =>
            {
                o.FixedDeliveryCost = 15.50m;
                o.BlobSasMinutes = 5;
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
            db.Database.EnsureCreated();
            Seed(db);
        });
    }

    private static void Seed(ShopDbContext db)
    {
        var catPhones = new Category { Id = Guid.NewGuid(), Name = "Smartphones", Slug = "phones" };
        var catLaptops = new Category { Id = Guid.NewGuid(), Name = "Laptops", Slug = "laptops" };

        var p1 = new Product
        {
            Id = Guid.NewGuid(),
            Sku = "SKU-IPH-13",
            Name = "iPhone 13",
            Slug = "iphone-13",
            Price = 999.00m,
            InStock = true,
            Description = "Description иПхона",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        p1.Categories.Add(catPhones);
        p1.Images.Add(new ProductImage
        {
            Id = Guid.NewGuid(),
            ProductId = p1.Id,
            Container = "products",
            BlobName = "iphone13.png",
            SortOrder = 0,
            Alt = "iPhone 13",
            CreatedAt = DateTime.UtcNow
        });

        var p2 = new Product
        {
            Id = Guid.NewGuid(),
            Sku = "SKU-MBP-14",
            Name = "MacBook Pro 14",
            Slug = "mbp-14",
            Price = 1999.00m,
            InStock = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        p2.Categories.Add(catLaptops);

        db.AddRange(catPhones, catLaptops, p1, p2);
        db.SaveChanges();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _conn.Dispose();
    }
}

file sealed class TestBlobUrlGenerator : IBlobUrlGenerator
{
    public string GetReadUrl(string container, string blobName, DateTimeOffset? expiresOn = null)
        => $"https://cdn.test/{container}/{blobName}";
}
