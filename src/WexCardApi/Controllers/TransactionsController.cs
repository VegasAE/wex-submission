using Microsoft.AspNetCore.Mvc;

using WexCardApi.DB;
using WexCardApi.DTO;
using WexCardApi.Models;

namespace WexCardApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TransactionsController(AppDbContext _db)
    {
        this._db = _db;
    }

    /*
     * Post request creates new card and returns Id for later use
    */
    // POST: /api/transactions
    [HttpPost]
    public async Task<ActionResult<Transaction>> CreateTransaction([FromBody] CreateTransactionRequest payload)
    {
        // Validate card id
        Card? resp = await _db.Cards.FindAsync(payload.CardId);

        if (resp == null)
            return BadRequest();

        Transaction newTransaction = new Transaction{
            Description = payload.Description,
            Date = DateTime.Now, // TODO: Check this needs to be changed
            AmountUsd = payload.AmountUsd,
            CardId = payload.CardId  
        };

        _db.Transactions.Add(newTransaction);
        await _db.SaveChangesAsync();

        return Created($"/api/transactions/{newTransaction.Id}", newTransaction);
    }

    /*
     * Get request returns the requested transaction based on Id
    */
    // GET: /api/transactions/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Transaction>> GetTransaction(Guid id)
    {
        Transaction? transaction = await _db.Transactions.FindAsync(id);

        if (transaction == null)
            return NotFound();

        return Ok(transaction);
    }
}
