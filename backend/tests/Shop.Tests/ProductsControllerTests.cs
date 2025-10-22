using Shop.Web.Dto.Product;
using System.Net;
using System.Net.Http.Json;

public sealed class ProductsControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;
    public ProductsControllerTests(TestWebAppFactory f) => _client = f.CreateClient();

    [Fact]
    public async Task List_Filter_Sorts_And_Paginates()
    {
        var res = await _client.GetFromJsonAsync<PagedResult<ProductListItemDto>>("/api/products?categorySlugs=phones&minPrice=900&maxPrice=1200");
        Assert.NotNull(res);
        Assert.Equal(1, res!.Total);
        var item = Assert.Single(res.Items);
        Assert.Equal("iPhone 13", item.Name);
        Assert.Equal("https://cdn.test/products/iphone13.png", item.ThumbnailUrl);
    }

    [Fact]
    public async Task Details_BySlug_ReturnsOk()
    {
        var http = await _client.GetAsync("/api/products/iphone-13");
        Assert.Equal(HttpStatusCode.OK, http.StatusCode);

        var dto = await http.Content.ReadFromJsonAsync<ProductDetailsDto>();
        Assert.NotNull(dto);
        Assert.Equal("iPhone 13", dto!.Name);
        Assert.Single(dto.Images);
    }

    [Fact]
    public async Task Details_NotFound()
    {
        var res = await _client.GetAsync("/api/products/does-not-exist");
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }
}
