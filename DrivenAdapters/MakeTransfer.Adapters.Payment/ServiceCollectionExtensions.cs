using Microsoft.Extensions.DependencyInjection;
using MakeTransfer.Core.Application.Ports.Outgoing;

namespace MakeTransfer.Adapters.Payment;

/// <summary>
/// Extension methods for registering payment gateway services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the payment gateway adapter
    /// </summary>
    public static IServiceCollection AddPaymentGatewayAdapter(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentOutputPort, PaymentGatewayAdapter>();
        return services;
    }
}