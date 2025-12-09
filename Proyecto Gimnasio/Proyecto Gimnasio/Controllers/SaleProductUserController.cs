using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Gimnasio.Data;
using Proyecto_Gimnasio.Models;
using System.Security.Claims;

namespace Proyecto_Gimnasio.Controllers
{
	[Authorize]
	public class SaleProductUserController : Controller
	{
		private readonly AppDbContext _context;

		// Carrito con cantidad
		private static List<CartItem> CartProducts = new List<CartItem>();

		public class CartItem
		{
			public int ProductId { get; set; }
			public int Quantity { get; set; }
		}

		public SaleProductUserController(AppDbContext context)
		{
			_context = context;
		}

		// GET: Productos disponibles
		public async Task<IActionResult> Index()
		{
			if (!User.Identity!.IsAuthenticated)
				return RedirectToAction("Login", "Acceso");

			var products = await _context.Products
				.Include(p => p.Category)
				.ToListAsync();

			ViewBag.CartCount = CartProducts.Sum(c => c.Quantity);
			return View(products);
		}

		// POST: Agregar al carrito (con cantidad)
		[HttpPost]
		public async Task<IActionResult> Add(int id, int quantity = 1)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null)
			{
				TempData["error"] = "Producto no encontrado.";
				return RedirectToAction("Index");
			}

			if (quantity < 1) quantity = 1;
			if (quantity > product.Stock) quantity = product.Stock;

			var item = CartProducts.FirstOrDefault(c => c.ProductId == id);
			if (item != null)
			{
				if (item.Quantity + quantity > product.Stock)
				{
					TempData["error"] = "No hay suficiente stock.";
					return RedirectToAction("Index");
				}
				item.Quantity += quantity;
			}
			else
			{
				CartProducts.Add(new CartItem { ProductId = id, Quantity = quantity });
			}

			TempData["msg"] = $"Agregado: {quantity} x {product.NameProduct}";
			return RedirectToAction("Index");
		}

		// GET: Carrito
		public async Task<IActionResult> Cart()
		{
			var ids = CartProducts.Select(c => c.ProductId).ToList();
			var products = await _context.Products
				.Where(p => ids.Contains(p.IdProduct))
				.ToListAsync();

			ViewBag.CartItems = CartProducts;
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
		public async Task<IActionResult> Checkout()
		{
			if (!CartProducts.Any())
			{
				TempData["error"] = "El carrito está vacío.";
				return RedirectToAction("Index");
			}

			var userId = int.Parse(User.FindFirst("IdUsuario")!.Value);
			var user = await _context.Users
				.Include(u => u.Person)
				.FirstOrDefaultAsync(u => u.Id == userId);

			if (user?.Person == null)
			{
				TempData["error"] = "No se encontró tu perfil de cliente.";
				return RedirectToAction("Index");
			}

			var ids = CartProducts.Select(c => c.ProductId).ToList();
			var products = await _context.Products
				.Where(p => ids.Contains(p.IdProduct))
				.ToListAsync();

			ViewBag.Person = user.Person;
			ViewBag.CartItems = CartProducts;

			return View(products);
		}

		// POST: Confirmar compra
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CheckoutConfirm()
		{
			if (!CartProducts.Any())
			{
				TempData["error"] = "No hay productos en el carrito.";
				return RedirectToAction("Index");
			}

			var userId = int.Parse(User.FindFirst("IdUsuario")!.Value);
			var user = await _context.Users
				.Include(u => u.Person)
				.FirstOrDefaultAsync(u => u.Id == userId);

			if (user?.Person == null)
			{
				TempData["error"] = "Error con tu cuenta.";
				return RedirectToAction("Index");
			}

			var personId = user.Person.IdPerson;
			var productsInCart = await _context.Products
				.Where(p => CartProducts.Select(c => c.ProductId).Contains(p.IdProduct))
				.ToListAsync();

			// Validar stock final
			foreach (var item in CartProducts)
			{
				var p = productsInCart.FirstOrDefault(x => x.IdProduct == item.ProductId);
				if (p == null || p.Stock < item.Quantity)
				{
					TempData["error"] = $"Sin stock suficiente de {p?.NameProduct}";
					return RedirectToAction("Cart");
				}
			}

			double total = 0;
			foreach (var item in CartProducts)
			{
				var p = productsInCart.First(x => x.IdProduct == item.ProductId);
				total += p.Price * item.Quantity;
			}

			// CREAR LA VENTA CON SaleProduct
			var saleProduct = new SaleProduct
			{
				IdPerson = personId,
				Total = total,
				DateSale = DateTime.Now
			};

			_context.SalesProducts.Add(saleProduct);
			await _context.SaveChangesAsync(); // Genera el IdSaleProduct

			// Guardar detalles y restar stock
			foreach (var item in CartProducts)
			{
				var p = productsInCart.First(x => x.IdProduct == item.ProductId);

				_context.saleDetailsProducts.Add(new SaleDetailsProducts
				{
					IdSaleProduct = saleProduct.IdSaleProduct,
					IdProduct = p.IdProduct,
					Quantity = item.Quantity,
					TotalPrice = p.Price * item.Quantity
				});

				p.Stock -= item.Quantity;
			}

			await _context.SaveChangesAsync();

			CartProducts.Clear();
			TempData["ok"] = "¡Compra realizada con éxito! Gracias por tu compra.";
			return RedirectToAction("Index");
		}

		// MOSTRAR MIS COMPRAS (esto era lo que fallaba)
		public async Task<IActionResult> MyPurchases()
		{
			var userId = int.Parse(User.FindFirst("IdUsuario")!.Value);
			var personId = await _context.Users
				.Where(u => u.Id == userId)
				.Select(u => u.Person!.IdPerson)
				.FirstOrDefaultAsync();

			if (personId == 0)
			{
				TempData["error"] = "No se encontró tu perfil.";
				return View(new List<SaleProduct>());
			}

			var purchases = await _context.SalesProducts
				.Include(sp => sp.saleDetailsProducts)
					.ThenInclude(d => d.Product)
				.Where(sp => sp.IdPerson == personId)
				.OrderByDescending(sp => sp.DateSale)
				.ToListAsync();

			return View(purchases);
		}
	}
}