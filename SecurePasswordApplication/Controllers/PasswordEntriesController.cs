using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurePasswordApplication.Models;
using SecurePasswordApplication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using SecurePasswordApplication.Services;

[Authorize]
public class PasswordEntriesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ActivityLogService _activityLogService;
    private readonly EncryptionService _encryptionService;

    public PasswordEntriesController(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    ActivityLogService activityLogService,
    EncryptionService encryptionService)
    {
        _context = context;
        _userManager = userManager;
        _activityLogService = activityLogService;
        _encryptionService = encryptionService;
    }

    // GET: PASSWORDENTRYS
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        var entries = await _context.PasswordEntries
            .Where(p => p.UserId == userId)
            .ToListAsync();

        return View(entries);
    }
    // GET: PASSWORDENTRYS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);

        var passwordentry = await _context.PasswordEntries
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (passwordentry == null)
        {
            return NotFound();
        }

        return View(passwordentry);
    }

    // GET: PASSWORDENTRYS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: PASSWORDENTRYS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Website,Username,Password,Notes,ExpirationDate")] PasswordEntry passwordentry)
    {
        if (ModelState.IsValid)
        {
            passwordentry.UserId = _userManager.GetUserId(User);
            passwordentry.Password =
            _encryptionService.Encrypt(passwordentry.Password);
            passwordentry.CreatedDate = DateTime.Now;
            _context.Add(passwordentry);
            await _context.SaveChangesAsync();

            await _activityLogService.LogAsync(
                "Create",
                $"Created password entry for {passwordentry.Website}"
            );

            return RedirectToAction(nameof(Index));
        }
        return View(passwordentry);
    }

    // GET: PASSWORDENTRYS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);

        var passwordentry = await _context.PasswordEntries
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (passwordentry == null)
        {
            return NotFound();
        }
        passwordentry.Password =
        _encryptionService.Decrypt(passwordentry.Password);
        return View(passwordentry);
    }

    // POST: PASSWORDENTRYS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Website,Username,Password,Notes,ExpirationDate")] PasswordEntry passwordentry)
    {
        if (id != passwordentry.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var userId = _userManager.GetUserId(User);

            var existingEntry = await _context.PasswordEntries
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (existingEntry == null)
            {
                return NotFound();
            }

            existingEntry.Website = passwordentry.Website;
            existingEntry.Username = passwordentry.Username;
            existingEntry.Password =
            _encryptionService.Encrypt(passwordentry.Password);
            existingEntry.Notes = passwordentry.Notes;
            existingEntry.ExpirationDate = passwordentry.ExpirationDate;

            await _context.SaveChangesAsync();

            await _activityLogService.LogAsync(
                "Edit",
                $"Updated password entry for {existingEntry.Website}"
            );

            return RedirectToAction(nameof(Index));
        }
       
        return View(passwordentry);
    }

    // GET: PASSWORDENTRYS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var userId = _userManager.GetUserId(User);

        var passwordentry = await _context.PasswordEntries
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (passwordentry == null)
        {
            return NotFound();
        }

        return View(passwordentry);
    }

    // POST: PASSWORDENTRYS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var userId = _userManager.GetUserId(User);

        var passwordentry = await _context.PasswordEntries
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (passwordentry != null)
        {
            await _activityLogService.LogAsync(
            "Delete",
            $"Deleted password entry for {passwordentry.Website}"
            );

            _context.PasswordEntries.Remove(passwordentry);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetPassword(int id)
    {
        var userId = _userManager.GetUserId(User);

        var passwordEntry = await _context.PasswordEntries
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (passwordEntry == null)
        {
            return NotFound();
        }

        await _activityLogService.LogAsync(
            "Copy",
            $"Copied password for {passwordEntry.Website}"
        );

        var decryptedPassword =
     _encryptionService.Decrypt(passwordEntry.Password);

        return Content(decryptedPassword);
    }

    [HttpGet]
    public async Task<IActionResult> RevealPassword(int id)
    {
        var userId = _userManager.GetUserId(User);

        var passwordEntry = await _context.PasswordEntries
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (passwordEntry == null)
        {
            return NotFound();
        }

        await _activityLogService.LogAsync(
            "Reveal",
            $"Viewed password for {passwordEntry.Website}"
        );

        var decryptedPassword =
    _encryptionService.Decrypt(passwordEntry.Password);

        return Content(decryptedPassword);
    }

}
