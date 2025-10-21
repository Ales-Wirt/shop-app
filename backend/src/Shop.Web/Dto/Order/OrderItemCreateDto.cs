using System.ComponentModel.DataAnnotations;

namespace Shop.Web.Dto.Order
{
    public class OrderItemCreateDto
    {
        [Required]
        public Guid ProductId { get; init; }

        [Range(1, 1_000)]
        public int Quantity { get; init; }
    }
}
