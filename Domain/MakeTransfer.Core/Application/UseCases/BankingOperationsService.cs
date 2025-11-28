using MakeTransfer.Core.Application.Ports.Incoming;
using MakeTransfer.Core.Application.Ports.Outgoing;
using MakeTransfer.Core.Application.DataSets;
using MakeTransfer.Core.Domain.Transfers;
using MakeTransfer.Core.Domain.Accounts;
using MakeTransfer.Core.Shared;

namespace MakeTransfer.Core.Application.UseCases;

public sealed class BankingOperationsService : IBankingOperationsInputPort
{
    private readonly IDatabaseOutputPort _databasePort;
    private readonly IPaymentOutputPort _paymentPort;
    private readonly INotificationOutputPort _notificationPort;

    public BankingOperationsService(
        IDatabaseOutputPort databasePort,
        IPaymentOutputPort paymentPort,
        INotificationOutputPort notificationPort)
    {
        _databasePort = databasePort ?? throw new ArgumentNullException(nameof(databasePort));
        _paymentPort = paymentPort ?? throw new ArgumentNullException(nameof(paymentPort));
        _notificationPort = notificationPort ?? throw new ArgumentNullException(nameof(notificationPort));
    }

    public OperationResult<TransferResult> ExecuteTransfer(TransferData transferData)
    {
        try
        {
            var fromAccount = _databasePort.GetAccountById(transferData.FromAccountId);
            var toAccount = _databasePort.GetAccountById(transferData.ToAccountId);

            if (fromAccount is null || toAccount is null)
            {
                return OperationResult<TransferResult>.NotFoundResult(
                   "Source or destination account not found.",
                   transferData.CorrelationId);
            }

            var nowUtc = DateTime.UtcNow;

            if (!CanExecuteTransfer(
                fromAccount,
                toAccount,
                transferData.Amount,
                transferData.Currency,
                nowUtc,
                out var reason))
            {
                return OperationResult<TransferResult>.ValidationErrorResult(
                    $"Transfer rejected: {reason}",
                    transferData.CorrelationId);
            }

            // Build transfer object in Pending state
            var transferId = Guid.NewGuid().ToString("N");
            var transfer = new Transfer(
                transferId,
                transferData.FromAccountId,
                transferData.ToAccountId,
                transferData.Currency,
                transferData.Amount,
                transferData.Reference,
                nowUtc);

            var today = DateOnly.FromDateTime(nowUtc.Date);

            // Apply domain state changes
            fromAccount.Debit(transferData.Amount, today);
            toAccount.Credit(transferData.Amount);

            // Persist all changes atomically through single database port
            if (!_databasePort.ExecuteTransfer(fromAccount, toAccount, transfer))
            {
                return OperationResult<TransferResult>.FailureResult(
                    500,
                    "Could not persist transfer and account updates.",
                    transferData.CorrelationId);
            }

            // Call external payment gateway
            var paymentResult = _paymentPort.ExecuteTransfer(transfer);

            if (!paymentResult.Success)
            {
                transfer.MarkFailed(paymentResult.FailureReason ?? "Payment gateway failure.");
                _databasePort.ExecuteTransfer(fromAccount, toAccount, transfer);

                _notificationPort.NotifyTransfer(
                    transfer,
                    $"Transfer {transfer.TransferId} failed: {transfer.FailureReason}");

                return OperationResult<TransferResult>.FailureResult(
                    502,
                    $"Transfer failed: {transfer.FailureReason}",
                    transferData.CorrelationId);
            }

            transfer.MarkCompleted();
            _databasePort.ExecuteTransfer(fromAccount, toAccount, transfer);

            _notificationPort.NotifyTransfer(
                transfer,
                $"Transfer {transfer.TransferId} completed successfully.");

            var result = new TransferResult(
                transfer.TransferId,
                transfer.Status,
                "Transfer completed successfully.",
                nowUtc,
                paymentResult.ExternalReference);

            return OperationResult<TransferResult>.SuccessResult(
                result,
                "Transfer completed successfully.",
                transferData.CorrelationId);
        }
        catch (Exception ex)
        {
            return OperationResult<TransferResult>.FailureResult(
                500,
                $"Unexpected error during transfer: {ex.Message}",
                transferData.CorrelationId);
        }
    }

    /// <summary>
    /// Validates if a transfer can be executed based on business rules.
    /// </summary>
    private static bool CanExecuteTransfer(
        Account from,
        Account to,
        decimal amount,
        string currency,
        DateTime nowUtc,
        out string failureReason)
    {
        failureReason = string.Empty;

        if (!from.IsSameCurrency(currency) || !to.IsSameCurrency(currency))
        {
            failureReason = "Currency mismatch.";
            return false;
        }

        if (!from.HasSufficientBalance(amount))
        {
            failureReason = "Insufficient funds.";
            return false;
        }

        var today = DateOnly.FromDateTime(nowUtc.Date);

        if (!from.IsWithinDailyLimit(amount, today))
        {
            failureReason = "Daily transfer limit exceeded.";
            return false;
        }

        if (amount <= 0)
        {
            failureReason = "Transfer amount must be positive.";
            return false;
        }

        if (from.AccountId == to.AccountId)
        {
            failureReason = "Source and destination accounts must be different.";
            return false;
        }

        return true;
    }
}