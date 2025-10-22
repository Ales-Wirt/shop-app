using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shop.Web.Options;

namespace Shop.Web;

public static class MigrationExtensions
{
    public static async Task MigrateAndSeedAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
        await db.Database.MigrateAsync(ct);

        var opts = scope.ServiceProvider.GetRequiredService<IOptions<ShopOptions>>().Value;
        if (!opts.SeedOnStartup) return;

        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();

        await seeder.SeedAsync(ct);
    }
}
