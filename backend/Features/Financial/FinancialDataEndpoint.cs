using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IncomeApp.Features.Financial.Models;
using IncomeApp.Features.Financial.Services;
using System.Security.Claims;

namespace IncomeApp.Features.Financial;

[ApiController]
[Route("api/financial")]
[Authorize]
public class FinancialDataEndpoint : ControllerBase
{
    private readonly IFinancialService _financialService;
    private readonly ILogger<FinancialDataEndpoint> _logger;

    public FinancialDataEndpoint(IFinancialService financialService, ILogger<FinancialDataEndpoint> logger)
    {
        _financialService = financialService;
        _logger = logger;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardSummary>> GetDashboard(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var dashboard = await _financialService.GetDashboardSummaryAsync(userId, cancellationToken);
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve dashboard for UserId={UserId}", GetUserId());
            return StatusCode(500, new { message = "Error retrieving dashboard data" });
        }
    }

    [HttpPost("summary")]
    public async Task<ActionResult<DashboardSummary>> UpdateSummary([FromBody] UpdateSummaryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _financialService.UpdateSummaryStatsAsync(
                userId,
                request.Income,
                request.TotalSavings,
                request.TotalInvestment,
                request.NetWorthGrowth,
                cancellationToken
            );
            _logger.LogInformation("Summary stats updated for UserId={UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update summary stats for UserId={UserId}", GetUserId());
            return StatusCode(500, new { message = "Error updating summary" });
        }
    }

    [HttpPost("expenses")]
    public async Task<ActionResult<CategoryBreakdown>> CreateExpense([FromBody] CategoryBreakdown expense, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _financialService.AddExpenseAsync(userId, expense, cancellationToken);
            _logger.LogInformation("Expense created. ExpenseId={ExpenseId}, UserId={UserId}", result.Id, userId);
            return CreatedAtAction(nameof(GetDashboard), new { }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create expense for UserId={UserId}", GetUserId());
            return StatusCode(500, new { message = "Error creating expense" });
        }
    }

    [HttpPut("expenses/{id}")]
    public async Task<ActionResult<CategoryBreakdown>> UpdateExpense(Guid id, [FromBody] CategoryBreakdown expense, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _financialService.UpdateExpenseAsync(userId, id, expense, cancellationToken);
            if (result == null) return NotFound();

            _logger.LogInformation("Expense updated. ExpenseId={ExpenseId}, UserId={UserId}", id, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update expense ExpenseId={ExpenseId} for UserId={UserId}", id, GetUserId());
            return StatusCode(500, new { message = "Error updating expense" });
        }
    }

    [HttpDelete("expenses/{id}")]
    public async Task<ActionResult> DeleteExpense(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _financialService.DeleteExpenseAsync(userId, id, cancellationToken);
            if (!success) return NotFound();

            _logger.LogInformation("Expense deleted. ExpenseId={ExpenseId}, UserId={UserId}", id, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete expense ExpenseId={ExpenseId} for UserId={UserId}", id, GetUserId());
            return StatusCode(500, new { message = "Error deleting expense" });
        }
    }

    [HttpPost("insurances")]
    public async Task<ActionResult<Insurance>> CreateInsurance([FromBody] Insurance insurance, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _financialService.AddInsuranceAsync(userId, insurance, cancellationToken);
            _logger.LogInformation("Insurance created. InsuranceId={InsuranceId}, UserId={UserId}", result.Id, userId);
            return CreatedAtAction(nameof(GetDashboard), new { }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create insurance for UserId={UserId}", GetUserId());
            return StatusCode(500, new { message = "Error creating insurance" });
        }
    }

    [HttpPut("insurances/{id}")]
    public async Task<ActionResult<Insurance>> UpdateInsurance(Guid id, [FromBody] Insurance insurance, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _financialService.UpdateInsuranceAsync(userId, id, insurance, cancellationToken);
            if (result == null) return NotFound();

            _logger.LogInformation("Insurance updated. InsuranceId={InsuranceId}, UserId={UserId}", id, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update insurance InsuranceId={InsuranceId} for UserId={UserId}", id, GetUserId());
            return StatusCode(500, new { message = "Error updating insurance" });
        }
    }

    [HttpDelete("insurances/{id}")]
    public async Task<ActionResult> DeleteInsurance(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _financialService.DeleteInsuranceAsync(userId, id, cancellationToken);
            if (!success) return NotFound();

            _logger.LogInformation("Insurance deleted. InsuranceId={InsuranceId}, UserId={UserId}", id, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete insurance InsuranceId={InsuranceId} for UserId={UserId}", id, GetUserId());
            return StatusCode(500, new { message = "Error deleting insurance" });
        }
    }

    [HttpPost("debts")]
    public async Task<ActionResult<Debt>> CreateDebt([FromBody] Debt debt, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _financialService.AddDebtAsync(userId, debt, cancellationToken);
            _logger.LogInformation("Debt created. DebtId={DebtId}, UserId={UserId}", result.Id, userId);
            return CreatedAtAction(nameof(GetDashboard), new { }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create debt for UserId={UserId}", GetUserId());
            return StatusCode(500, new { message = "Error creating debt" });
        }
    }

    [HttpPut("debts/{id}")]
    public async Task<ActionResult<Debt>> UpdateDebt(Guid id, [FromBody] Debt debt, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _financialService.UpdateDebtAsync(userId, id, debt, cancellationToken);
            if (result == null) return NotFound();

            _logger.LogInformation("Debt updated. DebtId={DebtId}, UserId={UserId}", id, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update debt DebtId={DebtId} for UserId={UserId}", id, GetUserId());
            return StatusCode(500, new { message = "Error updating debt" });
        }
    }

    [HttpDelete("debts/{id}")]
    public async Task<ActionResult> DeleteDebt(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _financialService.DeleteDebtAsync(userId, id, cancellationToken);
            if (!success) return NotFound();

            _logger.LogInformation("Debt deleted. DebtId={DebtId}, UserId={UserId}", id, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete debt DebtId={DebtId} for UserId={UserId}", id, GetUserId());
            return StatusCode(500, new { message = "Error deleting debt" });
        }
    }

    private string? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        return claim?.Value;
    }
}
