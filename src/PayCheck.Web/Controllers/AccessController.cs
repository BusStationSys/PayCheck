using System.Security.Claims;
using ARVTech.DataAccess.DTOs.UniPayCheck;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace PayCheck.Web.Controllers
{
    public class AccessController : Controller
    {
        public IActionResult Login()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;

            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if ((loginDto.CpfEmailUsername == "andre" || loginDto.CpfEmailUsername == "arvtech@arvtech.com.br") &&
                (loginDto.Password == "123456"))
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier,loginDto.CpfEmailUsername),
                    new Claim("OtherProperty","OtherValue"),
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties authenticationProperties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = loginDto.KeepLoggedIn,
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(
                        claimsIdentity),
                    authenticationProperties);

                return RedirectToAction(
                    "Index",
                    "Home");
            }

            ViewData["ValidateMessage"] = "Usuário não encontrado.";

            return View();
        }
    }
}
