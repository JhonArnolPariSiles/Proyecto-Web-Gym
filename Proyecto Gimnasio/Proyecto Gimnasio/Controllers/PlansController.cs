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
    public class PlansController : Controller
    {
        private readonly AppDbContext _context;

        public PlansController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Plans
        public async Task<IActionResult> Index()
        {
            return View(await _context.Planss.ToListAsync());
        }

        // GET: Plans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plans = await _context.Planss
                .FirstOrDefaultAsync(m => m.IdPlan == id);
            if (plans == null)
            {
                return NotFound();
            }

            return View(plans);
        }

        // GET: Plans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Plans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPlan,NamePlan,Price,Description,StartDate,EndDate")] Plans plans)
        {
            // --- INICIO VALIDACIONES AGREGADAS ---

            // 1. Validar Precio (Mayor o igual a 1)
            if (plans.Price < 1)
            {
                ModelState.AddModelError("Price", "El precio debe ser mayor o igual a 1.");
            }

            // 2. Validar Fecha de Inicio (No puede ser pasada)
            if (plans.StartDate.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("StartDate", "La fecha de inicio no puede ser anterior a hoy.");
            }

            // 3. Validar Duración (Mínimo 30 días)
            TimeSpan diferencia = plans.EndDate - plans.StartDate;
            if (diferencia.TotalDays < 30)
            {
                ModelState.AddModelError("EndDate", "El plan debe durar al menos 30 días.");
            }

            // --- FIN VALIDACIONES AGREGADAS ---

            if (ModelState.IsValid)
            {
                _context.Add(plans);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plans);
        }

        // GET: Plans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plans = await _context.Planss.FindAsync(id);
            if (plans == null)
            {
                return NotFound();
            }
            return View(plans);
        }

        // POST: Plans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPlan,NamePlan,Price,Description,StartDate,EndDate")] Plans plans)
        {
            if (id != plans.IdPlan)
            {
                return NotFound();
            }

            // --- INICIO VALIDACIONES AGREGADAS (Mismas que en Create) ---

            if (plans.Price < 1)
            {
                ModelState.AddModelError("Price", "El precio debe ser mayor o igual a 1.");
            }

            if (plans.StartDate.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("StartDate", "La fecha de inicio no puede ser anterior a hoy.");
            }

            TimeSpan diferencia = plans.EndDate - plans.StartDate;
            if (diferencia.TotalDays < 30)
            {
                ModelState.AddModelError("EndDate", "El plan debe durar al menos 30 días.");
            }

            // --- FIN VALIDACIONES AGREGADAS ---

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plans);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlansExists(plans.IdPlan))
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
            return View(plans);
        }

        // GET: Plans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plans = await _context.Planss
                .FirstOrDefaultAsync(m => m.IdPlan == id);
            if (plans == null)
            {
                return NotFound();
            }

            return View(plans);
        }

        // POST: Plans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plans = await _context.Planss.FindAsync(id);
            if (plans != null)
            {
                _context.Planss.Remove(plans);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlansExists(int id)
        {
            return _context.Planss.Any(e => e.IdPlan == id);
        }
    }
}