using System.ComponentModel.DataAnnotations;

namespace WexCardApi.DTO;

public record CreateCardRequest(
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Credit limit must be greater than zero.")]
    decimal CreditLimit
);
