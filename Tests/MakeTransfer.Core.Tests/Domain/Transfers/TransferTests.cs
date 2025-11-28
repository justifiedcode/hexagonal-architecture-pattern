using MakeTransfer.Core.Domain.Transfers;

namespace MakeTransfer.Core.Tests.Domain.Transfers;

public class TransferTests
{
    [Fact]
    public void Constructor_ValidInputs_CreatesTransferInPendingState()
    {
        // Arrange
        var transferId = "TXN001";
        var fromAccountId = "ACC001";
        var toAccountId = "ACC002";
        var currency = "USD";
        var amount = 100m;
        var reference = "Payment for services";
        var createdAt = DateTime.UtcNow;

        // Act
        var transfer = new Transfer(transferId, fromAccountId, toAccountId, currency, amount, reference, createdAt);

        // Assert
        Assert.Equal(transferId, transfer.TransferId);
        Assert.Equal(fromAccountId, transfer.FromAccountId);
        Assert.Equal(toAccountId, transfer.ToAccountId);
        Assert.Equal(currency, transfer.Currency);
        Assert.Equal(amount, transfer.Amount);
        Assert.Equal(reference, transfer.Reference);
        Assert.Equal(createdAt, transfer.CreatedAtUtc);
        Assert.Equal(TransferStatus.Pending, transfer.Status);
        Assert.Null(transfer.FailureReason);
    }

    [Fact]
    public void MarkCompleted_PendingTransfer_UpdatesStatusAndClearsFailureReason()
    {
        // Arrange
        var transfer = CreateTestTransfer();
        transfer.MarkFailed("Some error"); // Set a failure first

        // Act
        transfer.MarkCompleted();

        // Assert
        Assert.Equal(TransferStatus.Completed, transfer.Status);
        Assert.Null(transfer.FailureReason);
    }

    [Fact]
    public void MarkFailed_PendingTransfer_UpdatesStatusAndSetsFailureReason()
    {
        // Arrange
        var transfer = CreateTestTransfer();
        var failureReason = "Insufficient funds";

        // Act
        transfer.MarkFailed(failureReason);

        // Assert
        Assert.Equal(TransferStatus.Failed, transfer.Status);
        Assert.Equal(failureReason, transfer.FailureReason);
    }

    private static Transfer CreateTestTransfer()
    {
        return new Transfer("TXN001", "ACC001", "ACC002", "USD", 100m, "Test transfer", DateTime.UtcNow);
    }
}