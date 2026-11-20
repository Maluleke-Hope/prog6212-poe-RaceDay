using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using System.Text.Json;

namespace RaceDay.Web.Controllers;

public class AuthController : Controller
{
    private readonly ApiClient _apiClient;

    public AuthController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var response = await _apiClient.PostAsync("api/auth/login", new { email, password });

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Invalid login attempt.";
            return View();
        }

        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            var authCookie = cookies.FirstOrDefault(c => c.StartsWith("RaceDay.Auth="));
            if (authCookie != null)
            {
                HttpContext.Session.SetString("AuthCookie", authCookie.Split(';')[0]);
            }
        }

        var json = await response.Content.ReadAsStringAsync();
        var user = JsonDocument.Parse(json).RootElement;

        HttpContext.Session.SetString("UserId", user.GetProperty("userId").ToString());
        HttpContext.Session.SetString("UserRole", user.GetProperty("role").GetString()!);
        HttpContext.Session.SetString("UserName", user.GetProperty("fullName").GetString()!);

        return RedirectToAction("Index", "Events");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string fullName, string email, string password, string role)
    {
        var response = await _apiClient.PostAsync("api/auth/register", new { fullName, email, password, role });

        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = await response.Content.ReadAsStringAsync();
            ViewBag.Error = errorMsg;
            return View();
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _apiClient.PostAsync("api/auth/logout", new { });
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}