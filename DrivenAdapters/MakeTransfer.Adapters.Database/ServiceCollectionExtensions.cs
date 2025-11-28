using Microsoft.Extensions.DependencyInjection;
using MakeTransfer.Core.Application.Ports.Outgoing;

namespace MakeTransfer.Adapters.Database;

/// <summary>
/// Extension methods for registering database adapter services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the in-memory database adapter implementation
    /// </summary>
    public static IServiceCollection AddInMemoryDatabaseAdapter(this IServiceCollection services)
    {
        services.AddSingleton<IDatabaseOutputPort, InMemoryDatabaseAdapter>();
        return services;
    }
}