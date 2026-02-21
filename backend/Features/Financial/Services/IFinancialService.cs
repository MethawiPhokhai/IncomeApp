using IncomeApp.Features.Financial.Models;

namespace IncomeApp.Features.Financial.Services;

public interface IFinancialService
{
    Task<DashboardSummary> GetDashboardSummaryAsync(string userId, CancellationToken cancellationToken = default);
    Task<DashboardSummary> UpdateSummaryStatsAsync(string userId, decimal income, decimal savings, decimal investment, decimal growth, CancellationToken cancellationToken = default);
    Task<CategoryBreakdown> AddExpenseAsync(string userId, CategoryBreakdown expense, CancellationToken cancellationToken = default);
    Task<CategoryBreakdown?> UpdateExpenseAsync(string userId, Guid id, CategoryBreakdown expense, CancellationToken cancellationToken = default);
    Task<bool> DeleteExpenseAsync(string userId, Guid id, CancellationToken cancellationToken = default);
    
    // Insurance CRUD operations
    Task<Insurance> AddInsuranceAsync(string userId, Insurance insurance, CancellationToken cancellationToken = default);
    Task<Insurance?> UpdateInsuranceAsync(string userId, Guid id, Insurance insurance, CancellationToken cancellationToken = default);
    Task<bool> DeleteInsuranceAsync(string userId, Guid id, CancellationToken cancellationToken = default);
    
    // Debt CRUD operations
    Task<Debt> AddDebtAsync(string userId, Debt debt, CancellationToken cancellationToken = default);
    Task<Debt?> UpdateDebtAsync(string userId, Guid id, Debt debt, CancellationToken cancellationToken = default);
    Task<bool> DeleteDebtAsync(string userId, Guid id, CancellationToken cancellationToken = default);
}
