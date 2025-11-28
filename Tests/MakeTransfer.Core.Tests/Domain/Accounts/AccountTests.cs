using MakeTransfer.Core.Domain.Accounts;

namespace MakeTransfer.Core.Tests.Domain.Accounts;

public class AccountTests
{
    [Fact]
    public void Constructor_ValidInputs_CreatesAccount()
    {
        // Arrange & Act
        var account = new Account("ACC001", "John Doe", "USD", 1000m, 500m);

        // Assert
        Assert.Equal("ACC001", account.AccountId);
        Assert.Equal("John Doe", account.OwnerName);
        Assert.Equal("USD", account.Currency);
        Assert.Equal(1000m, account.Balance);
        Assert.Equal(500m, account.DailyDebitLimit);
        Assert.Equal(0m, account.DailyDebitedAmount);
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow.Date), account.DailyLimitDate);
    }

    [Fact]
    public void Constructor_NullAccountId_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Account(null!, "John Doe", "USD", 1000m, 500m));
    }

    [Theory]
    [InlineData(1000, 500, true)]
    [InlineData(1000, 1000, true)]
    [InlineData(1000, 1001, false)]
    public void HasSufficientBalance_VariousAmounts_ReturnsExpected(decimal balance, decimal amount, bool expected)
    {
        // Arrange
        var account = new Account("ACC001", "John Doe", "USD", balance, 500m);

        // Act & Assert
        Assert.Equal(expected, account.HasSufficientBalance(amount));
    }

    [Theory]
    [InlineData("USD", "USD", true)]
    [InlineData("USD", "usd", true)]
    [InlineData("USD", "EUR", false)]
    public void IsSameCurrency_VariousCurrencies_ReturnsExpected(string accountCurrency, string testCurrency, bool expected)
    {
        // Arrange
        var account = new Account("ACC001", "John Doe", accountCurrency, 1000m, 500m);

        // Act & Assert
        Assert.Equal(expected, account.IsSameCurrency(testCurrency));
    }

    [Fact]
    public void IsWithinDailyLimit_SameDay_ChecksAgainstCurrentAmount()
    {
        // Arrange
        var account = new Account("ACC001", "John Doe", "USD", 1000m, 500m);
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        // Act & Assert
        Assert.True(account.IsWithinDailyLimit(300m, today));
        Assert.False(account.IsWithinDailyLimit(600m, today));
    }

    [Fact]
    public void IsWithinDailyLimit_NewDay_ResetsCounter()
    {
        // Arrange
        var account = new Account("ACC001", "John Doe", "USD", 1000m, 500m);
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var tomorrow = today.AddDays(1);

        // Simulate some usage today
        account.Debit(300m, today);

        // Act & Assert - should reset for new day
        Assert.True(account.IsWithinDailyLimit(400m, tomorrow));
    }

    [Fact]
    public void Debit_ValidAmount_UpdatesBalanceAndDailyAmount()
    {
        // Arrange
        var account = new Account("ACC001", "John Doe", "USD", 1000m, 500m);
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        // Act
        account.Debit(200m, today);

        // Assert
        Assert.Equal(800m, account.Balance);
        Assert.Equal(200m, account.DailyDebitedAmount);
    }

    [Fact]
    public void Debit_InsufficientFunds_ThrowsInvalidOperationException()
    {
        // Arrange
        var account = new Account("ACC001", "John Doe", "USD", 100m, 500m);
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => account.Debit(200m, today));
        Assert.Equal("Insufficient funds.", exception.Message);
    }

    [Fact]
    public void Debit_ExceedsDailyLimit_ThrowsInvalidOperationException()
    {
        // Arrange
        var account = new Account("ACC001", "John Doe", "USD", 1000m, 300m);
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => account.Debit(400m, today));
        Assert.Equal("Daily debit limit exceeded.", exception.Message);
    }

    [Fact]
    public void Credit_ValidAmount_UpdatesBalance()
    {
        // Arrange
        var account = new Account("ACC001", "John Doe", "USD", 1000m, 500m);

        // Act
        account.Credit(200m);

        // Assert
        Assert.Equal(1200m, account.Balance);
    }
}