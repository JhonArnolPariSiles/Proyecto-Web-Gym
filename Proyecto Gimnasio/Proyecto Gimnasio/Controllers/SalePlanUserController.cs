using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Gimnasio.Data;
using Proyecto_Gimnasio.Models;
using System.Security.Claims;

namespace Proyecto_Gimnasio.Controllers
{
	[Authorize]
	public class SalePlanUserController : Controller
	{
		private readonly AppDbContext _context;

		private static List<int> CartPlans = new List<int>();

		public SalePlanUserController(AppDbContext context)
		{
			_context = context;
		}

		
		public async Task<IActionResult> Index()
		{
			var plans = await _context.Planss.ToListAsync();

			ViewBag.CartPlans = CartPlans;
			ViewBag.CartCount = CartPlans.Count;

		
			ViewBag.CurrentDate = DateTime.Now;

			return View(plans);
		}

	
		[HttpPost]
		public async Task<IActionResult> Add(int id)
		{
			var plan = await _context.Planss.FindAsync(id);
			if (plan == null)
			{
				TempData["error"] = "Plan no encontrado.";
				return RedirectToAction("Index");
			}

			if (CartPlans.Contains(id))
			{
				TempData["msg"] = "Este plan ya está en tu carrito.";
			}
			else
			{
				CartPlans.Add(id);
				TempData["ok"] = $"{plan.NamePlan} agregado al carrito.";
			}

			return RedirectToAction("Index");
		}

	
		public async Task<IActionResult> Cart()
		{
			var plans = await _context.Planss
				.Where(p => CartPlans.Contains(p.IdPlan))
				.ToListAsync();

			ViewBag.CartPlans = CartPlans;
			return View(plans);
		}

		
		[HttpPost]
		public IActionResult Remove(int id)
		{
			CartPlans.Remove(id);
			TempData["msg"] = "Plan eliminado del carrito.";
			return RedirectToAction("Cart");
		}

		// GET: Checkout
		public async Task<IActionResult> Checkout()
		{
			if (!CartPlans.Any())
			{
				TempData["error"] = "El carrito está vacío.";
				return RedirectToAction("Index");
			}

			var userId = int.Parse(User.FindFirst("IdUsuario")!.Value);
			var person = await _context.Users
				.Include(u => u.Person)
				.Where(u => u.Id == userId)
				.Select(u => u.Person)
				.FirstOrDefaultAsync();

			if (person == null)
			{
				TempData["error"] = "No se encontró tu perfil.";
				return RedirectToAction("Index");
			}

			var plans = await _context.Planss
				.Where(p => CartPlans.Contains(p.IdPlan))
				.ToListAsync();

			ViewBag.Person = person;
			ViewBag.CartPlans = CartPlans;
			ViewBag.Total = plans.Sum(p => p.Price);

			return View(plans);
		}

		// POST: Confirmar compra
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CheckoutConfirm()
		{
			if (!CartPlans.Any())
			{
				TempData["error"] = "No hay planes en el carrito.";
				return RedirectToAction("Index");
			}

			var userId = int.Parse(User.FindFirst("IdUsuario")!.Value);
			var person = await _context.Users
				.Include(u => u.Person)
				.Where(u => u.Id == userId)
				.Select(u => u.Person)
				.FirstOrDefaultAsync();

			if (person == null) return RedirectToAction("Index");

			var plans = await _context.Planss
				.Where(p => CartPlans.Contains(p.IdPlan))
				.ToListAsync();

			double total = plans.Sum(p => p.Price);

			var sale = new Sale
			{
				IdPerson = person.IdPerson,
				Total = total,
				DateSale = DateTime.Now,
				UserId = userId,
				RegisterDate = DateTime.Now,
				Status = true
			};

			_context.Sales.Add(sale);
			await _context.SaveChangesAsync();

			foreach (var plan in plans)
			{
				_context.saleDetailsPlans.Add(new SaleDetailsPlans
				{
					IdSale = sale.IdSale,
					IdPlan = plan.IdPlan,
					Quantity = 1,
					TotalPrice = plan.Price
				});
			}

			await _context.SaveChangesAsync();

			CartPlans.Clear();
			TempData["ok"] = "¡Compra de plan(es) realizada con éxito!";
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> MyPlans()
		{
			var userId = int.Parse(User.FindFirst("IdUsuario")!.Value);
			var personId = await _context.Users
				.Where(u => u.Id == userId)
				.Select(u => u.Person!.IdPerson)
				.FirstOrDefaultAsync();

			var sales = await _context.Sales
				.Include(s => s.saleDetailsPlans)
					.ThenInclude(d => d.Plans)
				.Where(s => s.IdPerson == personId)
				.OrderByDescending(s => s.DateSale)
				.ToListAsync();

			return View(sales);
		}
	}
}