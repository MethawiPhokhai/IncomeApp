using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IncomeApp.Features.Financial.Models;
using IncomeApp.Features.Financial.Services;
using System.Security.Claims;

namespace IncomeApp.Features.Financial;

[ApiController]
[Route("api/financial")]
[Authorize] // Require JWT authentication
public class FinancialDataEndpoint : ControllerBase
{
    private readonly IFinancialService _financialService;

    public FinancialDataEndpoint(IFinancialService financialService)
    {
        _financialService = financialService;
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
            return StatusCode(500, new { message = "Error retrieving dashboard data", error = ex.Message, innerError = ex.InnerException?.Message });
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
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating summary", error = ex.Message });
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
            return CreatedAtAction(nameof(GetDashboard), new { }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating expense", error = ex.Message });
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
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating expense", error = ex.Message });
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
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting expense", error = ex.Message });
        }
    }

    // Insurance CRUD operations
    [HttpPost("insurances")]
    public async Task<ActionResult<Insurance>> CreateInsurance([FromBody] Insurance insurance, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _financialService.AddInsuranceAsync(userId, insurance, cancellationToken);
            return CreatedAtAction(nameof(GetDashboard), new { }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating insurance", error = ex.Message });
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
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating insurance", error = ex.Message });
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
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting insurance", error = ex.Message });
        }
    }

    // Debt CRUD operations
    [HttpPost("debts")]
    public async Task<ActionResult<Debt>> CreateDebt([FromBody] Debt debt, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _financialService.AddDebtAsync(userId, debt, cancellationToken);
            return CreatedAtAction(nameof(GetDashboard), new { }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating debt", error = ex.Message });
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
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating debt", error = ex.Message });
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
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting debt", error = ex.Message });
        }
    }

    private string? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        return claim?.Value;
    }
}
