
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using WexCardApi.Controllers;
using WexCardApi.Models;
using WexCardApi.DB;
using WexCardApi.DTO;

public class TransactionTest
{

    private readonly AppDbContext _db;
    private readonly MockTreasuryClient _mockExchange;
    private readonly TransactionsController _controller;
    private readonly Card _card;

    public TransactionTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);
        _mockExchange = new MockTreasuryClient();
        _controller = new TransactionsController(_db, _mockExchange);

        _card = new Card { CreditLimit = 5000 };
        _db.Cards.Add(_card);
        _db.SaveChanges();
    }

    [Fact]
    public async Task TransactionTest_ShouldCreate()
    {
        var payload = new CreateTransactionRequest("Test purchase", _card.Id, 100);

        var resp = await _controller.CreateTransaction(payload);

        var createdResult = Assert.IsType<CreatedResult>(resp.Result);
        var transaction = Assert.IsType<Transaction>(createdResult.Value);
        Assert.Equal("Test purchase", transaction.Description);
        Assert.Equal(100, transaction.AmountUsd);
        Assert.Equal(_card.Id, transaction.CardId);
        Assert.NotEqual(Guid.Empty, transaction.Id);
    }

    [Fact]
    public async Task TransactionTest_ShouldNotCreateWithInvalidCard()
    {
        var payload = new CreateTransactionRequest("Test purchase", Guid.NewGuid(), 100);

        var resp = await _controller.CreateTransaction(payload);

        Assert.IsType<BadRequestObjectResult>(resp.Result);
    }

    [Fact]
    public async Task TransactionTest_ShouldGet()
    {
        var transaction = new Transaction
        {
            Description = "Test purchase",
            Date = DateTime.Now,
            AmountUsd = 250,
            CardId = _card.Id
        };
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        var resp = await _controller.GetTransaction(transaction.Id, null);
        var getResult = Assert.IsType<OkObjectResult>(resp);
        var retrieved = Assert.IsType<Transaction>(getResult.Value);
        Assert.Equal(transaction, retrieved);
    }

    [Fact]
    public async Task TransactionTest_ShouldNotGet()
    {
        var resp = await _controller.GetTransaction(Guid.Empty, null);
        Assert.IsType<NotFoundResult>(resp);
    }

    [Fact]
    public async Task TransactionTest_ShouldGetWithCurrency()
    {
        _mockExchange.SetResponse(new TreasuryRateRecord("Dollar", "1.5", "2026-01-01"));
        var transaction = new Transaction
        {
            Description = "Test Purchase Currency",
            Date = DateTime.Parse("2026-02-15"),
            AmountUsd = 250,
            CardId = _card.Id
        };
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        var resp = await _controller.GetTransaction(transaction.Id, "Dollar");
        var getResult = Assert.IsType<OkObjectResult>(resp);
        var retrieved = Assert.IsType<ConvertedTransaction>(getResult.Value);

        var expected = new ConvertedTransaction
        (
            transaction.Id,
            transaction.Description,
            transaction.Date,
            transaction.AmountUsd,
            1.5M,
            transaction.AmountUsd * 1.5M
        );

        Assert.Equal(expected, retrieved);
    }

    [Fact]
    public async Task TransactionTest_ShouldNotGetWithCurrency_NoRateAvailable()
    {
        _mockExchange.SetResponse(null);
        var transaction = new Transaction
        {
            Description = "Test Purchase No Rate",
            Date = DateTime.Parse("2026-02-15"),
            AmountUsd = 250,
            CardId = _card.Id
        };
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        var resp = await _controller.GetTransaction(transaction.Id, "FakeCurrency");
        var notFound = Assert.IsType<NotFoundObjectResult>(resp);
        Assert.Equal("The purchase cannot be converted to the target currency", notFound.Value);
    }

    [Fact]
    public async Task TransactionTest_ShouldNotGetWithCurrency_TransactionNotFound()
    {
        var resp = await _controller.GetTransaction(Guid.Empty, "Dollar");
        Assert.IsType<NotFoundResult>(resp);
    }
}
