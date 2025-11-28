using Microsoft.AspNetCore.Mvc;
using MakeTransfer.Core.Application.Ports.Incoming;
using MakeTransfer.Api.Models.Responses;

namespace MakeTransfer.Api.Controllers;

/// <summary>
/// Handles account inquiries - balance checks, account details, etc.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AccountsController : ControllerBase
{
    private readonly IBankingInquiryInputPort _bankingInquiry;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(
        IBankingInquiryInputPort bankingInquiry,
        ILogger<AccountsController> logger)
    {
        _bankingInquiry = bankingInquiry ?? throw new ArgumentNullException(nameof(bankingInquiry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get detailed account information
    /// </summary>
    /// <param name="accountId">Account identifier</param>
    [HttpGet("{accountId}")]
    [ProducesResponseType(typeof(ApiResponse<AccountResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public IActionResult GetAccountDetails(string accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                400,
                "Account ID is required"));
        }

        _logger.LogInformation("Retrieving account details for {AccountId}", accountId);

        try
        {
            var result = _bankingInquiry.GetAccountDetails(accountId);

            // Convert domain result to HTTP response
            AccountResponse? accountResponse = null;
            if (result.Success && result.Data != null)
            {
                accountResponse = new AccountResponse
                {
                    AccountId = result.Data.AccountId,
                    OwnerName = result.Data.OwnerName,
                    Currency = result.Data.Currency,
                    Balance = result.Data.Balance,
                    DailyDebitLimit = result.Data.DailyDebitLimit,
                    DailyDebitedAmount = result.Data.DailyDebitedAmount,
                    DailyLimitDate = result.Data.DailyLimitDate.ToString("yyyy-MM-dd"),
                    LastUpdated = result.Data.LastUpdated
                };
            }

            var apiResponse = result.Success
                ? ApiResponse<AccountResponse>.SuccessResponse(accountResponse!, result.Message)
                : ApiResponse<AccountResponse>.ErrorResponse(result.StatusCode, result.Message);

            return StatusCode(result.StatusCode, apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account details for {AccountId}", accountId);
            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                500,
                "Error retrieving account details"));
        }
    }

    /// <summary>
    /// Get account balance information
    /// </summary>
    /// <param name="accountId">Account identifier</param>
    [HttpGet("{accountId}/balance")]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public IActionResult GetAccountBalance(string accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                400,
                "Account ID is required"));
        }

        _logger.LogInformation("Retrieving balance for {AccountId}", accountId);

        try
        {
            var result = _bankingInquiry.GetAccountBalance(accountId);

            // Convert domain result to HTTP response
            BalanceResponse? balanceResponse = null;
            if (result.Success && result.Data != null)
            {
                balanceResponse = new BalanceResponse
                {
                    AccountId = result.Data.AccountId,
                    Available = result.Data.Available,
                    Current = result.Data.Current,
                    Currency = result.Data.Currency,
                    AsOf = result.Data.AsOf
                };
            }

            var apiResponse = result.Success
                ? ApiResponse<BalanceResponse>.SuccessResponse(balanceResponse!, result.Message)
                : ApiResponse<BalanceResponse>.ErrorResponse(result.StatusCode, result.Message);

            return StatusCode(result.StatusCode, apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving balance for {AccountId}", accountId);
            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                500,
                "Error retrieving account balance"));
        }
    }
}