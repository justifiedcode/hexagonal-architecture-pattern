using MakeTransfer.Core.Domain.Transfers;

namespace MakeTransfer.Core.Application.Ports.Outgoing;

/// <summary>
/// Output port for processing payments through external payment gateways.
/// Implementations handle the actual payment processing via Stripe, PayPal, etc.
/// </summary>
public interface IPaymentOutputPort
{
    PaymentResult ExecuteTransfer(Transfer transfer);
}