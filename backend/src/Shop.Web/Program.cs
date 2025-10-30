using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Shop.Repositories.Repositories.Interfaces;
using Shop.Repositories.UnitsOfWork.Interfaces;
using Shop.Web;
using Shop.Web.Options;
using Shop.Web.Services;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();

        //policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var vaultUri = builder.Configuration["KeyVault_VaultUri"];

if(!string.IsNullOrEmpty(vaultUri))
{
    var client = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
    builder.Configuration.AddAzureKeyVault(client, new KeyVaultSecretManager());
}

DefaultAzureCredentialOptions options = new()
{
    ExcludeEnvironmentCredential = true,
    ExcludeManagedIdentityCredential = true,
};


var storageAccountName = "shopappblobstorage";

var connectinoString = builder.Configuration.GetConnectionString("SqlServer");

builder.Services.Configure<ShopOptions>(builder.Configuration.GetSection("Shop"));
builder.Services.AddSingleton<IOrderNumberGenerator, DefaultOrderNumberGenerator>();

builder.Services.AddSingleton(sp =>
{
    DefaultAzureCredential credential = new DefaultAzureCredential(options);

    string blobServiceEndpoint = $"https://{storageAccountName}.blob.core.windows.net";

    return new BlobServiceClient(new Uri(blobServiceEndpoint), credential);
});

builder.Services.AddSingleton<IBlobUrlGenerator, AzureBlobService>();
builder.Services.AddTransient<DataSeeder>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

if (builder.Environment.IsDevelopment())
    builder.Services.AddDbContextFactory<ShopDbContext>(opt => opt.UseSqlServer(connectinoString));
else
    builder.Services.AddDbContext<ShopDbContext>(opt => opt.UseSqlServer(connectinoString));

builder.Services.AddScoped<IProductRepository, Shop.Repositories.Repositories.ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, Shop.Repositories.Repositories.CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, Shop.Repositories.Repositories.OrderRepository>();

builder.Services.AddScoped<IShopUnitOfWork, Shop.Repositories.UnitsOfWork.ShopUnitOfWork>();
//builder.Services.AddAzureClients(clientBuilder =>
//{
//    clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnection:blobServiceUri"]!).WithName("StorageConnection");
//    clientBuilder.AddQueueServiceClient(builder.Configuration["StorageConnection:queueServiceUri"]!).WithName("StorageConnection");
//    clientBuilder.AddTableServiceClient(builder.Configuration["StorageConnection:tableServiceUri"]!).WithName("StorageConnection");
//});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ShopDbContext>>();
        await using var db = await factory.CreateDbContextAsync();
        await db.Database.MigrateAsync();
    }
}


app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseRouting();
app.MapControllers();
app.MapGet("/", () => "E-shop API running");
app.MapGet("/products", (ShopDbContext context) =>
{
    return context.Products.ToList();
});

//await app.Services.MigrateAndSeedAsync();

app.Run();
