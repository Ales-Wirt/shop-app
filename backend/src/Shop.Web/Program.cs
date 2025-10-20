using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var vaultUri = builder.Configuration["KeyVault_VaultUri"];

if(!string.IsNullOrEmpty(vaultUri))
{
    var client = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
    builder.Configuration.AddAzureKeyVault(client, new KeyVaultSecretManager());
}

var connectinoString = builder.Configuration.GetConnectionString("SqlServer");



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    builder.Services.AddDbContextFactory<ShopDbContext>(opt => opt.UseSqlServer(connectinoString));
    using (var scope = app.Services.CreateScope())
    {
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ShopDbContext>>();
        await using var db = await factory.CreateDbContextAsync();
        await db.Database.MigrateAsync();
    }
    
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}
else
{
    builder.Services.AddDbContext<ShopDbContext>(opt => opt.UseSqlServer(connectinoString));
}

    app.UseHttpsRedirection();
app.MapGet("/", () => "E-shop API running");
app.MapGet("/products", (ShopDbContext context) =>
{
    
    return context.Products.ToList();
});
app.Run();
