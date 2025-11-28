using MakeTransfer.Core.Application.Ports.Incoming;
using MakeTransfer.Core.Application.Ports.Outgoing;
using MakeTransfer.Core.Application.UseCases;
using MakeTransfer.Core.Domain.Accounts;
using Moq;

namespace MakeTransfer.Core.Tests.Application.UseCases;

public class BankingInquiryServiceTests
{
    private readonly Mock<IDatabaseOutputPort> _databasePortMock;
    private readonly IBankingInquiryInputPort _bankingInquiry;

  public BankingInquiryServiceTests()
  {
     _databasePortMock = new Mock<IDatabaseOutputPort>();
   _bankingInquiry = new BankingInquiryService(_databasePortMock.Object);
    }

    [Fact]
    public void GetAccountDetails_ValidAccount_ReturnsAccountDetails()
    {
    // Arrange
        var account = new Account("ACC001", "John Doe", "USD", 1000m, 500m);
     _databasePortMock.Setup(x => x.GetAccountById("ACC001")).Returns(account);

    // Act
        var result = _bankingInquiry.GetAccountDetails("ACC001");

        // Assert
  Assert.True(result.Success);
   Assert.Equal(200, result.StatusCode);
     Assert.NotNull(result.Data);
     Assert.Equal("ACC001", result.Data.AccountId);
        Assert.Equal("John Doe", result.Data.OwnerName);
    Assert.Equal(1000m, result.Data.Balance);
    }

    [Fact]
    public void GetAccountDetails_AccountNotFound_ReturnsNotFound()
 {
// Arrange
        _databasePortMock.Setup(x => x.GetAccountById("ACC999")).Returns((Account?)null);

     // Act
 var result = _bankingInquiry.GetAccountDetails("ACC999");

  // Assert
        Assert.False(result.Success);
 Assert.Equal(404, result.StatusCode);
   Assert.Contains("not found", result.Message);
    }

    [Fact]
    public void GetAccountBalance_ValidAccount_ReturnsBalance()
    {
    // Arrange
        var account = new Account("ACC001", "John Doe", "USD", 1000m, 500m);
      _databasePortMock.Setup(x => x.GetAccountById("ACC001")).Returns(account);

   // Act
 var result = _bankingInquiry.GetAccountBalance("ACC001");

   // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
   Assert.NotNull(result.Data);
        Assert.Equal("ACC001", result.Data.AccountId);
      Assert.Equal(1000m, result.Data.Available);
  Assert.Equal(1000m, result.Data.Current);
   Assert.Equal("USD", result.Data.Currency);
    }

 [Theory]
    [InlineData(null)]
[InlineData("")]
    [InlineData("   ")]
    public void GetAccountDetails_InvalidAccountId_ReturnsValidationError(string accountId)
    {
  // Act
        var result = _bankingInquiry.GetAccountDetails(accountId);

   // Assert
        Assert.False(result.Success);
 Assert.Equal(400, result.StatusCode);
 Assert.Contains("required", result.Message);
    }
}