namespace MakeTransfer.Api.Models.Responses;

/// <summary>
/// HTTP response model for transfer operations.
/// This is the external representation returned to clients.
/// </summary>
public sealed class TransferResponse
{
    public string TransferId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
    public string? ExternalReference { get; set; }
}

/// <summary>
/// HTTP response model for account details.
/// </summary>
public sealed class AccountResponse
{
    public string AccountId { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal DailyDebitLimit { get; set; }
    public decimal DailyDebitedAmount { get; set; }
    public string DailyLimitDate { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// HTTP response model for account balance inquiries.
/// </summary>
public sealed class BalanceResponse
{
    public string AccountId { get; set; } = string.Empty;
    public decimal Available { get; set; }
    public decimal Current { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime AsOf { get; set; }
}