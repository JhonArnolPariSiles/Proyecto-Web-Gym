using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Gimnasio.Data;
using Proyecto_Gimnasio.Models;

namespace Proyecto_Gimnasio.Controllers
{
	public class CartItem
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
	}

	public class SaleProductController : Controller
	{
		private readonly AppDbContext _context;

		// Carrito y persona seleccionada (estático para demo)
		private static List<CartItem> CartProducts = new List<CartItem>();
		private static int SelectedPersonId = 0;

		public SaleProductController(AppDbContext context)
		{
			_context = context;
		}

		// GET: Index - Muestra productos y personas con historial
		[HttpGet]
		public async Task<IActionResult> Index(string searchName = "")
		{
			var products = await _context.Products
				.Include(p => p.Category)
				.ToListAsync();

			var query = _context.Persons.AsQueryable();

			if (!string.IsNullOrEmpty(searchName))
			{
				query = query.Where(p => (p.Name + " " + p.LasName).Contains(searchName));
			}

			var persons = await query
				.Include(p => p.SaleProducts)
					.ThenInclude(sp => sp.saleDetailsProducts)
						.ThenInclude(d => d.Product)
				.ToListAsync();

			ViewBag.Persons = persons;
			ViewBag.SelectedPersonId = SelectedPersonId;
			ViewBag.SearchName = searchName;

			return View(products);
		}

		// POST: Seleccionar persona
		[HttpPost]
		public IActionResult SelectPerson(int personId)
		{
			SelectedPersonId = personId;
			TempData["msg"] = "Persona seleccionada para la compra.";
			return RedirectToAction("Index");
		}

		// POST: Agregar al carrito
		[HttpPost]
		public async Task<IActionResult> Add(int id, int quantity)
		{
			if (SelectedPersonId == 0)
			{
				TempData["error"] = "Debe seleccionar una persona antes de agregar productos.";
				return RedirectToAction("Index");
			}

			if (quantity < 1)
			{
				TempData["error"] = "La cantidad debe ser al menos 1.";
				return RedirectToAction("Index");
			}

			var productDb = await _context.Products.FindAsync(id);
			if (productDb == null) return NotFound();

			var existingItem = CartProducts.FirstOrDefault(c => c.ProductId == id);
			int cantidadActual = existingItem?.Quantity ?? 0;

			if ((cantidadActual + quantity) > productDb.Stock)
			{
				TempData["error"] = $"Stock insuficiente. Solo hay {productDb.Stock} unidades disponibles.";
				return RedirectToAction("Index");
			}

			if (existingItem != null)
				existingItem.Quantity += quantity;
			else
				CartProducts.Add(new CartItem { ProductId = id, Quantity = quantity });

			TempData["msg"] = "Producto agregado al carrito.";
			return RedirectToAction("Index");
		}

		// GET: Carrito
		[HttpGet]
		public async Task<IActionResult> Cart()
		{
			var ids = CartProducts.Select(c => c.ProductId).ToList();
			var products = await _context.Products
				.Where(p => ids.Contains(p.IdProduct))
				.ToListAsync();

			ViewBag.CartItems = CartProducts;
			ViewBag.SelectedPersonId = SelectedPersonId;
			ViewBag.Person = SelectedPersonId > 0 ? await _context.Persons.FindAsync(SelectedPersonId) : null;

			return View(products);
		}

		// POST: Quitar del carrito
		[HttpPost]
		public IActionResult Remove(int id)
		{
			CartProducts.RemoveAll(c => c.ProductId == id);
			return RedirectToAction("Cart");
		}

		// GET: Checkout
		[HttpGet]
		public async Task<IActionResult> Checkout()
		{
			if (CartProducts.Count == 0 || SelectedPersonId == 0)
			{
				TempData["error"] = "Carrito vacío o persona no seleccionada.";
				return RedirectToAction("Index");
			}

			var ids = CartProducts.Select(c => c.ProductId).ToList();
			var products = await _context.Products
				.Where(p => ids.Contains(p.IdProduct))
				.ToListAsync();

			ViewBag.Person = await _context.Persons.FindAsync(SelectedPersonId);
			ViewBag.CartItems = CartProducts;

			return View(products);
		}

		// POST: Confirmar compra - AQUÍ ESTABA EL ERROR
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CheckoutConfirm()
		{
			if (CartProducts.Count == 0 || SelectedPersonId == 0)
			{
				TempData["error"] = "No hay productos para procesar.";
				return RedirectToAction("Index");
			}

			var ids = CartProducts.Select(c => c.ProductId).ToList();
			var productsDb = await _context.Products.Where(p => ids.Contains(p.IdProduct)).ToListAsync();

			double totalVenta = 0;
			foreach (var item in CartProducts)
			{
				var p = productsDb.FirstOrDefault(x => x.IdProduct == item.ProductId);
				if (p == null || p.Stock < item.Quantity)
				{
					TempData["error"] = $"Stock insuficiente para {p?.NameProduct ?? "el producto"}.";
					return RedirectToAction("Cart");
				}
				totalVenta += p.Price * item.Quantity;
			}

			// CREAR LA VENTA DE PRODUCTOS
			var saleProduct = new SaleProduct
			{
				IdPerson = SelectedPersonId,
				Total = totalVenta,
				DateSale = DateTime.Now
			};

			_context.SalesProducts.Add(saleProduct);

			// ESTO ERA LO QUE FALTABA: Guardar para generar el ID
			await _context.SaveChangesAsync();

			// AHORA SÍ podemos usar el ID generado
			foreach (var item in CartProducts)
			{
				var p = productsDb.First(x => x.IdProduct == item.ProductId);

				var detail = new SaleDetailsProducts
				{
					IdSaleProduct = saleProduct.IdSaleProduct,  // ID válido
					IdProduct = p.IdProduct,
					Quantity = item.Quantity,
					TotalPrice = p.Price * item.Quantity
				};

				_context.saleDetailsProducts.Add(detail);
				p.Stock -= item.Quantity; // Restar stock
			}

			await _context.SaveChangesAsync();

			// Limpiar
			CartProducts.Clear();
			SelectedPersonId = 0;
			TempData["ok"] = "¡Compra realizada exitosamente y stock actualizado!";

			return RedirectToAction("Index");
		}

		// GET: PersonsIndex - Historial completo
		[HttpGet]
		public async Task<IActionResult> PersonsIndex()
		{
			var persons = await _context.Persons
				.Include(p => p.SaleProducts)
					.ThenInclude(sp => sp.saleDetailsProducts)
						.ThenInclude(d => d.Product)
				.OrderBy(p => p.LasName)
				.ThenBy(p => p.Name)
				.ToListAsync();

			return View(persons);
		}
	}
}