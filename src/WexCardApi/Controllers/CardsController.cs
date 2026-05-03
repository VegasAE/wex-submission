using Microsoft.AspNetCore.Mvc;
using WexCardApi.DB;
using WexCardApi.Models;
using WexCardApi.DTO;

namespace WexCardApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardsController : ControllerBase
{
    private readonly AppDbContext _db;

    public CardsController(AppDbContext _db)
    {
        this._db = _db;
    }

    /*
     * Post request creates new card and returns Id for later use
    */
    // POST: api/cards
    [HttpPost]
    public async Task<ActionResult<Card>> CreateCard([FromBody] CreateCardRequest payload)
    {
        Card newCard = new Card();
        newCard.CreditLimit = payload.CreditLimit;
        _db.Cards.Add(newCard);
        await _db.SaveChangesAsync();

        return Created($"/api/cards/{newCard.Id}", newCard);
    }

    /*
     * Get request returns the requested card based on Id
    */
    // GET: /api/cards/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Card>> GetCard(Guid id)
    {
        Card? card = await _db.Cards.FindAsync(id);

        if (card == null)
            return NotFound();

        return Ok(card);
    }

}
