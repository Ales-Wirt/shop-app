namespace Shop.Web.Dto.Order
{
    public class OrderCreatedDto
    {
        public required string OrderNumber { get; init; }
        public required decimal TotalCost { get; init; }
        public required string Status { get; init; }
    }
}
