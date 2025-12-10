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
            // Si el usuario ya inició sesión, revisamos su rol para redirigirlo
            if (User.Identity!.IsAuthenticated)
            {
                // Si es Cliente -> Va a su Perfil
                if (User.IsInRole("Customer"))
                {
                    return RedirectToAction("User", "Home");
                }

                // Si es Admin o Employee -> Va al Index principal
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // 2. PROCESAR EL LOGIN (POST)
        [HttpPost]
        public async Task<IActionResult> Login(string correo, string clave)
        {
            // Buscar usuario
            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == correo);

            // Validar usuario y contraseña
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(clave, usuario.Password))
            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }

            // CREAR LA COOKIE DE SESIÓN
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim("IdUsuario", usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // --- LÓGICA DE REDIRECCIÓN FINAL ---

            if (usuario.Rol == "Customer")
            {
                // Solo el cliente va a la vista "User"
                return RedirectToAction("User", "Home");
            }
            else
            {
                // AQUÍ ENTRAN "Admin" Y "Employee"
                // Ambos van al Dashboard principal (Index)
                return RedirectToAction("Index", "Home");
            }
        }

        // 3. CERRAR SESIÓN
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