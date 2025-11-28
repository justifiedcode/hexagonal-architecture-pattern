using MakeTransfer.Core.Application.Ports.Outgoing;
using MakeTransfer.Core.Domain.Transfers;

namespace MakeTransfer.Adapters.Notification;

/// <summary>
/// Simple console notification for demo purposes.
/// In production, this would connect to SendGrid, Twilio, etc.
/// </summary>
public sealed class ConsoleNotificationAdapter : INotificationOutputPort
{
    public void NotifyTransfer(Transfer transfer, string message)
    {
        var timestamp = DateTime.UtcNow.ToString("HH:mm:ss");

        Console.WriteLine($"[{timestamp}] {transfer.Status}: {message}");
        Console.WriteLine($"       Transfer: {transfer.TransferId} | {transfer.Amount:C} {transfer.Currency}");

        if (transfer.Status == TransferStatus.Failed && !string.IsNullOrEmpty(transfer.FailureReason))
        {
            Console.WriteLine($"  Reason: {transfer.FailureReason}");
        }

        Console.WriteLine();
    }
}