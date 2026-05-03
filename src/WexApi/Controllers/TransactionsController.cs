using Microsoft.AspNetCore.Mvc;

using WexApi.DB;
using WexApi.DTO;
using WexApi.Models;
using WexApi.Services;

namespace WexApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITreasuryClient _exchange;

    public TransactionsController(AppDbContext db, ITreasuryClient exchange)
    {
        _db = db;
        _exchange = exchange;
    }

    /*
     * Post request creates new transaction and returns Id for later use
    */
    // POST: /api/transactions
    [HttpPost]
    public async Task<ActionResult<Transaction>> CreateTransaction([FromBody] CreateTransactionRequest payload)
    {
        // Validate card id
        Card? resp = await _db.Cards.FindAsync(payload.CardId);

        if (resp == null)
            return BadRequest("Bad Request");

        Transaction newTransaction = new Transaction
        {
            Description = payload.Description,
            Date = DateTime.UtcNow,
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
    // GET: /api/transactions/{id}?currency={currency}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetTransaction(Guid id, [FromQuery] string? currency)
    {
        Transaction? transaction = await _db.Transactions.FindAsync(id);

        if (transaction == null)
            return NotFound();

        if (currency == null)
            return Ok(transaction);

        // Check treasury client for exchange rate
        GetExchangeRateRequest req = new GetExchangeRateRequest(currency, transaction.Date);

        var record = await _exchange.GetExchangeRate(req);

        if (record == null)
            return NotFound("The purchase cannot be converted to the target currency");

        // Try to parse decimal and return error if fails
        if (!decimal.TryParse(record.ExchangeRate, out decimal rate))
            return StatusCode(502, "Invalid exchange rate received from Treasury API");


        // Build new transaction return
        ConvertedTransaction convTransaction = new ConvertedTransaction(
            transaction.Id,
            transaction.Description,
            transaction.Date,
            transaction.AmountUsd,
            rate,
            transaction.AmountUsd * rate
        );

        return Ok(convTransaction);
    }
}
