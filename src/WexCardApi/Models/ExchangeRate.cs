public class ExchangeRate
{
    public decimal Rate { get; set; }
    public required string Currency { get; set; }
    public DateOnly RecordDate { get; set; }
}
