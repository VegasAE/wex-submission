
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using WexCardApi.Controllers;
using WexCardApi.Models;
using WexCardApi.DB;
using WexCardApi.DTO;

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
    public async Task CardTest_ShouldNotCreate()
    {
        var payload = new CreateCardRequest(-68);
        var resp = await _controller.CreateCard(payload);

        Assert.IsType<BadRequestResult>(resp.Result);
    }
}
