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
    public class CustomerPersonController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerPersonController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CustomerPerson
        public async Task<IActionResult> Index()
        {
            var customers = await _context.Users
                .Include(u => u.Person)
                .Where(u => u.Rol == "Customer")
                .ToListAsync();
            return View(customers);
        }

        // GET: CustomerPerson/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(m => m.Id == id && m.Rol == "Customer");
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: CustomerPerson/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CustomerPerson/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string Email, string Password,
            string Name, string LasName, string SecondLastName,
            DateTime DateBirthay, int Cnit, char Gender)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == Email))
                {
                    ModelState.AddModelError("Email", "Este email ya está registrado");
                    ViewBag.Error = "Email ya existe";
                    return View();
                }

                var user = new User
                {
                    Email = Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(Password),
                    Rol = "Customer",
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
                return View();
            }
        }

        // GET: CustomerPerson/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Customer");
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: CustomerPerson/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                if (user == null || user.Rol != "Customer")
                {
                    return NotFound();
                }

                user.Email = Email;
                if (!string.IsNullOrEmpty(Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(Password);
                }
                user.primarySession = primarySession;
                user.Rol = "Customer";

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

        // GET: CustomerPerson/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(m => m.Id == id && m.Rol == "Customer");
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: CustomerPerson/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Customer");
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
            return _context.Users.Any(e => e.Id == id && e.Rol == "Customer");
        }
    }
}
