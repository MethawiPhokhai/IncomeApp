using IncomeApp.Features.Financial.Models;
using Supabase;

namespace IncomeApp.Features.Financial.Services;

public class FinancialService : IFinancialService
{
    private readonly Client _supabase;

    public FinancialService(Client supabase)
    {
        _supabase = supabase;
    }

    // No more in-memory storage - all data is now fetched from Supabase

    public async Task<DashboardSummary> GetDashboardSummaryAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        // Fetch financial summary from database
        var summaryData = await GetOrCreateFinancialSummaryAsync(userId, cancellationToken);
        
        // Fetch expenses, insurances, and debts from Supabase
        var expenses = await GetExpensesAsync(userId, cancellationToken);
        var insurances = await GetInsurancesAsync(userId, cancellationToken);
        var debts = await GetDebtsAsync(userId, cancellationToken);
        
        // Define subscriptions (TODO: Move to Supabase in the future)
        var subscriptions = new List<Subscription>
        {
            new() { Name = "ค่าประกัน AIA สะสมทรัพย์ 20/8", Amount = 1845m, BillingCycle = "Monthly", NextBillingDate = DateTime.Now.AddDays(20), Remark = "", BankApp = "KTB" },
            new() { Name = "กรองน้ำ Coway", Amount = 490m, BillingCycle = "Monthly", NextBillingDate = DateTime.Now.AddDays(5), Remark = "", BankApp = "KTB" },
            new() { Name = "กรองน้ำ Coway บ้าน", Amount = 690m, BillingCycle = "Monthly", NextBillingDate = DateTime.Now.AddDays(8), Remark = "", BankApp = "KTB" },
            new() { Name = "กรองอากาศ Coway", Amount = 555m, BillingCycle = "Monthly", NextBillingDate = DateTime.Now.AddDays(12), Remark = "", BankApp = "KTB" },
            new() { Name = "กรองอากาศ Coway บ้าน", Amount = 790m, BillingCycle = "Monthly", NextBillingDate = DateTime.Now.AddDays(15), Remark = "", BankApp = "KTB" },
            new() { Name = "Netflix", Amount = 105m, BillingCycle = "Monthly", NextBillingDate = DateTime.Now.AddDays(7), Remark = "โอนให้ไอเก่งทุกเดือน", BankApp = "Kbank" },
            new() { Name = "Spotify", Amount = 209m, BillingCycle = "Monthly", NextBillingDate = DateTime.Now.AddDays(10), Remark = "", BankApp = "Kbank" },
            new() { Name = "Youtube", Amount = 119m, BillingCycle = "Monthly", NextBillingDate = DateTime.Now.AddDays(14), Remark = "", BankApp = "Kbank" },
            new() { Name = "Google One", Amount = 750m, BillingCycle = "Monthly", NextBillingDate = DateTime.Now.AddDays(20), Remark = "", BankApp = "KTB" },
        };
        
        // Calculate chart data using the fetched expenses
        var chartData = new ChartData
        {
            ExpensesByApp = CalculateAppExpenses(expenses, subscriptions),
            TopExpenses = CalculateAppExpenses(expenses, subscriptions).Take(5).ToList()
        };
        
        // Calculate total expenses from actual data
        var appExpenseTotal = expenses.Sum(e => e.Amount);
        var subTotal = subscriptions.Sum(s => s.Amount);
        var totalExpenses = appExpenseTotal + subTotal;

        // Build the summary object using database values
        var summary = new DashboardSummary
        {
            Income = summaryData.Income,
            TotalSavings = summaryData.TotalSavings, 
            TotalInvestment = summaryData.TotalInvestment,
            TotalExpenses = totalExpenses, 
            NetWorthGrowth = summaryData.NetWorthGrowth,
            NetWorthGrowthPercent = (summaryData.NetWorthGrowth / (summaryData.Income > 0 ? summaryData.Income : 1)) * 100,
            SavingRate = summaryData.Income > 0 ? ((summaryData.TotalSavings + summaryData.TotalInvestment) / summaryData.Income) * 100 : 0,
            BurnRate = totalExpenses / 30, // Approx daily burn
            DailyBudget = (summaryData.Income - totalExpenses - summaryData.TotalSavings - summaryData.TotalInvestment) / 30,
            
            Categories = expenses,
            Subscriptions = subscriptions,
            Insurances = insurances,
            Debts = debts,
            
            Charts = chartData
        };
        
        return summary;
    }
    
    public async Task<DashboardSummary> UpdateSummaryStatsAsync(string userId, decimal income, decimal savings, decimal investment, decimal growth, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var userGuid = Guid.Parse(userId);
        var now = DateTime.UtcNow;
        
        // Try to get existing summary
        var existingSummary = await _supabase
            .From<FinancialSummary>()
            .Where(s => s.UserId == userGuid)
            .Single();
        
        if (existingSummary != null)
        {
            // Update existing summary
            existingSummary.Income = income;
            existingSummary.TotalSavings = savings;
            existingSummary.TotalInvestment = investment;
            existingSummary.NetWorthGrowth = growth;
            existingSummary.UpdatedAt = now;
            
            await _supabase
                .From<FinancialSummary>()
                .Update(existingSummary);
        }
        else
        {
            // Create new summary
            var newSummary = new FinancialSummary
            {
                Id = Guid.NewGuid(),
                UserId = userGuid,
                Income = income,
                TotalSavings = savings,
                TotalInvestment = investment,
                NetWorthGrowth = growth,
                CreatedAt = now,
                UpdatedAt = now
            };
            
            await _supabase
                .From<FinancialSummary>()
                .Insert(newSummary);
        }
        
        return await GetDashboardSummaryAsync(userId, cancellationToken);
    }
    
    public async Task<CategoryBreakdown> AddExpenseAsync(string userId, CategoryBreakdown expense, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var userGuid = Guid.Parse(userId);
        var now = DateTime.UtcNow;
        
        var newExpense = new ExpenseEntity
        {
            Id = Guid.NewGuid(),
            UserId = userGuid,
            Name = expense.Name,
            Amount = expense.Amount,
            Type = expense.Type,
            Color = GetAppColor(expense.BankApp),
            BankApp = expense.BankApp,
            CreatedAt = now,
            UpdatedAt = now
        };
        
        await _supabase
            .From<ExpenseEntity>()
            .Insert(newExpense);
        
        return new CategoryBreakdown
        {
            Id = newExpense.Id,
            Name = newExpense.Name,
            Amount = newExpense.Amount,
            Type = newExpense.Type,
            Color = newExpense.Color,
            BankApp = newExpense.BankApp
        };
    }

    public async Task<CategoryBreakdown?> UpdateExpenseAsync(string userId, Guid id, CategoryBreakdown expense, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var userGuid = Guid.Parse(userId);
        
        var existing = await _supabase
            .From<ExpenseEntity>()
            .Where(e => e.Id == id && e.UserId == userGuid)
            .Single();
        
        if (existing == null)
        {
            return null;
        }

        existing.Name = expense.Name;
        existing.Amount = expense.Amount;
        existing.Type = expense.Type;
        existing.BankApp = expense.BankApp;
        existing.Color = GetAppColor(expense.BankApp);
        existing.UpdatedAt = DateTime.UtcNow;
        
        await _supabase
            .From<ExpenseEntity>()
            .Update(existing);
        
        return new CategoryBreakdown
        {
            Id = existing.Id,
            Name = existing.Name,
            Amount = existing.Amount,
            Type = existing.Type,
            Color = existing.Color,
            BankApp = existing.BankApp
        };
    }

    public async Task<bool> DeleteExpenseAsync(string userId, Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var userGuid = Guid.Parse(userId);
        
        var existing = await _supabase
            .From<ExpenseEntity>()
            .Where(e => e.Id == id && e.UserId == userGuid)
            .Single();
        
        if (existing == null)
        {
            return false;
        }

        await _supabase
            .From<ExpenseEntity>()
            .Where(e => e.Id == id)
            .Delete();
        
        return true;
    }

    // Insurance CRUD operations
    public async Task<Insurance> AddInsuranceAsync(string userId, Insurance insurance, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var userGuid = Guid.Parse(userId);
        var now = DateTime.UtcNow;
        
        var newInsurance = new InsuranceEntity
        {
            Id = Guid.NewGuid(),
            UserId = userGuid,
            Provider = insurance.Provider,
            PolicyName = insurance.PolicyName,
            Premium = insurance.Premium,
            DueDate = insurance.DueDate,
            Status = insurance.Status,
            CreatedAt = now,
            UpdatedAt = now
        };
        
        await _supabase
            .From<InsuranceEntity>()
            .Insert(newInsurance);
        
        return new Insurance
        {
            Id = newInsurance.Id,
            Provider = newInsurance.Provider,
            PolicyName = newInsurance.PolicyName,
            Premium = newInsurance.Premium,
            DueDate = newInsurance.DueDate,
            Status = newInsurance.Status
        };
    }

    public async Task<Insurance?> UpdateInsuranceAsync(string userId, Guid id, Insurance insurance, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var userGuid = Guid.Parse(userId);
        
        var existing = await _supabase
            .From<InsuranceEntity>()
            .Where(i => i.Id == id && i.UserId == userGuid)
            .Single();
        
        if (existing == null)
        {
            return null;
        }

        existing.Provider = insurance.Provider;
        existing.PolicyName = insurance.PolicyName;
        existing.Premium = insurance.Premium;
        existing.DueDate = insurance.DueDate;
        existing.Status = insurance.Status;
        existing.UpdatedAt = DateTime.UtcNow;
        
        await _supabase
            .From<InsuranceEntity>()
            .Update(existing);
        
        return new Insurance
        {
            Id = existing.Id,
            Provider = existing.Provider,
            PolicyName = existing.PolicyName,
            Premium = existing.Premium,
            DueDate = existing.DueDate,
            Status = existing.Status
        };
    }

    public async Task<bool> DeleteInsuranceAsync(string userId, Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var userGuid = Guid.Parse(userId);
        
        var existing = await _supabase
            .From<InsuranceEntity>()
            .Where(i => i.Id == id && i.UserId == userGuid)
            .Single();
        
        if (existing == null)
        {
            return false;
        }

        await _supabase
            .From<InsuranceEntity>()
            .Where(i => i.Id == id)
            .Delete();
        
        return true;
    }

    // Debt CRUD operations
    public async Task<Debt> AddDebtAsync(string userId, Debt debt, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var userGuid = Guid.Parse(userId);
        var now = DateTime.UtcNow;
        
        var newDebt = new DebtEntity
        {
            Id = Guid.NewGuid(),
            UserId = userGuid,
            Name = debt.Name,
            MonthlyPayment = debt.MonthlyPayment,
            CurrentInstallment = debt.CurrentInstallment,
            TotalInstallments = debt.TotalInstallments,
            RemainingAmount = debt.RemainingAmount,
            TotalAmount = debt.TotalAmount,
            CreatedAt = now,
            UpdatedAt = now
        };
        
        await _supabase
            .From<DebtEntity>()
            .Insert(newDebt);
        
        return new Debt
        {
            Id = newDebt.Id,
            Name = newDebt.Name,
            MonthlyPayment = newDebt.MonthlyPayment,
            CurrentInstallment = newDebt.CurrentInstallment,
            TotalInstallments = newDebt.TotalInstallments,
            RemainingAmount = newDebt.RemainingAmount,
            TotalAmount = newDebt.TotalAmount
        };
    }

    public async Task<Debt?> UpdateDebtAsync(string userId, Guid id, Debt debt, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var userGuid = Guid.Parse(userId);
        
        var existing = await _supabase
            .From<DebtEntity>()
            .Where(d => d.Id == id && d.UserId == userGuid)
            .Single();
        
        if (existing == null)
        {
            return null;
        }

        existing.Name = debt.Name;
        existing.MonthlyPayment = debt.MonthlyPayment;
        existing.CurrentInstallment = debt.CurrentInstallment;
        existing.TotalInstallments = debt.TotalInstallments;
        existing.RemainingAmount = debt.RemainingAmount;
        existing.TotalAmount = debt.TotalAmount;
        existing.UpdatedAt = DateTime.UtcNow;
        
        await _supabase
            .From<DebtEntity>()
            .Update(existing);
        
        return new Debt
        {
            Id = existing.Id,
            Name = existing.Name,
            MonthlyPayment = existing.MonthlyPayment,
            CurrentInstallment = existing.CurrentInstallment,
            TotalInstallments = existing.TotalInstallments,
            RemainingAmount = existing.RemainingAmount,
            TotalAmount = existing.TotalAmount
        };
    }

    public async Task<bool> DeleteDebtAsync(string userId, Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var userGuid = Guid.Parse(userId);
        
        var existing = await _supabase
            .From<DebtEntity>()
            .Where(d => d.Id == id && d.UserId == userGuid)
            .Single();
        
        if (existing == null)
        {
            return false;
        }

        await _supabase
            .From<DebtEntity>()
            .Where(d => d.Id == id)
            .Delete();
        
        return true;
    }

    private static List<ChartDataPoint> CalculateAppExpenses(List<CategoryBreakdown> categories, List<Subscription> subscriptions)
    {
        // Group categories by BankApp and sum their amounts
        var appGroups = categories
            .Where(c => !string.IsNullOrEmpty(c.BankApp))
            .GroupBy(c => c.BankApp)
            .Select(g => new ChartDataPoint
            {
                Label = g.Key,
                Value = g.Sum(c => c.Amount),
                Color = GetAppColor(g.Key)
            })
            .ToList();
        
        // Add Subscription total
        var subscriptionTotal = subscriptions.Sum(s => s.Amount);
        if (subscriptionTotal > 0)
        {
            appGroups.Add(new ChartDataPoint
            {
                Label = "Subscription",
                Value = subscriptionTotal,
                Color = GetAppColor("Subscription")
            });
        }
        
        // Add Other/No App categories
        var otherTotal = categories
            .Where(c => string.IsNullOrEmpty(c.BankApp))
            .Sum(c => c.Amount);
        if (otherTotal > 0)
        {
            appGroups.Add(new ChartDataPoint
            {
                Label = "อื่นๆ",
                Value = otherTotal,
                Color = GetAppColor("Other")
            });
        }
        
        // Sort by value descending
        return appGroups.OrderByDescending(a => a.Value).ToList();
    }
    
    private static string GetAppColor(string appName)
    {
        return appName switch
        {
            "Kbank" => "#10b981",      // Green
            "Office" => "#8b5cf6",     // Purple
            "Make" => "#f59e0b",       // Orange/Amber
            "KTB" => "#06b6d4",        // Cyan
            "Dime" => "#ef4444",       // Red
            "Subscription" => "#ec4899", // Pink
            "Other" or "อื่นๆ" => "#6b7280", // Gray
            _ => "#94a3b8"             // Default slate
        };
    }
    
    /// <summary>
    /// Helper method to get or create financial summary for a user
    /// </summary>
    private async Task<FinancialSummary> GetOrCreateFinancialSummaryAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userGuid = Guid.Parse(userId);
        
        try
        {
            // Try to get existing summary
            var existingSummary = await _supabase
                .From<FinancialSummary>()
                .Where(s => s.UserId == userGuid)
                .Single();
            
            if (existingSummary != null)
            {
                return existingSummary;
            }
        }
        catch
        {
            // Summary doesn't exist, will create default one
        }
        
        // Create default summary with initial values
        var now = DateTime.UtcNow;
        var defaultSummary = new FinancialSummary
        {
            Id = Guid.NewGuid(),
            UserId = userGuid,
            Income = 103000m,
            TotalSavings = 22000m,
            TotalInvestment = 47000m,
            NetWorthGrowth = 15000m,
            CreatedAt = now,
            UpdatedAt = now
        };
        
        await _supabase
            .From<FinancialSummary>()
            .Insert(defaultSummary);
        
        return defaultSummary;
    }
    
    /// <summary>
    /// Helper method to get expenses for a user from Supabase
    /// </summary>
    private async Task<List<CategoryBreakdown>> GetExpensesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userGuid = Guid.Parse(userId);
        
        var expenseEntities = await _supabase
            .From<ExpenseEntity>()
            .Where(e => e.UserId == userGuid)
            .Get();
        
        return expenseEntities.Models
            .Select(e => new CategoryBreakdown
            {
                Id = e.Id,
                Name = e.Name,
                Amount = e.Amount,
                Type = e.Type,
                Color = e.Color,
                BankApp = e.BankApp
            })
            .ToList();
    }
    
    /// <summary>
    /// Helper method to get insurances for a user from Supabase
    /// </summary>
    private async Task<List<Insurance>> GetInsurancesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userGuid = Guid.Parse(userId);
        
        var insuranceEntities = await _supabase
            .From<InsuranceEntity>()
            .Where(i => i.UserId == userGuid)
            .Get();
        
        return insuranceEntities.Models
            .Select(i => new Insurance
            {
                Id = i.Id,
                Provider = i.Provider,
                PolicyName = i.PolicyName,
                Premium = i.Premium,
                DueDate = i.DueDate,
                Status = i.Status
            })
            .ToList();
    }
    
    /// <summary>
    /// Helper method to get debts for a user from Supabase
    /// </summary>
    private async Task<List<Debt>> GetDebtsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userGuid = Guid.Parse(userId);
        
        var debtEntities = await _supabase
            .From<DebtEntity>()
            .Where(d => d.UserId == userGuid)
            .Get();
        
        return debtEntities.Models
            .Select(d => new Debt
            {
                Id = d.Id,
                Name = d.Name,
                MonthlyPayment = d.MonthlyPayment,
                CurrentInstallment = d.CurrentInstallment,
                TotalInstallments = d.TotalInstallments,
                RemainingAmount = d.RemainingAmount,
                TotalAmount = d.TotalAmount
            })
            .ToList();
    }
}
