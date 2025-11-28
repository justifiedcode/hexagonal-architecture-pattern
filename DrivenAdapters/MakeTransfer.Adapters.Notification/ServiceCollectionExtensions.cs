using Microsoft.Extensions.DependencyInjection;
using MakeTransfer.Core.Application.Ports.Outgoing;

namespace MakeTransfer.Adapters.Notification;

/// <summary>
/// Extension methods for registering notification services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the console notification adapter with default settings
    /// </summary>
    public static IServiceCollection AddConsoleNotificationAdapter(this IServiceCollection services)
    {
        services.AddSingleton<INotificationOutputPort, ConsoleNotificationAdapter>();
        return services;
    }
}