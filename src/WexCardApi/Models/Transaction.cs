namespace WexCardApi.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public required string Description { get; set; }
    public DateTime Date { get; set; }
    public decimal AmountUsd { get; set; }
    public Guid CardId { get; set; }
}
