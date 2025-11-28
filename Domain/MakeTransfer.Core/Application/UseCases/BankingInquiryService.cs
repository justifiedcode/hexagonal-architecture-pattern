using MakeTransfer.Core.Application.Ports.Incoming;
using MakeTransfer.Core.Application.Ports.Outgoing;
using MakeTransfer.Core.Application.DataSets;
using MakeTransfer.Core.Shared;

namespace MakeTransfer.Core.Application.UseCases;

public sealed class BankingInquiryService : IBankingInquiryInputPort
{
    private readonly IDatabaseOutputPort _databasePort;

    public BankingInquiryService(IDatabaseOutputPort databasePort)
    {
        _databasePort = databasePort ?? throw new ArgumentNullException(nameof(databasePort));
    }

    public OperationResult<AccountDetails> GetAccountDetails(string accountId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                return OperationResult<AccountDetails>.ValidationErrorResult(
                 "Account ID is required.");
            }

            var account = _databasePort.GetAccountById(accountId);

            if (account is null)
            {
                return OperationResult<AccountDetails>.NotFoundResult(
                 $"Account {accountId} not found.");
            }

            var accountDetails = new AccountDetails(
                account.AccountId,
                account.OwnerName,
                account.Currency,
                account.Balance,
                account.DailyDebitLimit,
                account.DailyDebitedAmount,
                account.DailyLimitDate,
                DateTime.UtcNow);

            return OperationResult<AccountDetails>.SuccessResult(
                accountDetails,
                "Account details retrieved successfully.");
        }
        catch (Exception ex)
        {
            return OperationResult<AccountDetails>.FailureResult(
                500,
                $"Error retrieving account details: {ex.Message}");
        }
    }

    public OperationResult<AccountBalance> GetAccountBalance(string accountId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                return OperationResult<AccountBalance>.ValidationErrorResult(
                    "Account ID is required.");
            }

            var account = _databasePort.GetAccountById(accountId);

            if (account is null)
            {
                return OperationResult<AccountBalance>.NotFoundResult(
                    $"Account {accountId} not found.");
            }

            // For simplicity, available = current balance
            // In a real system, available might be current - holds/pending
            var accountBalance = new AccountBalance(
                account.AccountId,
                account.Balance, // Available
                account.Balance, // Current
                account.Currency,
                DateTime.UtcNow);

            return OperationResult<AccountBalance>.SuccessResult(
                accountBalance,
                "Account balance retrieved successfully.");
        }
        catch (Exception ex)
        {
            return OperationResult<AccountBalance>.FailureResult(
                500,
                $"Error retrieving account balance: {ex.Message}");
        }
    }
}