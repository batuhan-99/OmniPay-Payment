using System.ComponentModel.DataAnnotations;

namespace OmniPay.Application.DTOs;

public sealed class CreatePaymentRequest
{
    [Required]
    public Guid MerchantId { get; set; }

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Payment amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter ISO code.")]
    public string Currency { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
