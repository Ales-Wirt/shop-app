using Shop.Web.Dto.Order;
using Shop.Web.Dto.Product;
using System.Net;
using System.Net.Http.Json;

public sealed class OrdersControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;
    public OrdersControllerTests(TestWebAppFactory f) => _client = f.CreateClient();

    [Fact]
    public async Task Create_Computes_Totals_And_Returns_Number()
    {
        // Получим продукт из списка (чтобы взять Guid)
        var list = await _client.GetFromJsonAsync<PagedResult<ProductListItemDto>>("/api/products?q=iphone");
        var iph = list!.Items.First();

        var req = new OrderCreateDto
        {
            FirstName = "Ivan",
            LastName = "Petrov",
            Phone = "1234567890",
            Email = "ivan@example.com",
            Items = new() { new OrderItemCreateDto { ProductId = iph.Id, Quantity = 2 } }
        };

        var resp = await _client.PostAsJsonAsync("/api/orders", req);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var dto = await resp.Content.ReadFromJsonAsync<OrderCreatedDto>();
        Assert.NotNull(dto);
        Assert.StartsWith("ORD-", dto!.OrderNumber);
        Assert.Equal(1998.00m, dto.TotalCost);

        var get = await _client.GetAsync($"/api/orders/{dto.OrderNumber}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
    }

    [Fact]
    public async Task Create_Rejects_OutOfStock()
    {
        var list = await _client.GetFromJsonAsync<PagedResult<ProductListItemDto>>("/api/products?q=macbook");
        var mbp = list!.Items.First();

        var req = new OrderCreateDto
        {
            FirstName = "Ivan",
            LastName = "Petrov",
            Phone = "1234567890",
            Email = "ivan@example.com",
            Items = new() { new OrderItemCreateDto { ProductId = mbp.Id, Quantity = 1 } }
        };

        var resp = await _client.PostAsJsonAsync("/api/orders", req);
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        var body = await resp.Content.ReadAsStringAsync();
        Assert.Contains("out of stock", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Create_InvalidEmail_Returns400()
    {
        var list = await _client.GetFromJsonAsync<PagedResult<ProductListItemDto>>("/api/products?q=iphone");
        var prod = list!.Items.First();
        var req = new OrderCreateDto
        {
            FirstName = "Ivan",
            LastName = "Petrov",
            Phone = "1234567890",
            Email = "bad-email",
            Items = new() { new OrderItemCreateDto { ProductId = prod.Id, Quantity = 1 } }
        };

        var resp = await _client.PostAsJsonAsync("/api/orders", req);
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }
}
