using System.ComponentModel.DataAnnotations;

namespace WexCardApi.DTO;

public record GetExchangeRateRequest(
    [Required] string Currency,
    [Required] DateTime RecordDate

);
