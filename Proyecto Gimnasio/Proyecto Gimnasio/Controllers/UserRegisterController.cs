using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Gimnasio.Data;
using Proyecto_Gimnasio.Models;
using BCrypt.Net;

namespace Proyecto_Gimnasio.Controllers
{
    public class UserRegisterController : Controller
    {
        private readonly AppDbContext _context;

        public UserRegisterController(AppDbContext context)
        {
            _context = context;
        }

        // GET: UserRegister
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.Include(u => u.Person).ToListAsync());
        }

        // GET: UserRegister/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserRegister/Create
        // POST: UserRegister/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Password,Person")] User user)
        {
            try
            {
                // 1. VALIDAR DUPLICADOS
                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Este email ya está registrado");
                    return View(user);
                }

                if (user.Person != null)
                {
                    if (await _context.Persons.AnyAsync(p => p.Cnit == user.Person.Cnit))
                    {
                        ModelState.AddModelError("Person.Cnit", "Este CNIT ya está registrado");
                        return View(user);
                    }
                }

                // 2. CREAR Y GUARDAR EL USUARIO
                var newUser = new User
                {
                    Email = user.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    Rol = "Customer",
                    primarySession = true
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // 3. CREAR Y GUARDAR LA PERSONA
                if (user.Person != null)
                {
                    var newPerson = new Person
                    {
                        Name = user.Person.Name,
                        LasName = user.Person.LasName,
                        SecondLastName = user.Person.SecondLastName,
                        DateBirthay = user.Person.DateBirthay,
                        Cnit = user.Person.Cnit,
                        Gender = user.Person.Gender,
                        UserId = newUser.Id,
                        RegisterDate = DateTime.Now,
                        LastDate = DateTime.Now,
                        Status = true
                    };

                    _context.Persons.Add(newPerson);
                    await _context.SaveChangesAsync();
                }

                // --- CAMBIO AQUÍ: REDIRIGIR AL HOME ---
                return RedirectToAction("User", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error: " + ex.Message;
                if (ex.InnerException != null)
                {
                    ViewBag.Error += " | Inner: " + ex.InnerException.Message;
                }
                return View(user);
            }
        }
    }
}