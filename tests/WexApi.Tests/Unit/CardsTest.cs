
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using WexApi.Controllers;
using WexApi.Models;
using WexApi.DB;
using WexApi.DTO;

public class CardTest
{

    private readonly AppDbContext _db;
    private readonly CardsController _controller;

    public CardTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);
        _controller = new CardsController(_db);
    }


    [Fact]
    public async Task CardTest_ShouldCreate()
    {
        var payload = new CreateCardRequest(500);

        var resp = await _controller.CreateCard(payload);

        var createdResult = Assert.IsType<CreatedResult>(resp.Result);
        var card = Assert.IsType<Card>(createdResult.Value);
        Assert.Equal(500, card.CreditLimit);
        Assert.NotEqual(Guid.Empty, card.Id);
    }

    [Fact]
    public async Task CardTest_ShouldGet()
    {
        var card = new Card { CreditLimit = 1000 };
        _db.Cards.Add(card);
        await _db.SaveChangesAsync();

        var resp = await _controller.GetCard(card.Id);
        var getResult = Assert.IsType<OkObjectResult>(resp.Result);
        var retreived = Assert.IsType<Card>(getResult.Value);
        Assert.Equal(card, retreived);
    }

    [Fact]
    public async Task CardTest_ShouldNotGet()
    {
        var resp = await _controller.GetCard(Guid.Empty);
        var getResult = Assert.IsType<NotFoundResult>(resp.Result);
    }
}
