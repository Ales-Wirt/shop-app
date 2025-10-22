using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using Shop.Repositories.UnitsOfWork.Interfaces;
using Shop.Web;
using Shop.Web.Options;
using Shop.Web.Services;
using System;

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
    DefaultAzureCredential credential = new DefaultAzureCredential(options);

    string blobServiceEndpoint = $"https://{storageAccountName}.blob.core.windows.net";

    return new BlobServiceClient(new Uri(blobServiceEndpoint), credential);
});

builder.Services.AddSingleton<IBlobUrlGenerator, AzureBlobService>();
builder.Services.AddTransient<DataSeeder>();


//string containerName = "shopimagesblob";

//BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

//bool exists = await containerClient.ExistsAsync();

//if(!exists)
//{
//    await blobServiceClient.CreateBlobContainerAsync(containerName);
//}

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


app.UseHttpsRedirection();
app.MapGet("/", () => "E-shop API running");
app.MapGet("/products", (ShopDbContext context) =>
{
    
    return context.Products.ToList();
});

await app.Services.MigrateAndSeedAsync();

app.Run();
