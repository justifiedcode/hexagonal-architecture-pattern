using MakeTransfer.Core.Domain.Accounts;
using MakeTransfer.Core.Domain.Transfers;

namespace MakeTransfer.Core.Application.Ports.Outgoing;

/// <summary>
/// Database output port that represents persistence capabilities for the money transfer domain.
/// </summary>
public interface IDatabaseOutputPort
{
    /// <summary>
    /// Retrieves an account by its identifier.
    /// </summary>
    /// <param name="accountId">The unique account identifier</param>
    /// <returns>The account if found, null otherwise</returns>
    Account? GetAccountById(string accountId);

    /// <summary>
    /// Executes a complete transfer operation atomically.
    /// This includes updating both accounts and persisting the transfer record.
    /// </summary>
    /// <param name="fromAccount">The source account (already debited)</param>
    /// <param name="toAccount">The destination account (already credited)</param>
    /// <param name="transfer">The transfer record to persist</param>
    /// <returns>True if the operation succeeded, false otherwise</returns>
    bool ExecuteTransfer(Account fromAccount, Account toAccount, Transfer transfer);
}