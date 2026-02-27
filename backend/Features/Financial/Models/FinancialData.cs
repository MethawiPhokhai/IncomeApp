namespace IncomeApp.Features.Financial.Models;

public class DashboardSummary
{
    public decimal Income { get; set; }
    public decimal TotalSavings { get; set; }
    public decimal TotalInvestment { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetWorthGrowth { get; set; }
    public decimal NetWorthGrowthPercent { get; set; }
    public decimal SavingRate { get; set; }
    public decimal BurnRate { get; set; }
    public decimal DailyBudget { get; set; }
    public List<CategoryBreakdown> Categories { get; set; } = new();
    public List<Subscription> Subscriptions { get; set; } = new();
    public List<Insurance> Insurances { get; set; } = new();
    public List<Debt> Debts { get; set; } = new();
    public ChartData Charts { get; set; } = new();
}

public class UpdateSummaryRequest
{
    public decimal Income { get; set; }
    public decimal TotalSavings { get; set; }
    public decimal TotalInvestment { get; set; }
    public decimal NetWorthGrowth { get; set; }
}

public class CategoryBreakdown
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty; // "Fixed", "Variable", "Family", "Health"
    public string Color { get; set; } = string.Empty;
    public string BankApp { get; set; } = string.Empty; // "Dime", "Make", "KTB", "Kbank", "Office"
    public bool IsHighlighted { get; set; }
}

public class Subscription
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string BillingCycle { get; set; } = string.Empty; // "Monthly", "Yearly"
    public DateTime NextBillingDate { get; set; }
    public string Remark { get; set; } = string.Empty; // Additional notes
    public string BankApp { get; set; } = string.Empty; // Bank app name: "KTB", "Kbank"
}

public class Insurance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Provider { get; set; } = string.Empty;
    public string PolicyName { get; set; } = string.Empty;
    public decimal Premium { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty; // "Paid", "Upcoming", "Overdue"
}

public class Debt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public decimal MonthlyPayment { get; set; }
    public int CurrentInstallment { get; set; }
    public int TotalInstallments { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class ChartData
{
    public List<ChartDataPoint> ExpensesByApp { get; set; } = new();
    public List<ChartDataPoint> TopExpenses { get; set; } = new();
}

public class ChartDataPoint
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Color { get; set; } = string.Empty;
}
