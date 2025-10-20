using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Shop.Infrastructure
{
    public class ShopDbContextFactory : IDesignTimeDbContextFactory<ShopDbContext>
    {
        public ShopDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var pathToWeb = Path.Combine("..", "Shop.Web");
            var absolutePath = Path.GetFullPath(pathToWeb);

            var hostBuilder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
            {
                EnvironmentName = env,
                ContentRootPath = absolutePath,
            });
            var vaultUri = hostBuilder.Configuration["KeyVault_VaultUri"];

            if (!string.IsNullOrEmpty(vaultUri))
            {
                var client = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
                hostBuilder.Configuration.AddAzureKeyVault(client, new KeyVaultSecretManager());
            }

            var connectinoString = hostBuilder.Configuration.GetConnectionString("SqlServer");

            var options = new DbContextOptionsBuilder<ShopDbContext>().UseSqlServer(connectinoString).Options;

            return new ShopDbContext(options);
        }
    }
}
