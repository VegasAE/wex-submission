namespace WexCardApi.Models;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Description { get; set; }
    public DateTime Date { get; set; }
    public decimal AmountUsd { get; set; }
    public Guid CardId { get; set; }
    public Card Card { get; set; } = null!;
}
