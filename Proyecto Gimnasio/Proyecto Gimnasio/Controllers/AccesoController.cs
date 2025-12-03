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

  
        public IActionResult Login()
        {
            // Si ya está logueado, lo mandamos al menú principal
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // 2. Procesar el Login
        [HttpPost]
        public async Task<IActionResult> Login(string correo, string clave)
        {
            // Buscamos el usuario por Email en tu base de datos
            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == correo);

            // Verificamos si existe y si la contraseña coincide 
            // Escribe esto tal cual, con las 3 partes: BCrypt.Net.BCrypt
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(clave, usuario.Password))
            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }

            // CREAR LA COOKIE
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim("IdUsuario", usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol) 
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Redirigir al Home
            return RedirectToAction("Index", "Home");
        }

        // 3. Cerrar Sesión
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