namespace MakeTransfer.Core.Domain.Transfers;

public sealed class Transfer
{
    public string TransferId { get; }
    public string FromAccountId { get; }
    public string ToAccountId { get; }
    public string Currency { get; }
    public decimal Amount { get; }
    public string Reference { get; }
    public DateTime CreatedAtUtc { get; }
    public TransferStatus Status { get; private set; }
    public string? FailureReason { get; private set; }

    public Transfer(
        string transferId,
        string fromAccountId,
        string toAccountId,
        string currency,
        decimal amount,
        string reference,
        DateTime createdAtUtc)
    {
        TransferId = transferId;
        FromAccountId = fromAccountId;
        ToAccountId = toAccountId;
        Currency = currency;
        Amount = amount;
        Reference = reference;
        CreatedAtUtc = createdAtUtc;
        Status = TransferStatus.Pending;
    }

    public void MarkCompleted()
    {
        Status = TransferStatus.Completed;
        FailureReason = null;
    }

    public void MarkFailed(string reason)
    {
        Status = TransferStatus.Failed;
        FailureReason = reason;
    }
}