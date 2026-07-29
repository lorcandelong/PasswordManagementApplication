using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurePasswordApplication.Data;
using SecurePasswordApplication.Models;
using SecurePasswordApplication.Services;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace SecurePasswordApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private int CalculatePasswordStrength(string password)
        {
            int score = 0;

            if (password.Length >= 8)
                score++;

            if (password.Length >= 12)
                score++;

            if (password.Any(char.IsUpper))
                score++;

            if (password.Any(char.IsLower))
                score++;

            if (password.Any(char.IsDigit))
                score++;

            if (password.Any(ch => !char.IsLetterOrDigit(ch)))
                score++;

            return score;
        }


        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EncryptionService _encryptionService;

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var totalPasswords = await _context.PasswordEntries
            .CountAsync(p => p.UserId == userId);

            var activityLogs = await _context.ActivityLogs
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .ToListAsync();

            var today = DateTime.Today;

            var expiringPasswords = await _context.PasswordEntries
                .Where(p => p.UserId == userId &&
                            p.ExpirationDate.HasValue &&
                            p.ExpirationDate.Value.Date <= today.AddDays(30))
                .OrderBy(p => p.ExpirationDate)
                .ToListAsync();

            var passwords = await _context.PasswordEntries
            .Where(p => p.UserId == userId)
             .ToListAsync();

            var cutoffDate = DateTime.UtcNow.AddDays(-10);

            var failedLoginAttempts = await _context.ActivityLogs
                .CountAsync(a =>
                    a.UserId == userId &&
                    a.Action == "Security" &&
                    a.Description.Contains("Failed login") &&
                    a.Timestamp >= cutoffDate);
            var expiredPasswords = await _context.PasswordEntries
          .CountAsync(p =>
             p.UserId == userId &&
             p.ExpirationDate.HasValue &&
              p.ExpirationDate < today);



            int weakPasswords = 0;

            foreach (var password in passwords)
            {
                var decrypted =
                    _encryptionService.Decrypt(password.Password);

                if (CalculatePasswordStrength(decrypted) < 4)
                {
                    weakPasswords++;
                }
            }

            int securityScore = 100;

            securityScore -= failedLoginAttempts * 1;

            securityScore -= weakPasswords * 3;

            securityScore -= expiredPasswords * 5;


            if (securityScore < 0)
            {
                securityScore = 0;
            }
            var model = new HomeViewModel
            {
                TotalPasswords = totalPasswords,
                SecurityScore = securityScore,
                FailedLoginAttempts = failedLoginAttempts,
                WeakPasswords = weakPasswords,
                ExpiredPasswords = expiredPasswords,
                ActivityLogs = activityLogs,
                ExpiringPasswords = expiringPasswords
            };

           

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public HomeController(
     ApplicationDbContext context,
     UserManager<ApplicationUser> userManager,
     EncryptionService encryptionService)
        {
            _context = context;
            _userManager = userManager;
            _encryptionService = encryptionService;
        }

    }

}
