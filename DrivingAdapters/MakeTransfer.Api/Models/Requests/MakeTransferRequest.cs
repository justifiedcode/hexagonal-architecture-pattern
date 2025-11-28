using System.ComponentModel.DataAnnotations;

namespace MakeTransfer.Api.Models.Requests;

/// <summary>
/// External DTO for money transfer requests from HTTP clients.
/// This represents the "external interface" that gets adapted to internal domain DataSets.
/// </summary>
public sealed class MakeTransferRequest
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string FromAccountId { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string ToAccountId { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = string.Empty;

    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Reference { get; set; } = string.Empty;
}