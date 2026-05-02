using System.Net;
using System.Net.Http.Json;
using WexCardApi.Models;

namespace CardAPI.Test.Integration;

public class TransactionsIntegrationTest : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;

    public TransactionsIntegrationTest(CustomWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<Card> CreateTestCard(decimal creditLimit = 5000)
    {
        var response = await _client.PostAsJsonAsync("/api/cards", new { creditLimit });
        var card = await response.Content.ReadFromJsonAsync<Card>();
        return card!;
    }

    [Fact]
    public async Task CreateTransaction_ValidPayload_Returns201()
    {
        var card = await CreateTestCard();

        var response = await _client.PostAsJsonAsync("/api/transactions", new
        {
            description = "Test purchase",
            cardId = card.Id,
            amountUsd = 100
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var transaction = await response.Content.ReadFromJsonAsync<Transaction>();
        Assert.NotNull(transaction);
        Assert.Equal("Test purchase", transaction.Description);
        Assert.Equal(100, transaction.AmountUsd);
        Assert.Equal(card.Id, transaction.CardId);
        Assert.NotEqual(Guid.Empty, transaction.Id);
    }

    [Fact]
    public async Task CreateTransaction_InvalidCardId_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/transactions", new
        {
            description = "Test purchase",
            cardId = Guid.NewGuid(),
            amountUsd = 100
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTransaction_NegativeAmount_Returns400()
    {
        var card = await CreateTestCard();

        var response = await _client.PostAsJsonAsync("/api/transactions", new
        {
            description = "Test purchase",
            cardId = card.Id,
            amountUsd = -50
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTransaction_MissingDescription_Returns400()
    {
        var card = await CreateTestCard();

        var response = await _client.PostAsJsonAsync("/api/transactions", new
        {
            cardId = card.Id,
            amountUsd = 100
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetTransaction_ExistingTransaction_Returns200()
    {
        var card = await CreateTestCard();

        var createResponse = await _client.PostAsJsonAsync("/api/transactions", new
        {
            description = "Test purchase",
            cardId = card.Id,
            amountUsd = 250
        });
        var created = await createResponse.Content.ReadFromJsonAsync<Transaction>();

        var response = await _client.GetAsync($"/api/transactions/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var transaction = await response.Content.ReadFromJsonAsync<Transaction>();
        Assert.NotNull(transaction);
        Assert.Equal("Test purchase", transaction.Description);
        Assert.Equal(250, transaction.AmountUsd);
    }

    [Fact]
    public async Task GetTransaction_NonExistentTransaction_Returns404()
    {
        var response = await _client.GetAsync($"/api/transactions/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
