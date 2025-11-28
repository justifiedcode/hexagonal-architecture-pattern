using Moq;
using MakeTransfer.Core.Application.Ports.Incoming;
using MakeTransfer.Core.Application.Ports.Outgoing;
using MakeTransfer.Core.Application.DataSets;
using MakeTransfer.Core.Domain.Accounts;
using MakeTransfer.Core.Domain.Transfers;
using MakeTransfer.Core.Application.UseCases;

namespace MakeTransfer.Core.Tests.Application.UseCases;

public class BankingOperationsServiceTests
{
    private readonly Mock<IDatabaseOutputPort> _databasePortMock;
    private readonly Mock<IPaymentOutputPort> _paymentPortMock;
    private readonly Mock<INotificationOutputPort> _notificationPortMock;
    private readonly IBankingOperationsInputPort _bankingOperations;

    public BankingOperationsServiceTests()
    {
        _databasePortMock = new Mock<IDatabaseOutputPort>();
        _paymentPortMock = new Mock<IPaymentOutputPort>();
        _notificationPortMock = new Mock<INotificationOutputPort>();

        _bankingOperations = new BankingOperationsService(
            _databasePortMock.Object,
            _paymentPortMock.Object,
            _notificationPortMock.Object);
    }

    [Fact]
    public void ExecuteTransfer_SuccessfulTransfer_ReturnsSuccessResult()
    {
        // Arrange
        var fromAccount = new Account("ACC001", "John Doe", "USD", 1000m, 500m);
        var toAccount = new Account("ACC002", "Jane Smith", "USD", 0m, 500m);
        var transferData = new TransferData("ACC001", "ACC002", 200m, "USD", "Payment for services");

        _databasePortMock.Setup(x => x.GetAccountById("ACC001")).Returns(fromAccount);
        _databasePortMock.Setup(x => x.GetAccountById("ACC002")).Returns(toAccount);
        _databasePortMock.Setup(x => x.ExecuteTransfer(It.IsAny<Account>(), It.IsAny<Account>(), It.IsAny<Transfer>())).Returns(true);
        _paymentPortMock.Setup(x => x.ExecuteTransfer(It.IsAny<Transfer>()))
            .Returns(new PaymentResult(true, null, "EXT123"));

        // Act
        var result = _bankingOperations.ExecuteTransfer(transferData);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("Transfer completed successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(transferData.CorrelationId, result.CorrelationId);

        // Verify account balances were updated
        Assert.Equal(800m, fromAccount.Balance);
        Assert.Equal(200m, toAccount.Balance);
    }

    [Fact]
    public void ExecuteTransfer_AccountNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var transferData = new TransferData("ACC001", "ACC002", 200m, "USD", "Payment");
        _databasePortMock.Setup(x => x.GetAccountById("ACC001")).Returns((Account?)null);

        // Act
        var result = _bankingOperations.ExecuteTransfer(transferData);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("not found", result.Message);
        Assert.Equal(transferData.CorrelationId, result.CorrelationId);
    }

    [Fact]
    public void ExecuteTransfer_InsufficientFunds_ReturnsValidationError()
    {
        // Arrange
        var fromAccount = new Account("ACC001", "John Doe", "USD", 100m, 500m);
        var toAccount = new Account("ACC002", "Jane Smith", "USD", 0m, 500m);
        var transferData = new TransferData("ACC001", "ACC002", 200m, "USD", "Payment");

        _databasePortMock.Setup(x => x.GetAccountById("ACC001")).Returns(fromAccount);
        _databasePortMock.Setup(x => x.GetAccountById("ACC002")).Returns(toAccount);

        // Act
        var result = _bankingOperations.ExecuteTransfer(transferData);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Insufficient funds", result.Message);
    }

    [Fact]
    public void ExecuteTransfer_PaymentGatewayFails_ReturnsFailureResult()
    {
        // Arrange
        var fromAccount = new Account("ACC001", "John Doe", "USD", 1000m, 500m);
        var toAccount = new Account("ACC002", "Jane Smith", "USD", 0m, 500m);
        var transferData = new TransferData("ACC001", "ACC002", 200m, "USD", "Payment");

        _databasePortMock.Setup(x => x.GetAccountById("ACC001")).Returns(fromAccount);
        _databasePortMock.Setup(x => x.GetAccountById("ACC002")).Returns(toAccount);
        _databasePortMock.Setup(x => x.ExecuteTransfer(It.IsAny<Account>(), It.IsAny<Account>(), It.IsAny<Transfer>())).Returns(true);
        _paymentPortMock.Setup(x => x.ExecuteTransfer(It.IsAny<Transfer>()))
            .Returns(new PaymentResult(false, "External system unavailable"));

        // Act
        var result = _bankingOperations.ExecuteTransfer(transferData);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(502, result.StatusCode);
        Assert.Contains("External system unavailable", result.Message);
    }
}