using Postgrest.Attributes;
using Postgrest.Models;

namespace IncomeApp.Features.Financial.Models;

[Table("debts")]
public class DebtEntity : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("monthly_payment")]
    public decimal MonthlyPayment { get; set; }

    [Column("current_installment")]
    public int CurrentInstallment { get; set; }

    [Column("total_installments")]
    public int TotalInstallments { get; set; }

    [Column("remaining_amount")]
    public decimal RemainingAmount { get; set; }

    [Column("total_amount")]
    public decimal TotalAmount { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
