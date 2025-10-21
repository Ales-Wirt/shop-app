using System.ComponentModel.DataAnnotations;

namespace Shop.Web.Dto.Order
{
    public class OrderCreateDto
    {
        [Required, MaxLength(100)]
        public required string FirstName { get; init; }

        [Required, MaxLength(100)]
        public required string LastName { get; init; }

        [Required, Phone, MaxLength(32)]
        public required string Phone { get; init; }

        [Required, EmailAddress, MaxLength(256)]
        public required string Email { get; init; }

        [MinLength(1)]
        public required List<OrderItemCreateDto> Items { get; init; }
    }
}
