using System.ComponentModel.DataAnnotations;

namespace WexApi.DTO;

public record GetExchangeRateRequest(
    [Required] string Currency,
    [Required] DateTime RecordDate

);
