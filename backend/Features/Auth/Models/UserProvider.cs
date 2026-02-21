using Postgrest.Attributes;
using Postgrest.Models;

namespace backend.Features.Auth.Models;

[Table("user_providers")]
public class UserProvider : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("provider_name")]
    public string ProviderName { get; set; } = string.Empty;

    [Column("provider_user_id")]
    public string ProviderUserId { get; set; } = string.Empty;

    [Column("provider_email")]
    public string? ProviderEmail { get; set; }

    [Column("provider_data")]
    public Dictionary<string, object>? ProviderData { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
