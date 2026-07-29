using Microsoft.EntityFrameworkCore;
using SecurePasswordApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SecurePasswordApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PasswordEntry> PasswordEntries { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        
    }
}
