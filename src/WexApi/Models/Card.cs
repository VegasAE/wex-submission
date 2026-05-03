namespace WexApi.Models;

public class Card
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal CreditLimit { get; set; }
}
