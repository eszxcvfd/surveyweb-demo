using Microsoft.AspNetCore.Mvc;
using SurveyWeb.Models;
using SurveyWeb.Services.Interfaces;

namespace SurveyWeb.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    // GET: Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success, message, user) = await _accountService.RegisterAsync(model);

        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }

        // Lưu thông tin vào session
        HttpContext.Session.SetString("UserId", user!._id.ToString());
        HttpContext.Session.SetString("UserEmail", user.email);
        HttpContext.Session.SetString("UserName", user.fullName ?? user.email);

        TempData["SuccessMessage"] = "Đăng ký thành công! Chào mừng bạn đến với SurveyWeb.";
        return RedirectToAction("Index", "Home");
    }

    // GET: Account/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success, message, user) = await _accountService.LoginAsync(model);

        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }

        // Lưu thông tin vào session
        HttpContext.Session.SetString("UserId", user!._id.ToString());
        HttpContext.Session.SetString("UserEmail", user.email);
        HttpContext.Session.SetString("UserName", user.fullName ?? user.email);

        TempData["SuccessMessage"] = "Đăng nhập thành công!";
        return RedirectToAction("Index", "Home");
    }

    // GET: Account/Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["SuccessMessage"] = "Đã đăng xuất thành công.";
        return RedirectToAction("Index", "Home");
    }
}
