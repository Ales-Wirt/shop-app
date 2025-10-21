namespace Shop.Web.Services
{
    public interface IBlobUrlGenerator
    {
        string GetReadUrl(string container, string blobName, DateTimeOffset? expiresOn = null);
    }
}
