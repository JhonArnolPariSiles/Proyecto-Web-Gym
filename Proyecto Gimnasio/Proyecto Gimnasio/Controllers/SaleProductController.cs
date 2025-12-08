using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Gimnasio.Data;
using Proyecto_Gimnasio.Models;

namespace Proyecto_Gimnasio.Controllers
{
    // [NUEVO] Clase auxiliar para guardar ID y Cantidad en el carrito
    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class SaleProductController : Controller
    {
        private readonly AppDbContext _context;

        // [MODIFICADO] Ahora la lista es de tipo CartItem, no de int.
        private static List<CartItem> CartProducts = new List<CartItem>();

        // Persona seleccionada
        private static int SelectedPersonId = 0;

        public SaleProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Index (Mostrar productos + buscar personas)
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

        // POST: SelectPerson
        [HttpPost]
        public IActionResult SelectPerson(int personId)
        {
            SelectedPersonId = personId;
            TempData["msg"] = "Persona seleccionada para la compra.";
            return RedirectToAction("Index");
        }

        // POST: Add (Agregar producto al carrito con Cantidad)
        [HttpPost]
        public async Task<IActionResult> Add(int id, int quantity)
        {
            // 1. Validar que se haya seleccionado una persona
            if (SelectedPersonId == 0)
            {
                TempData["error"] = "Debe seleccionar una persona antes de agregar productos.";
                return RedirectToAction("Index");
            }

            // 2. Validar que la cantidad sea positiva
            if (quantity < 1)
            {
                TempData["error"] = "La cantidad debe ser al menos 1.";
                return RedirectToAction("Index");
            }

            // 3. Verificar Stock real en la Base de Datos
            var productDb = await _context.Products.FindAsync(id);
            if (productDb == null) return NotFound();

            // Revisar si ya está en el carrito para sumar cantidades
            var existingItem = CartProducts.FirstOrDefault(c => c.ProductId == id);
            int cantidadActualEnCarrito = existingItem != null ? existingItem.Quantity : 0;

            // [VALIDACIÓN DE STOCK]
            if ((cantidadActualEnCarrito + quantity) > productDb.Stock)
            {
                TempData["error"] = $"Stock insuficiente. Tienes {cantidadActualEnCarrito} en carrito y quieres agregar {quantity}, pero solo hay {productDb.Stock} en stock.";
                return RedirectToAction("Index");
            }

            // 4. Agregar o actualizar carrito
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                CartProducts.Add(new CartItem { ProductId = id, Quantity = quantity });
            }

            TempData["msg"] = "Producto agregado al carrito.";
            return RedirectToAction("Index");
        }

        // GET: Cart (Mostrar carrito)
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            // Obtenemos los IDs para buscar detalles en BD
            var ids = CartProducts.Select(c => c.ProductId).ToList();

            var products = await _context.Products
                .Where(p => ids.Contains(p.IdProduct))
                .ToListAsync();

            // [IMPORTANTE] Pasamos el carrito (con cantidades) a la vista
            ViewBag.CartItems = CartProducts;
            ViewBag.SelectedPersonId = SelectedPersonId;

            return View(products);
        }

        // POST: Remove (Quitar producto)
        [HttpPost]
        public IActionResult Remove(int id)
        {
            var item = CartProducts.FirstOrDefault(c => c.ProductId == id);
            if (item != null)
                CartProducts.Remove(item);

            return RedirectToAction("Cart");
        }

        // GET: Checkout (Resumen antes de pagar)
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
            ViewBag.CartItems = CartProducts; // Para mostrar totales

            return View(products);
        }

        // POST: CheckoutConfirm (Confirmar compra y RESTAR STOCK)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutConfirm()
        {
            if (CartProducts.Count == 0 || SelectedPersonId == 0)
            {
                TempData["error"] = "No hay productos para procesar.";
                return RedirectToAction("Index");
            }

            // 1. Traer productos de la BD
            var ids = CartProducts.Select(c => c.ProductId).ToList();
            var productsDb = await _context.Products
                .Where(p => ids.Contains(p.IdProduct))
                .ToListAsync();

            // 2. Calcular total y validar stock final
            double totalVenta = 0;
            foreach (var item in CartProducts)
            {
                var p = productsDb.FirstOrDefault(x => x.IdProduct == item.ProductId);
                if (p != null)
                {
                    // Seguridad extra: Verificar stock de nuevo
                    if (p.Stock < item.Quantity)
                    {
                        TempData["error"] = $"Stock insuficiente para {p.NameProduct} al procesar.";
                        return RedirectToAction("Cart");
                    }
                    totalVenta += p.Price * item.Quantity;
                }
            }

            // 3. Crear Venta
            var sale = new Sale
            {
                IdPerson = SelectedPersonId,
                Total = totalVenta,
                DateSale = DateTime.Now,
                UserId = 1, // ID del usuario logueado (ajustar si tienes autenticación)
                RegisterDate = DateTime.Now,
                Status = true
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            // 4. Crear Detalles y RESTAR STOCK
            foreach (var item in CartProducts)
            {
                var p = productsDb.FirstOrDefault(x => x.IdProduct == item.ProductId);
                if (p != null)
                {
                    // A. Guardar Detalle
                    var detail = new SaleDetailsProducts
                    {
                        IdSale = sale.IdSale,
                        IdProduct = p.IdProduct,
                        Quantity = item.Quantity,
                        TotalPrice = p.Price * item.Quantity
                    };
                    _context.saleDetailsProducts.Add(detail);

                    // B. [CAMBIO CLAVE] Restar Stock en la BD
                    p.Stock = p.Stock - item.Quantity;
                    _context.Update(p);
                }
            }

            await _context.SaveChangesAsync();

            // 5. Limpiar
            CartProducts.Clear();
            SelectedPersonId = 0;

            TempData["ok"] = "Compra realizada exitosamente y stock actualizado.";
            return RedirectToAction("Index");
        }

        // GET: PersonsIndex (Historial)
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