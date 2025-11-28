using MakeTransfer.Core.Application.DataSets;
using MakeTransfer.Core.Shared;

namespace MakeTransfer.Core.Application.Ports.Incoming;

/// <summary>
/// Input port for banking inquiry operations (read-only).
/// This port provides access to account information and account balance.
/// </summary>
public interface IBankingInquiryInputPort
{
    /// <summary>
    /// Retrieves account details by account identifier.
    /// </summary>
    /// <param name="accountId">The account identifier</param>
    /// <returns>Account details if found</returns>
    OperationResult<AccountDetails> GetAccountDetails(string accountId);
    
    /// <summary>
    /// Gets the current balance for an account.
    /// </summary>
    /// <param name="accountId">The account identifier</param>
    /// <returns>Current account balance</returns>
    OperationResult<AccountBalance> GetAccountBalance(string accountId);
}