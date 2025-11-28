using MakeTransfer.Core.Application.Ports.Outgoing;
using MakeTransfer.Core.Domain.Accounts;
using MakeTransfer.Core.Domain.Transfers;

namespace MakeTransfer.Adapters.Database;

/// <summary>
/// In-memory database implementation for demo purposes.
/// In a real app, this would connect to SQL Server, PostgreSQL, MongoDB, etc.
/// </summary>
public sealed class InMemoryDatabaseAdapter : IDatabaseOutputPort
{
    private static readonly Dictionary<string, Account> _accounts = new()
    {
        ["ACC001"] = new Account("ACC001", "John Doe", "USD", 5000m, 1000m),
        ["ACC002"] = new Account("ACC002", "Jane Smith", "USD", 2500m, 500m),
        ["ACC003"] = new Account("ACC003", "Bob Johnson", "EUR", 3000m, 750m),
        ["ACC004"] = new Account("ACC004", "Alice Brown", "USD", 10000m, 2000m),
        ["ACC005"] = new Account("ACC005", "Charlie Wilson", "GBP", 4500m, 800m)
    };

    private static readonly Dictionary<string, Transfer> _transfers = new();
    private static readonly object _lockObject = new();

    public Account? GetAccountById(string accountId)
    {
        lock (_lockObject)
        {
            Console.WriteLine($"Looking up account: {accountId}");
            var found = _accounts.TryGetValue(accountId, out var account);
            Console.WriteLine($"Account {accountId} {(found ? "found" : "not found")}");
            return account;
        }
    }

    public bool ExecuteTransfer(Account fromAccount, Account toAccount, Transfer transfer)
    {
        lock (_lockObject)
        {
            try
            {
                Console.WriteLine($"Starting transfer {transfer.TransferId}");

                // Simulate atomic database transaction
                Console.WriteLine($"Updating account balances:");
                Console.WriteLine($"   {fromAccount.AccountId}: Balance = {fromAccount.Balance:C}");
                Console.WriteLine($"   {toAccount.AccountId}: Balance = {toAccount.Balance:C}");

                // Update accounts in "database"
                _accounts[fromAccount.AccountId] = fromAccount;
                _accounts[toAccount.AccountId] = toAccount;

                // Save transfer record
                _transfers[transfer.TransferId] = transfer;

                Console.WriteLine($"Transfer {transfer.TransferId} saved with status {transfer.Status}");
                Console.WriteLine($"Transaction committed successfully");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Transaction failed for transfer {transfer.TransferId}: {ex.Message}");
                Console.WriteLine($"Rolling back transaction...");
                return false;
            }
        }
    }

}