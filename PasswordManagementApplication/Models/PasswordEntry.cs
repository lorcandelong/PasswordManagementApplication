using SecurePasswordApplication.Data;
namespace SecurePasswordApplication.Models

{
    public class PasswordEntry
    {
        public int Id { get; set; }

        public string Website { get; set; } = "";

        public string Username { get; set; } = "";

        public string Password { get; set; } = "";

        public string? Notes { get; set; }

        public string? UserId { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime CreatedDate { get; set; }
        
        public ApplicationUser? User { get; set; }
    }
}