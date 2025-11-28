using MakeTransfer.Core.Domain.Transfers;

namespace MakeTransfer.Core.Tests.Domain.Transfers;

public class PaymentResultTests
{
    [Fact]
    public void Constructor_SuccessfulPayment_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var result = new PaymentResult(true, null, "EXT123");

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.FailureReason);
        Assert.Equal("EXT123", result.ExternalReference);
    }

    [Fact]
    public void Constructor_FailedPayment_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var result = new PaymentResult(false, "Network timeout");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Network timeout", result.FailureReason);
        Assert.Null(result.ExternalReference);
    }

    [Fact]
    public void Constructor_DefaultParameters_SetsNullValues()
    {
        // Arrange & Act
        var result = new PaymentResult(true);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.FailureReason);
        Assert.Null(result.ExternalReference);
    }
}