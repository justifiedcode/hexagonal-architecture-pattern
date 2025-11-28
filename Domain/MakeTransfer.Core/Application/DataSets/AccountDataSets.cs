using MakeTransfer.Core.Domain.Transfers;

namespace MakeTransfer.Core.Application.DataSets;

/// <summary>
/// Data container representing the outcome of a transfer operation.
/// </summary>
public sealed record TransferResult(
    string TransferId,
    TransferStatus Status,
    string Message,
    DateTime ProcessedAt,
    string? ExternalReference = null
);

/// <summary>
/// Data container for account details in inquiry operations.
/// </summary>
public sealed record AccountDetails(
    string AccountId,
    string OwnerName,
    string Currency,
    decimal Balance,
    decimal DailyDebitLimit,
    decimal DailyDebitedAmount,
    DateOnly DailyLimitDate,
    DateTime LastUpdated
);

/// <summary>
/// Data container for account balance information.
/// </summary>
public sealed record AccountBalance(
    string AccountId,
    decimal Available,
    decimal Current,
    string Currency,
    DateTime AsOf
);