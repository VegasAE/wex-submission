using System.Net;
using System.Net.Http.Json;
using WexCardApi.Models;

namespace CardAPI.Test.Integration;

public class CardsIntegrationTest : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;

    public CardsIntegrationTest(CustomWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCard_ValidCreditLimit_Returns201()
    {
        var response = await _client.PostAsJsonAsync("/api/cards", new { creditLimit = 5000 });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var card = await response.Content.ReadFromJsonAsync<Card>();
        Assert.NotNull(card);
        Assert.Equal(5000, card.CreditLimit);
        Assert.NotEqual(Guid.Empty, card.Id);
    }

    [Fact]
    public async Task CreateCard_NegativeCreditLimit_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/cards", new { creditLimit = -100 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCard_ZeroCreditLimit_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/cards", new { creditLimit = 0 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCard_MissingBody_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/cards", new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCard_ExistingCard_Returns200()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/cards", new { creditLimit = 3000 });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<Card>();

        var response = await _client.GetAsync($"/api/cards/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var card = await response.Content.ReadFromJsonAsync<Card>();
        Assert.NotNull(card);
        Assert.Equal(3000, card.CreditLimit);
    }

    [Fact]
    public async Task GetCard_NonExistentCard_Returns404()
    {
        var response = await _client.GetAsync($"/api/cards/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
