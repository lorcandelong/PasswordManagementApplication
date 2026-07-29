using SecurePasswordApplication.Data;

namespace SecurePasswordApplication.Models;

public class ActivityLog
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public ApplicationUser? User { get; set; }

    public required string Action { get; set; }

    public required string Description { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}