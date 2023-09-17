namespace PayCheck.Web.Controllers;

using System.Diagnostics;
using System.Security.Claims;
using ARVTech.DataAccess.DTOs.UniPayCheck;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayCheck.Web.Models;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ClaimsPrincipal claimsPrincipal = HttpContext.User;

        if (claimsPrincipal.Identity.IsAuthenticated)
        {
            TempData["GuidUsuario"] = HttpContext.User.Claims.First(c => c.Type == $"{nameof(UsuarioResponseDto.Guid)}Usuario").Value;

            TempData["Username"] = HttpContext.User.Claims.First(c => c.Type == nameof(UsuarioResponseDto.Username)).Value;

            TempData["GuidColaborador"] = HttpContext.User.Claims.First(c => c.Type == $"{nameof(UsuarioResponseDto.Colaborador.Guid)}Colaborador").Value;

            TempData["NomeColaborador"] = HttpContext.User.Claims.First(c => c.Type == $"{nameof(UsuarioResponseDto.Colaborador.Nome)}Colaborador").Value;

            TempData["EmailUsuario"] = HttpContext.User.Claims.First(c => c.Type == $"{nameof(UsuarioResponseDto.Email)}Usuario").Value;

            TempData.Keep();
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(
            "Login",
            "Access");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}