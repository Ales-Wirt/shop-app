namespace Shop.Web.Options
{
    public class ShopOptions
    {
        public decimal? FixedDeliveryCost { get; set; } = null;

        public int BlobSasMinutes { get; set; } = 60;
    }
}
