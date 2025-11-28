using MakeTransfer.Core.Domain.Transfers;

namespace MakeTransfer.Core.Application.Ports.Outgoing;

/// <summary>
/// Output port for sending notifications about transfer events.
/// Implementations handle sending notifications via email, SMS, push notifications, etc.
/// </summary>
public interface INotificationOutputPort
{
    void NotifyTransfer(Transfer transfer, string message);
}