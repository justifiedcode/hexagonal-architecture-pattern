namespace MakeTransfer.Core.Domain.Accounts;

public sealed class Account
{
    public string AccountId { get; }
    public string OwnerName { get; }
    public string Currency { get; }

    public decimal Balance { get; private set; }

    // Very simple daily-limit tracking just for the example
    public DateOnly DailyLimitDate { get; private set; }
    public decimal DailyDebitedAmount { get; private set; }

    public decimal DailyDebitLimit { get; }

    public Account(
        string accountId,
        string ownerName,
        string currency,
        decimal openingBalance,
        decimal dailyDebitLimit)
    {
        AccountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
        OwnerName = ownerName ?? throw new ArgumentNullException(nameof(ownerName));
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));

        Balance = openingBalance;
        DailyDebitLimit = dailyDebitLimit;

        DailyLimitDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        DailyDebitedAmount = 0m;
    }

    public bool HasSufficientBalance(decimal amount) => Balance >= amount;

    public bool IsSameCurrency(string currency) => string.Equals(Currency, currency, StringComparison.OrdinalIgnoreCase);

    public bool IsWithinDailyLimit(decimal amount, DateOnly today)
    {
        if (today != DailyLimitDate)
        {
            // new day; reset counter
            DailyLimitDate = today;
            DailyDebitedAmount = 0m;
        }

        return (DailyDebitedAmount + amount) <= DailyDebitLimit;
    }

    public void Debit(decimal amount, DateOnly today)
    {
        if (!HasSufficientBalance(amount))
            throw new InvalidOperationException("Insufficient funds.");

        if (!IsWithinDailyLimit(amount, today))
            throw new InvalidOperationException("Daily debit limit exceeded.");

        Balance -= amount;
        DailyDebitedAmount += amount;
    }

    public void Credit(decimal amount)
    {
        Balance += amount;
    }
}
