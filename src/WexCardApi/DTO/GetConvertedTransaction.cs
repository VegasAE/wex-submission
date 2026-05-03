namespace WexCardApi.DTO;

public record ConvertedTransaction(
    Guid Id,
    string Description,
    DateTime TransactionDate,
    decimal OriginalAmount,
    decimal ExchangeRate,
    decimal ConvertedAmount
);
