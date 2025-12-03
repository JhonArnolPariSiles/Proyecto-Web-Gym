using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Gimnasio.Data;
using Proyecto_Gimnasio.Models;

namespace Proyecto_Gimnasio.Controllers
{
	public class SaleProductController : Controller
	{
		private readonly AppDbContext _context;

		// Carrito de productos (Ids)
		private static List<int> CartProducts = new List<int>();

		// Persona seleccionada
		private static int SelectedPersonId = 0;

		public SaleProductController(AppDbContext context)
		{
			_context = context;
		}

		// Mostrar productos + buscar personas
		[HttpGet]
		public async Task<IActionResult> Index(string searchName = "")
		{
			var products = await _context.Products
				.Include(p => p.Category)
				.ToListAsync();

			var persons = string.IsNullOrEmpty(searchName)
				? await _context.Persons.ToListAsync()
				: await _context.Persons
					.Where(p => (p.Name + " " + p.LasName).Contains(searchName))
					.ToListAsync();

			ViewBag.Persons = persons;
			ViewBag.SelectedPersonId = SelectedPersonId;
			ViewBag.SearchName = searchName;

			return View(products);
		}

		// Seleccionar persona
		[HttpPost]
		public IActionResult SelectPerson(int personId)
		{
			SelectedPersonId = personId;
			TempData["msg"] = "Persona seleccionada para la compra.";
			return RedirectToAction("Index");
		}

		// Agregar producto al carrito
		[HttpPost]
		public IActionResult Add(int id)
		{
			if (SelectedPersonId == 0)
			{
				TempData["error"] = "Debe seleccionar una persona antes de agregar productos.";
				return RedirectToAction("Index");
			}

			CartProducts.Add(id);
			TempData["msg"] = "Producto agregado al carrito.";
			return RedirectToAction("Index");
		}

		// Mostrar carrito
		[HttpGet]
		public async Task<IActionResult> Cart()
		{
			var products = await _context.Products
				.Where(p => CartProducts.Contains(p.IdProduct))
				.ToListAsync();

			ViewBag.SelectedPersonId = SelectedPersonId;

			return View(products);
		}

		// Quitar producto
		[HttpPost]
		public IActionResult Remove(int id)
		{
			if (CartProducts.Contains(id))
				CartProducts.Remove(id);

			return RedirectToAction("Cart");
		}

		// Procesar compra
		[HttpGet]
		public async Task<IActionResult> Checkout()
		{
			if (CartProducts.Count == 0 || SelectedPersonId == 0)
			{
				TempData["error"] = "Carrito vacío o persona no seleccionada.";
				return RedirectToAction("Index");
			}

			var products = await _context.Products
				.Where(p => CartProducts.Contains(p.IdProduct))
				.ToListAsync();

			ViewBag.Person = await _context.Persons.FindAsync(SelectedPersonId);

			return View(products);
		}

		// Confirmar compra
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CheckoutConfirm()
		{
			if (CartProducts.Count == 0 || SelectedPersonId == 0)
			{
				TempData["error"] = "No hay productos para procesar.";
				return RedirectToAction("Index");
			}

			var products = await _context.Products
				.Where(p => CartProducts.Contains(p.IdProduct))
				.ToListAsync();

			// Crear venta
			var sale = new Sale
			{
				IdPerson = SelectedPersonId,
				Total = products.Sum(p => p.Price),
				DateSale = DateTime.Now,
				UserId = 1,
				RegisterDate = DateTime.Now,
				Status = true
			};

			_context.Sales.Add(sale);
			await _context.SaveChangesAsync();

			// Crear detalle por cada producto
			foreach (var product in products)
			{
				var detail = new SaleDetailsProducts
				{
					IdSale = sale.IdSale,
					IdProduct = product.IdProduct,
					Quantity = 1,
					TotalPrice = product.Price
				};

				_context.saleDetailsProducts.Add(detail);
			}

			await _context.SaveChangesAsync();

			// Reset carrito y persona
			CartProducts.Clear();
			SelectedPersonId = 0;

			TempData["ok"] = "Compra realizada exitosamente.";
			return RedirectToAction("Index");
		}

		// Personas y los productos que compraron
		[HttpGet]
		public async Task<IActionResult> PersonsIndex()
		{
			var persons = await _context.Persons
				.Include(p => p.Sales)
					.ThenInclude(s => s.SaleDetailsProducts)
						.ThenInclude(sd => sd.Product)
				.ToListAsync();

			return View(persons);
		}
	}
}
