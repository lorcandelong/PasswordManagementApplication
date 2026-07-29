namespace SecurePasswordApplication.Models
{
    public class HomeViewModel
    {
        public int TotalPasswords { get; set; }

        public int SecurityScore { get; set; }

        public int FailedLoginAttempts { get; set; }

        public int WeakPasswords { get; set; }

        public int ExpiredPasswords { get; set; }


        public IEnumerable<ActivityLog> ActivityLogs { get; set; }
            = new List<ActivityLog>();

        public IEnumerable<PasswordEntry> ExpiringPasswords { get; set; }
            = new List<PasswordEntry>();
    }
}