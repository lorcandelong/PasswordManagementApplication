// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SecurePasswordApplication.Data;
using SecurePasswordApplication.Services;

namespace SecurePasswordApplication.Areas.Identity.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LogoutModel> _logger;
    private readonly ActivityLogService _activityLogService;

    public LogoutModel(
       SignInManager<ApplicationUser> signInManager,
       ILogger<LogoutModel> logger,
       ActivityLogService activityLogService)
    {
        _signInManager = signInManager;
        _logger = logger;
        _activityLogService = activityLogService;
    }
    public async Task<IActionResult> OnPost(string? returnUrl = null)
    {
        await _activityLogService.LogAsync(
        "Logout",
        "User logged out"
        );

        await _signInManager.SignOutAsync();

        _logger.LogInformation("User logged out.");

        TempData["LogoutMessage"] = "You have successfully logged out.";

        return RedirectToAction("Index", "Home");
       
    }
}
