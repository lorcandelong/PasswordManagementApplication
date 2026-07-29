using SecurePasswordApplication.Data;
using SecurePasswordApplication.Models;
using Microsoft.AspNetCore.Identity;

namespace SecurePasswordApplication.Services;

public class ActivityLogService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ActivityLogService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(string action, string description, string? userId = null)
    {
        if (string.IsNullOrEmpty(userId))
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user != null)
            {
                userId = _userManager.GetUserId(user);
            }
        }

        var log = new ActivityLog
        {
            UserId = userId,
            Action = action,
            Description = description,
            Timestamp = DateTime.UtcNow
        };

        _context.ActivityLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
