using Microsoft.AspNetCore.Mvc;
using DigitalExam.Data;
using DigitalExam.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DigitalExam.Controllers;

public class AuthController : Controller
{
    private readonly DigitalExamContext _context;

    private readonly string authtype = "Cookies";

    public AuthController(DigitalExamContext context) => _context = context;

    [HttpGet]
    public IActionResult LogUp()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LogUp(User user)
    {
        if (await _context.Users.FirstOrDefaultAsync(p => p.Username == user.Username) != null)
        {
            ViewData["Error"] = "User is registered";
            return View();
        }

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Username) };
        var claimsIdentity = new ClaimsIdentity(claims, authtype);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult LogIn()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LogIn(User user)
    {
        var found_user = await _context.Users.FirstOrDefaultAsync(p => p.Username == user.Username);
        if (found_user == null)
        {
            ViewData["Error"] = "User is unauthorized";
            return View();
        }

        if (found_user.Password != user.Password)
        {
            ViewData["Error"] = "Uncorrected password";
            return View();
        }

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, found_user.Username) };
        var claimsIdentity = new ClaimsIdentity(claims, authtype);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }
}
