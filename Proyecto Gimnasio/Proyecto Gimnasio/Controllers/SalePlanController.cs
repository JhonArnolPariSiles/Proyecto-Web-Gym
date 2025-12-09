using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Gimnasio.Data;
using Proyecto_Gimnasio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Gimnasio.Controllers
{
    public class SalePlanController : Controller
    {
        private readonly AppDbContext _context;
        private static List<int> CartPlans = new List<int>();
        private static int SelectedPersonId = 0;

        public SalePlanController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string searchName = "")
        {
            var plans = await _context.Planss.ToListAsync();


            var personsQuery = _context.Persons
                .Include(p => p.User)
                .Where(p => p.User.Rol == "Customer");

            var persons = string.IsNullOrEmpty(searchName)
                ? await personsQuery.ToListAsync()
                : await personsQuery
                    .Where(p => (p.Name + " " + p.LasName + " " + p.SecondLastName).Contains(searchName))
                    .ToListAsync();

            ViewBag.Persons = persons;
            ViewBag.SelectedPersonId = SelectedPersonId;
            ViewBag.SearchName = searchName;

            return View(plans);
        }

        [HttpPost]
        public async Task<IActionResult> SelectPerson(int personId)
        {

            var person = await _context.Persons
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.IdPerson == personId);

            if (person == null)
            {
                TempData["error"] = "Persona no encontrada.";
                return RedirectToAction("Index");
            }

            if (person.User.Rol != "Customer")
            {
                TempData["error"] = "Solo se pueden seleccionar clientes (Customers).";
                return RedirectToAction("Index");
            }

            SelectedPersonId = personId;
            TempData["msg"] = "Persona seleccionada para la compra.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Add(int id)
        {
            if (SelectedPersonId == 0)
            {
                TempData["error"] = "Primero seleccione una persona.";
                return RedirectToAction("Index");
            }


            if (!CartPlans.Contains(id))
            {
                CartPlans.Add(id);
                TempData["msg"] = "Plan agregado al carrito.";
            }
            else
            {
                TempData["error"] = "Este plan ya está en el carrito.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var plans = await _context.Planss
                .Where(p => CartPlans.Contains(p.IdPlan))
                .ToListAsync();


            Person selectedPerson = null;
            if (SelectedPersonId > 0)
            {
                selectedPerson = await _context.Persons
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.IdPerson == SelectedPersonId);
            }

            ViewBag.SelectedPerson = selectedPerson;
            ViewBag.SelectedPersonId = SelectedPersonId;
            return View(plans);
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            if (CartPlans.Contains(id))
                CartPlans.Remove(id);

            TempData["msg"] = "Plan eliminado del carrito.";
            return RedirectToAction("Cart");
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            if (CartPlans.Count == 0 || SelectedPersonId == 0)
            {
                TempData["error"] = "El carrito está vacío o no se ha seleccionado persona.";
                return RedirectToAction("Index");
            }

            var plans = await _context.Planss
                .Where(p => CartPlans.Contains(p.IdPlan))
                .ToListAsync();


            var person = await _context.Persons
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.IdPerson == SelectedPersonId);

            ViewBag.Person = person;
            return View(plans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutConfirm()
        {
            if (CartPlans.Count == 0 || SelectedPersonId == 0)
            {
                TempData["error"] = "No hay planes o no se ha seleccionado persona.";
                return RedirectToAction("Index");
            }


            var person = await _context.Persons
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.IdPerson == SelectedPersonId);

            if (person == null || person.User.Rol != "Customer")
            {
                TempData["error"] = "La persona seleccionada no es válida.";
                CartPlans.Clear();
                SelectedPersonId = 0;
                return RedirectToAction("Index");
            }

            var plans = await _context.Planss
                .Where(p => CartPlans.Contains(p.IdPlan))
                .ToListAsync();


            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int userId = string.IsNullOrEmpty(currentUserId) ? 1 : int.Parse(currentUserId);

            var sale = new Sale
            {
                IdPerson = SelectedPersonId,
                Total = plans.Sum(p => p.Price),
                DateSale = DateTime.Now,
                UserId = userId,
                RegisterDate = DateTime.Now,
                Status = true
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            foreach (var plan in plans)
            {
                var detail = new SaleDetailsPlans
                {
                    IdSale = sale.IdSale,
                    IdPlan = plan.IdPlan,
                    Quantity = 1,
                    TotalPrice = plan.Price
                };
                _context.saleDetailsPlans.Add(detail);
            }

            await _context.SaveChangesAsync();

            CartPlans.Clear();
            SelectedPersonId = 0;

            TempData["ok"] = "Compra realizada con éxito.";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> PersonsIndex()
        {

            var persons = await _context.Persons
                .Include(p => p.User)
                .Include(p => p.Sales)
                    .ThenInclude(s => s.saleDetailsPlans)
                        .ThenInclude(sd => sd.Plans)
                .Where(p => p.User.Rol == "Customer")
                .ToListAsync();

            return View(persons);
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            CartPlans.Clear();
            SelectedPersonId = 0;
            TempData["msg"] = "Carrito limpiado.";
            return RedirectToAction("Index");
        }
    }
}
