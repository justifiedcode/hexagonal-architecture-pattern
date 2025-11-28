using MakeTransfer.Core.Application.DataSets;
using MakeTransfer.Core.Shared;

namespace MakeTransfer.Core.Application.Ports.Incoming;

/// <summary>
/// Input port for core banking operations that involve money movement.
/// This port represents transactional operations with strong consistency requirements.
/// </summary>
public interface IBankingOperationsInputPort
{
    /// <summary>
    /// Executes a money transfer between two accounts.
    /// This is an atomic operation that ensures consistency across accounts and audit trail.
    /// </summary>
    /// <param name="transferData">The transfer data containing all transfer details</param>
    /// <returns>Operation result with transfer outcome</returns>
    OperationResult<TransferResult> ExecuteTransfer(TransferData transferData);
}