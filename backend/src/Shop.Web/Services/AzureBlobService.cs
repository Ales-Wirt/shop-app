using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using Shop.Web.Options;

namespace Shop.Web.Services
{
    public class AzureBlobService(BlobServiceClient client, IOptions<ShopOptions> opt) : IBlobUrlGenerator
    {
        private readonly BlobServiceClient _blobServiceClient = client;
        private readonly ShopOptions _opt = opt.Value;


        public string GetReadUrl(string container, string blobName, DateTimeOffset? expiresOn = null)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(container);
            var blobClient = containerClient.GetBlobClient(blobName);
            
            if (!blobClient.CanGenerateSasUri)
                return blobClient.Uri.ToString();

            var sas = new BlobSasBuilder(BlobSasPermissions.Read, (expiresOn ?? DateTimeOffset.UtcNow.AddMinutes(_opt.BlobSasMinutes)))
            {
                BlobContainerName = container,
                BlobName = blobName,
                Resource = "b"
            };

            var sasUri = blobClient.GenerateSasUri(sas);
            return sasUri.ToString();
        }
    }
}
