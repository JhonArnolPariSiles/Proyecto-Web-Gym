using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Gimnasio.Data;
using Proyecto_Gimnasio.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto_Gimnasio.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Users
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.Include(u => u.Person).ToListAsync());
        }

        // GET: Users/Details/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Create()
        {
            var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            if (currentUserRole == "Employee")
            {
                ViewBag.IsEmployee = true;
            }
            else
            {
                ViewBag.IsEmployee = false;
            }

            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create(
            string Email, string Password, string Rol,
            string Name, string LasName, string SecondLastName,
            DateTime DateBirthay, int Cnit, char Gender)
        {
            try
            {
                var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

                // Validación: Employee solo puede crear Customer
                if (currentUserRole == "Employee" && Rol != "Customer")
                {
                    ModelState.AddModelError("Rol", "Solo puedes crear usuarios con rol Customer");
                    ViewBag.IsEmployee = true;
                    ViewBag.Error = "No tienes permiso para crear este tipo de usuario";
                    return View();
                }

                // Validación: Email único
                if (await _context.Users.AnyAsync(u => u.Email == Email))
                {
                    ModelState.AddModelError("Email", "Este email ya está registrado");
                    ViewBag.IsEmployee = currentUserRole == "Employee";
                    ViewBag.Error = "Email ya existe";
                    return View();
                }

                // Validación: CNIT único
                if (await _context.Persons.AnyAsync(p => p.Cnit == Cnit))
                {
                    ModelState.AddModelError("Cnit", "Este CNIT ya está registrado");
                    ViewBag.IsEmployee = currentUserRole == "Employee";
                    ViewBag.Error = "CNIT ya existe";
                    return View();
                }

                // Validación: Fecha de nacimiento - Mayor de 18 años
                var age = DateTime.Now.Year - DateBirthay.Year;
                if (DateBirthay > DateTime.Now.AddYears(-age)) age--;

                if (age < 18)
                {
                    ModelState.AddModelError("DateBirthay", "Debe ser mayor de 18 años");
                    ViewBag.IsEmployee = currentUserRole == "Employee";
                    ViewBag.Error = "El usuario debe ser mayor de 18 años";
                    return View();
                }

                // Validación: Fecha no puede ser futura
                if (DateBirthay.Date > DateTime.Now.Date)
                {
                    ModelState.AddModelError("DateBirthay", "La fecha de nacimiento no puede ser futura");
                    ViewBag.IsEmployee = currentUserRole == "Employee";
                    ViewBag.Error = "La fecha de nacimiento no puede ser futura";
                    return View();
                }

                var user = new User
                {
                    Email = Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(Password),
                    Rol = Rol,
                    primarySession = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var person = new Person
                {
                    Name = Name,
                    LasName = LasName,
                    SecondLastName = SecondLastName,
                    DateBirthay = DateBirthay,
                    Cnit = Cnit,
                    Gender = Gender,
                    UserId = user.Id,
                    RegisterDate = DateTime.Now,
                    LastDate = DateTime.Now,
                    Status = true
                };

                _context.Persons.Add(person);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error: " + ex.Message;
                if (ex.InnerException != null)
                {
                    ViewBag.Error += " | Inner: " + ex.InnerException.Message;
                }

                var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
                ViewBag.IsEmployee = currentUserRole == "Employee";

                return View();
            }
        }

        // GET: Users/Edit/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                 .Include(u => u.Person)
                 .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
            if (currentUserRole == "Employee" && user.Rol != "Customer")
            {
                return Forbid();
            }

            // Pasar el rol actual para que no se pueda cambiar
            ViewBag.CurrentRol = user.Rol;
            ViewBag.IsEmployee = currentUserRole == "Employee";

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int id,
            string Email, string Password, bool primarySession,
            int IdPerson, string Name, string LasName, string SecondLastName,
            DateTime DateBirthay, int Cnit, char Gender, int UserId)
        {
            if (id != UserId)
            {
                return NotFound();
            }

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
                if (currentUserRole == "Employee" && user.Rol != "Customer")
                {
                    return Forbid();
                }

                // Validación: Email único (excluyendo el usuario actual)
                if (await _context.Users.AnyAsync(u => u.Email == Email && u.Id != id))
                {
                    ModelState.AddModelError("Email", "Este email ya está registrado");
                    ViewBag.Error = "Email ya existe";
                    ViewBag.CurrentRol = user.Rol;
                    ViewBag.IsEmployee = currentUserRole == "Employee";
                    var userWithPerson = await _context.Users
                        .Include(u => u.Person)
                        .FirstOrDefaultAsync(u => u.Id == id);
                    return View(userWithPerson);
                }

                // Validación: CNIT único (excluyendo el usuario actual)
                if (await _context.Persons.AnyAsync(p => p.Cnit == Cnit && p.IdPerson != IdPerson))
                {
                    ModelState.AddModelError("Cnit", "Este CNIT ya está registrado");
                    ViewBag.Error = "CNIT ya existe";
                    ViewBag.CurrentRol = user.Rol;
                    ViewBag.IsEmployee = currentUserRole == "Employee";
                    var userWithPerson = await _context.Users
                        .Include(u => u.Person)
                        .FirstOrDefaultAsync(u => u.Id == id);
                    return View(userWithPerson);
                }

                // Validación: Fecha de nacimiento - Mayor de 18 años
                var age = DateTime.Now.Year - DateBirthay.Year;
                if (DateBirthay > DateTime.Now.AddYears(-age)) age--;

                if (age < 18)
                {
                    ModelState.AddModelError("DateBirthay", "Debe ser mayor de 18 años");
                    ViewBag.Error = "El usuario debe ser mayor de 18 años";
                    ViewBag.CurrentRol = user.Rol;
                    ViewBag.IsEmployee = currentUserRole == "Employee";
                    var userWithPerson = await _context.Users
                        .Include(u => u.Person)
                        .FirstOrDefaultAsync(u => u.Id == id);
                    return View(userWithPerson);
                }

                // Validación: Fecha no puede ser futura
                if (DateBirthay.Date > DateTime.Now.Date)
                {
                    ModelState.AddModelError("DateBirthay", "La fecha de nacimiento no puede ser futura");
                    ViewBag.Error = "La fecha de nacimiento no puede ser futura";
                    ViewBag.CurrentRol = user.Rol;
                    ViewBag.IsEmployee = currentUserRole == "Employee";
                    var userWithPerson = await _context.Users
                        .Include(u => u.Person)
                        .FirstOrDefaultAsync(u => u.Id == id);
                    return View(userWithPerson);
                }

                user.Email = Email;
                if (!string.IsNullOrEmpty(Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(Password);
                }
                user.primarySession = primarySession;
                // NO SE CAMBIA EL ROL EN EDIT

                _context.Update(user);

                var person = await _context.Persons.FindAsync(IdPerson);
                if (person != null)
                {
                    person.Name = Name;
                    person.LasName = LasName;
                    person.SecondLastName = SecondLastName;
                    person.DateBirthay = DateBirthay;
                    person.Cnit = Cnit;
                    person.Gender = Gender;
                    person.LastDate = DateTime.Now;
                    person.Status = true;

                    _context.Update(person);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error: " + ex.Message;
                var userWithPerson = await _context.Users
                    .Include(u => u.Person)
                    .FirstOrDefaultAsync(u => u.Id == id);
                ViewBag.CurrentRol = userWithPerson?.Rol;
                var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
                ViewBag.IsEmployee = currentUserRole == "Employee";
                return View(userWithPerson);
            }
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                if (user.Person != null)
                {
                    _context.Persons.Remove(user.Person);
                }
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
