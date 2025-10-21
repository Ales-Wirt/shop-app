using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using Shop.Repositories.UnitsOfWork.Interfaces;
using Shop.Web.Options;
using Shop.Web.Services;

var builder = WebApplication.CreateBuilder(args);

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
    var cs = builder.Configuration.GetConnectionString("AzureBlob");
    return new BlobServiceClient(cs);
});

builder.Services.AddSingleton<IBlobUrlGenerator, AzureBlobService>();


DefaultAzureCredential credential = new DefaultAzureCredential(options);

string blobServiceEndpoint = $"https://{storageAccountName}.blob.core.windows.net";

BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(blobServiceEndpoint), credential);

string containerName = "shopimagesblob";

BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

bool exists = await containerClient.ExistsAsync();

if(!exists)
{
    await blobServiceClient.CreateBlobContainerAsync(containerName);
}

string blobName = "heic1501a.jpg";
BlobClient blobClient = containerClient.GetBlobClient(blobName);
await using var upload = File.OpenRead(@"C:\Users\AlesWirt\Work\pet-projects\shop-app\backend\src\data\heic1501a.jpg");

await blobClient.UploadAsync(upload, new BlobUploadOptions
{
    HttpHeaders = new BlobHttpHeaders { ContentType = "image/jpeg" }
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
    builder.Services.AddDbContextFactory<ShopDbContext>(opt => opt.UseSqlServer(connectinoString));
else
    builder.Services.AddDbContext<ShopDbContext>(opt => opt.UseSqlServer(connectinoString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
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

builder.Services.AddScoped<IShopUnitOfWork, Shop.Repositories.UnitsOfWork.ShopUnitOfWork>();

app.UseHttpsRedirection();
app.MapGet("/", () => "E-shop API running");
app.MapGet("/products", (ShopDbContext context) =>
{
    
    return context.Products.ToList();
});
app.Run();
