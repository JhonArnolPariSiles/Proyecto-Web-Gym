using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Gimnasio.Data;
using Proyecto_Gimnasio.Models;

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
			if (!string.IsNullOrEmpty(searchName))
			{
				personsQuery = personsQuery
					.Where(p => (p.Name + " " + p.LasName).Contains(searchName));
			}

			var persons = await personsQuery.ToListAsync();

			ViewBag.Persons = persons;
			ViewBag.SelectedPersonId = SelectedPersonId;
			ViewBag.SearchName = searchName;

			return View(plans);
		}


		[HttpPost]
		public IActionResult SelectPerson(int personId)
		{
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

			CartPlans.Add(id);
			TempData["msg"] = "Plan agregado al carrito.";
			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> Cart()
		{
			var plans = await _context.Planss
				.Where(p => CartPlans.Contains(p.IdPlan))
				.ToListAsync();

			ViewBag.SelectedPersonId = SelectedPersonId;
			return View(plans);
		}

		[HttpPost]
		public IActionResult Remove(int id)
		{
			if (CartPlans.Contains(id))
				CartPlans.Remove(id);

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

			ViewBag.Person = await _context.Persons.FindAsync(SelectedPersonId);
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

			var plans = await _context.Planss
				.Where(p => CartPlans.Contains(p.IdPlan))
				.ToListAsync();

			var sale = new Sale
			{
				IdPerson = SelectedPersonId,
				Total = plans.Sum(p => p.Price),
				DateSale = DateTime.Now,
				UserId = 1,
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
				.Where(p => p.User.Rol == "Customer") 
				.Include(p => p.Sales)
					.ThenInclude(s => s.saleDetailsPlans)
						.ThenInclude(sd => sd.Plans)
				.ToListAsync();

			return View(persons);
		}
	}

}
