using MakeTransfer.Core.Application.Ports.Outgoing;
using MakeTransfer.Core.Domain.Transfers;

namespace MakeTransfer.Adapters.Payment;

/// <summary>
/// Simple payment gateway simulation for demo purposes.
/// In production, this would connect to Stripe, PayPal, etc.
/// </summary>
public sealed class PaymentGatewayAdapter : IPaymentOutputPort
{
    private static readonly Random _random = new();

    public PaymentResult ExecuteTransfer(Transfer transfer)
    {
        Console.WriteLine($"Processing payment: {transfer.Amount:C} {transfer.Currency}");
        Console.WriteLine($"From {transfer.FromAccountId} to {transfer.ToAccountId}");

        // Simulate processing time
        var processingTimeMs = _random.Next(100, 800);
        Thread.Sleep(processingTimeMs);

        // Simple success/failure logic - 90% success rate
        var success = _random.NextDouble() > 0.1;

        if (success)
        {
            var externalRef = GenerateReference();
            Console.WriteLine($"? Payment successful - Ref: {externalRef}");
            return new PaymentResult(true, null, externalRef);
        }
        else
        {
            var reason = "Payment gateway temporarily unavailable";
            Console.WriteLine($"? Payment failed - {reason}");
            return new PaymentResult(false, reason);
        }
    }

    private static string GenerateReference()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var suffix = _random.Next(1000, 9999);
        return $"PAY_{timestamp}_{suffix}";
    }
}