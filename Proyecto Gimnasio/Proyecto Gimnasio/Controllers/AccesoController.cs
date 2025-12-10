using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Proyecto_Gimnasio.Data;
using Proyecto_Gimnasio.Models;

namespace Proyecto_Gimnasio.Controllers
{
    public class AccesoController : Controller
    {
        private readonly AppDbContext _context;

        public AccesoController(AppDbContext context)
        {
            _context = context;
        }

        // 1. VISTA DE LOGIN (GET)
        public IActionResult Login()
        {
            
            if (User.Identity!.IsAuthenticated)
            {
                
                if (User.IsInRole("Customer"))
                {
                    return RedirectToAction("User", "Home");
                }

              
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // 2. PROCESAR EL LOGIN (POST)
        [HttpPost]
        public async Task<IActionResult> Login(string correo, string clave)
        {
           
            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == correo);

          
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(clave, usuario.Password))
            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }

         
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim("IdUsuario", usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

       

            if (usuario.Rol == "Customer")
            {
            
                return RedirectToAction("User", "Home");
            }
            else
            {
              
                return RedirectToAction("Index", "Home");
            }
        }

     




        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acceso");
        }

        public IActionResult LandingPage()
        {
            return View();
        }
    }
}