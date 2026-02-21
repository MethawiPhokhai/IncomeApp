using Postgrest.Attributes;
using Postgrest.Models;

namespace IncomeApp.Features.Financial.Models;

[Table("insurances")]
public class InsuranceEntity : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("provider")]
    public string Provider { get; set; } = string.Empty;

    [Column("policy_name")]
    public string PolicyName { get; set; } = string.Empty;

    [Column("premium")]
    public decimal Premium { get; set; }

    [Column("due_date")]
    public DateTime DueDate { get; set; }

    [Column("status")]
    public string Status { get; set; } = string.Empty; // "Paid", "Upcoming", "Overdue"

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
