using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Gimnasio.Data;
using Proyecto_Gimnasio.Models;

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
            return View(await _context.Users.ToListAsync());
        }

        // GET: UserRegister/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: UserRegister/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // IMPORTANTE: Agregamos "Person" al Bind

        public IActionResult Create()
        {
            // Opcional: Si quieres inicializar la persona vacía para evitar errores null en la vista
            // return View(new User { Person = new Person() }); 
            return View();
        }

        public async Task<IActionResult> Create([Bind("Id,Email,Person")] User user)
        {
            // --- LÓGICA DE AUTO-COMPLETADO ---

            // 1. Asignar valores por defecto para USER
            user.Rol = "User";
            user.primarySession = true;
            // Contraseña temporal (porque es required)
            user.Password = "Temporal123$";

            // 2. Limpiar errores del ModelState de los campos que ocultamos
            ModelState.Remove("Rol");
            ModelState.Remove("Password");
            ModelState.Remove("primarySession");

            // NOTA: No necesitamos limpiar Name/Cnit/Gender porque ahora sí están en el formulario.

            // 3. Validación de Email duplicado
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Este correo ya está registrado.");
            }

            if (ModelState.IsValid)
            {
                // EF Core es inteligente: al guardar User, guardará también Person
                // y asignará automáticamente el UserId a la persona.
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si falla, retornamos a la vista para mostrar errores
            return View(user);
        }

        // GET: UserRegister/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: UserRegister/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Password,primarySession,Rol")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: UserRegister/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: UserRegister/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
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
