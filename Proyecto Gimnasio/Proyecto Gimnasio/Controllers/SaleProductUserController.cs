using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Gimnasio.Data;
using Proyecto_Gimnasio.Models;
using System.Security.Claims;

namespace Proyecto_Gimnasio.Controllers
{
	public class SaleProductUserController : Controller
	{
		private readonly AppDbContext _context;
		private static List<int> CartProducts = new List<int>();

		public SaleProductUserController(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			if (!User.Identity!.IsAuthenticated)
				return RedirectToAction("Login", "Acceso");

			var products = await _context.Products.ToListAsync();
			return View(products);
		}

		[HttpPost]
		public IActionResult Add(int id)
		{
			CartProducts.Add(id);
			TempData["msg"] = "Producto agregado al carrito.";
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Cart()
		{
			var products = await _context.Products
				.Where(p => CartProducts.Contains(p.IdProduct))
				.ToListAsync();

			return View(products);
		}

		[HttpPost]
		public IActionResult Remove(int id)
		{
			if (CartProducts.Contains(id))
				CartProducts.Remove(id);

			return RedirectToAction("Cart");
		}

		public async Task<IActionResult> Checkout()
		{
			if (!CartProducts.Any())
			{
				TempData["error"] = "El carrito está vacío.";
				return RedirectToAction("Index");
			}

			var userId = int.Parse(User.Claims.First(c => c.Type == "IdUsuario").Value);

			// Traemos usuario con persona
			var user = await _context.Users
				.Include(u => u.Person)
				.FirstOrDefaultAsync(u => u.Id == userId);

			if (user?.Person == null)
			{
				TempData["error"] = "No se encontró la persona asociada al usuario.";
				return RedirectToAction("Index");
			}

			var products = await _context.Products
				.Where(p => CartProducts.Contains(p.IdProduct))
				.ToListAsync();

			ViewBag.Person = user.Person;

			return View(products);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CheckoutConfirm()
		{
			if (!CartProducts.Any())
			{
				TempData["error"] = "No hay productos en el carrito.";
				return RedirectToAction("Index");
			}

			var userId = int.Parse(User.Claims.First(c => c.Type == "IdUsuario").Value);

			var user = await _context.Users
				.Include(u => u.Person)
				.FirstOrDefaultAsync(u => u.Id == userId);

			if (user?.Person == null)
			{
				TempData["error"] = "No se encontró la persona asociada al usuario.";
				return RedirectToAction("Index");
			}

			var personId = user.Person.IdPerson;

			var products = await _context.Products
				.Where(p => CartProducts.Contains(p.IdProduct))
				.ToListAsync();

			var sale = new Sale
			{
				IdPerson = personId, 
				UserId = userId,
				Total = products.Sum(p => p.Price),
				DateSale = DateTime.Now,
				RegisterDate = DateTime.Now,
				Status = true
			};

			_context.Sales.Add(sale);
			await _context.SaveChangesAsync();

			foreach (var product in products)
			{
				_context.saleDetailsProducts.Add(new SaleDetailsProducts
				{
					IdSale = sale.IdSale,
					IdProduct = product.IdProduct,
					Quantity = 1,
					TotalPrice = product.Price
				});
			}

			await _context.SaveChangesAsync();

			CartProducts.Clear();

			TempData["ok"] = "Compra realizada con éxito.";
			return RedirectToAction("Index");
		}

		// Mostrar compras del usuario logueado
		public async Task<IActionResult> MyPurchases()
		{
			if (!User.Identity!.IsAuthenticated)
				return RedirectToAction("Login", "Acceso");

			var userId = int.Parse(User.Claims.First(c => c.Type == "IdUsuario").Value);

	
			var sales = await _context.Sales
				.Where(s => s.UserId == userId) 
				.Include(s => s.Person) 
				.Include(s => s.SaleDetailsProducts)
					.ThenInclude(sd => sd.Product) 
				.OrderByDescending(s => s.DateSale)
				.ToListAsync();

			return View(sales);
		}

	}
}
