using System.ComponentModel.DataAnnotations;

namespace MakeTransfer.Core.Application.DataSets;

/// <summary>
/// Data container for money transfer requests.
/// Simple data structure with no business logic acting as a data transfer object.
/// </summary>
public sealed record TransferData
{
    [Required]
    public string FromAccountId { get; init; } = string.Empty;

    [Required]
    public string ToAccountId { get; init; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; init; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; init; } = string.Empty;

    [Required]
    public string Reference { get; init; } = string.Empty;

    /// <summary>
    /// Identifies who/what initiated this transfer (user, system, channel)
    /// </summary>
    public string InitiatedBy { get; init; } = "SYSTEM";

    /// <summary>
    /// Correlation ID for tracing this operation across systems
    /// </summary>
    public string CorrelationId { get; init; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Channel that initiated the transfer (WEB, MOBILE, API, MESSAGE_QUEUE)
    /// </summary>
    public string Channel { get; init; } = "API";

    public TransferData() { }

    public TransferData(
        string fromAccountId,
        string toAccountId,
        decimal amount,
        string currency,
        string reference,
        string initiatedBy = "SYSTEM",
        string channel = "API")
    {
        FromAccountId = fromAccountId;
        ToAccountId = toAccountId;
        Amount = amount;
        Currency = currency;
        Reference = reference;
        InitiatedBy = initiatedBy;
        Channel = channel;
        CorrelationId = Guid.NewGuid().ToString("N");
    }
}