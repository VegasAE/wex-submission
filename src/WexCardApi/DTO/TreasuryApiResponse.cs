using System.Text.Json.Serialization;

namespace WexCardApi.DTO;

public record TreasuryApiResponse(
    [property: JsonPropertyName("data")]
    List<TreasuryRateRecord> Data
);

public record TreasuryRateRecord(
    [property: JsonPropertyName("currency")]
    string Currency,

    [property: JsonPropertyName("exchange_rate")]
    string ExchangeRate,

    [property: JsonPropertyName("record_date")]
    string RecordDate
);
