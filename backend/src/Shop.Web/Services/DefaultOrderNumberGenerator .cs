namespace Shop.Web.Services
{
    public class DefaultOrderNumberGenerator : IOrderNumberGenerator
    {
        public string Next() => $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";
    }
}
