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

        //if (ViewData["GuidUsuario"] is null)
        //{
        //    ViewData["GuidUsuario"] = TempData["GuidUsuario"];
        //}

        //if (ViewData["NomeColaborador"] is null)
        //{
        //    ViewData["NomeColaborador"] = TempData["NomeColaborador"];
        //}

        //ClaimsPrincipal claimsPrincipal = HttpContext.User;

        //if (claimsPrincipal.Identity.IsAuthenticated)
        //{

        //}

        //    ViewData["GuidUsuario"] = DateTime.Now;  //HttpContext.User.Claims.First(c => c.Type == nameof(UsuarioResponse.Guid));

        //TempData.Remove("GuidUsuario");
        //TempData.Remove("GuidColaborador");
        //TempData.Remove("NomeColaborador");
        //TempData.Remove("Username");

        //TempData["GuidUsuario"] = claimsPrincipal.Identity.Name;
        //TempData["GuidColaborador"] = usuarioResponse.Colaborador.Guid;
        //TempData["NomeColaborador"] = usuarioResponse.Colaborador.Nome;
        //TempData["Username"] = usuarioResponse.Username;

        //string x = TempData["GuidUsuario"].ToString();
    }

    public IActionResult Index()
    {
        ClaimsPrincipal claimsPrincipal = HttpContext.User;

        if (claimsPrincipal.Identity.IsAuthenticated)
        {
            ViewData["GuidUsuario"] = HttpContext.User.Claims.First(c => c.Type == nameof(UsuarioResponse.Guid)).Value;
            ViewData["Username"] = HttpContext.User.Claims.First(c => c.Type == nameof(UsuarioResponse.Username)).Value;
            ViewData["NomeColaborador"] = HttpContext.User.Claims.First(c => c.Type == nameof(UsuarioResponse.Colaborador.Nome)).Value;
            ViewData["Email"] = HttpContext.User.Claims.First(c => c.Type == nameof(UsuarioResponse.Email)).Value;
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