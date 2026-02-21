using Postgrest.Attributes;
using Postgrest.Models;

namespace IncomeApp.Features.Financial.Models;

[Table("financial_summaries")]
public class FinancialSummary : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }
    
    [Column("user_id")]
    public Guid UserId { get; set; }
    
    [Column("income")]
    public decimal Income { get; set; }
    
    [Column("total_savings")]
    public decimal TotalSavings { get; set; }
    
    [Column("total_investment")]
    public decimal TotalInvestment { get; set; }
    
    [Column("net_worth_growth")]
    public decimal NetWorthGrowth { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
