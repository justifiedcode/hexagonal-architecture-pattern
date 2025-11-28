namespace MakeTransfer.Core.Domain.Transfers;

public sealed class PaymentResult
{
    public bool Success { get; }
    public string? FailureReason { get; }
    public string? ExternalReference { get; }

    public PaymentResult(bool success, string? failureReason = null, string? externalReference = null)
    {
        Success = success;
        FailureReason = failureReason;
        ExternalReference = externalReference;
    }
}