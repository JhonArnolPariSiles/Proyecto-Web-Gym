using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Gimnasio.Data;
using Proyecto_Gimnasio.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Proyecto_Gimnasio.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.Include(u => u.Person).ToListAsync());
        }

        // GET: Users/Details/5
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
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(new[] { "Admin", "Customer", "Employee" });
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string Email, string Password, string Rol,
            string Name, string LasName, string SecondLastName,
            DateTime DateBirthay, int Cnit, char Gender)
        {
            try
            {
             
                if (await _context.Users.AnyAsync(u => u.Email == Email))
                {
                    ModelState.AddModelError("Email", "Este email ya está registrado");
                    ViewBag.Roles = new SelectList(new[] { "Admin", "Customer", "Employee" });
                    ViewBag.Error = "Email ya existe";
                    return View();
                }

            
                var user = new User
                {
                    Email = Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(Password),
                    Rol = Rol,
                    primarySession = false
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
                ViewBag.Roles = new SelectList(new[] { "Admin", "Customer", "Employee" });
                return View();
            }
        }

        // GET: Users/Edit/5
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
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            string Email, string Password, bool primarySession, string Rol,
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

                user.Email = Email;
                if (!string.IsNullOrEmpty(Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(Password);
                }
                user.primarySession = primarySession;
                user.Rol = Rol;

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
                return View(userWithPerson);
            }
        }

        // GET: Users/Delete/5
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