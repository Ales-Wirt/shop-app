namespace Shop.Web.Options;

public sealed class ShopOptions
{
    public decimal? FixedDeliveryCost { get; set; } = null;
    public int BlobSasMinutes { get; set; } = 60;
    public string ProductImagesContainer { get; set; } = "products";
    public bool SeedOnStartup { get; set; } = true;
}
