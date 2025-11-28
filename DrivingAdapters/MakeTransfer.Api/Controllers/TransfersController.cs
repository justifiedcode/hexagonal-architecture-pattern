using Microsoft.AspNetCore.Mvc;
using MakeTransfer.Core.Application.Ports.Incoming;
using MakeTransfer.Core.Application.DataSets;
using MakeTransfer.Api.Models.Requests;
using MakeTransfer.Api.Models.Responses;

namespace MakeTransfer.Api.Controllers;

/// <summary>
/// Handles money transfer requests from clients.
/// This is where HTTP requests get converted to domain operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class TransfersController : ControllerBase
{
    private readonly IBankingOperationsInputPort _bankingOperations;
    private readonly ILogger<TransfersController> _logger;

    public TransfersController(
        IBankingOperationsInputPort bankingOperations,
        ILogger<TransfersController> logger)
    {
        _bankingOperations = bankingOperations ?? throw new ArgumentNullException(nameof(bankingOperations));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Execute a money transfer between two accounts
    /// </summary>
    /// <param name="request">Transfer details from the client</param>
    /// <returns>HTTP response with transfer status</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TransferResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    [ProducesResponseType(typeof(ApiResponse<object>), 502)]
    public IActionResult ExecuteTransfer([FromBody] MakeTransferRequest request)
    {
        _logger.LogInformation(
            "Processing transfer request from {FromAccount} to {ToAccount} for {Amount} {Currency}",
            request.FromAccountId,
            request.ToAccountId,
            request.Amount,
            request.Currency);

        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                400,
                "Invalid request data",
                ModelState));
        }

        try
        {
            // Convert HTTP request to domain data
            var transferData = new TransferData(
                request.FromAccountId,
                request.ToAccountId,
                request.Amount,
                request.Currency,
                request.Reference,
                "WEB_API",     // Who initiated this
                "WEB_API"      // Which channel
            );

            // Let the domain handle the business logic
            var result = _bankingOperations.ExecuteTransfer(transferData);

            // Convert domain result back to HTTP response
            var transferResponse = new TransferResponse
            {
                TransferId = result.Data?.TransferId ?? "N/A",
                Status = result.Data?.Status.ToString() ?? "Unknown",
                Message = result.Message,
                ProcessedAt = result.Data?.ProcessedAt ?? DateTime.UtcNow,
                ExternalReference = result.Data?.ExternalReference
            };

            var apiResponse = result.Success
                ? ApiResponse<TransferResponse>.SuccessResponse(
                    transferResponse,
                    result.Message,
                    result.CorrelationId)
                : ApiResponse<TransferResponse>.ErrorResponse(
                    result.StatusCode,
                    result.Message,
                    transferResponse,
                    result.CorrelationId);

            _logger.LogInformation(
                "Transfer request completed with status {StatusCode}: {Message} [CorrelationId: {CorrelationId}]",
                result.StatusCode,
                result.Message,
                result.CorrelationId);

            return StatusCode(result.StatusCode, apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error processing transfer request for {FromAccount} to {ToAccount}",
                request.FromAccountId,
                request.ToAccountId);

            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                500,
                "An unexpected error occurred while processing the transfer"));
        }
    }
}