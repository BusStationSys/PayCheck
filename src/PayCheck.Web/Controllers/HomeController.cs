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
            ViewData["GuidUsuario"] = HttpContext.User.Claims.First(c => c.Type == nameof(UsuarioResponse.Guid)).Value;
            ViewData["Username"] = HttpContext.User.Claims.First(c => c.Type == nameof(UsuarioResponse.Username)).Value;
            ViewData["GuidColaborador"] = HttpContext.User.Claims.First(c => c.Type == nameof(UsuarioResponse.Colaborador.Guid)).Value;
            ViewData["NomeColaborador"] = HttpContext.User.Claims.First(c => c.Type == nameof(UsuarioResponse.Colaborador.Nome)).Value;
            ViewData["Email"] = HttpContext.User.Claims.First(c => c.Type == nameof(UsuarioResponse.Email)).Value;
        }

        //new Claim(nameof(usuarioResponse.Guid), usuarioResponse.Guid.ToString()),
        //                    new Claim(ClaimTypes.Name, usuarioResponse.Colaborador.Nome),
        //                    new Claim(nameof(usuarioResponse.Colaborador.Guid), guidColaborador),
        //                    new Claim(nameof(usuarioResponse.Colaborador.Nome), usuarioResponse.Colaborador.Nome),
        //                    new Claim(nameof(usuarioResponse.Username), usuarioResponse.Username),
        //                    new Claim(nameof(usuarioResponse.Email), usuarioResponse.Email),

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