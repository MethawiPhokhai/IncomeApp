using Postgrest.Attributes;
using Postgrest.Models;

namespace IncomeApp.Features.Financial.Models;

[Table("expenses")]
public class ExpenseEntity : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("type")]
    public string Type { get; set; } = string.Empty; // "Fixed", "Variable", "Family", "Health"

    [Column("color")]
    public string Color { get; set; } = string.Empty;

    [Column("bank_app")]
    public string BankApp { get; set; } = string.Empty; // "Dime", "Make", "KTB", "Kbank", "Office"

    [Column("is_highlighted")]
    public bool IsHighlighted { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
