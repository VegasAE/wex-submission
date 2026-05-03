using System.ComponentModel.DataAnnotations;

namespace WexApi.DTO;

public record CreateTransactionRequest(
    [Required]
    string Description,
    Guid CardId,

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    decimal AmountUsd

);
