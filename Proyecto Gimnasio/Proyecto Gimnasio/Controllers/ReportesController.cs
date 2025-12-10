using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Gimnasio.Data;
using Proyecto_Gimnasio.Models.ViewModels;

namespace Proyecto_Gimnasio.Controllers
{
	public class ReportesController : Controller
	{
		private readonly AppDbContext _context;

		public ReportesController(AppDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> ReporteVentasProductos(int? year)
		{
			var query = _context.SalesProducts
				.Include(sp => sp.saleDetailsProducts)
				.SelectMany(sp => sp.saleDetailsProducts,
					(sp, sdp) => new
					{
						Year = sp.DateSale.Year,
						Month = sp.DateSale.Month,
						TotalPrice = sdp.TotalPrice,
						Quantity = sdp.Quantity
					})
				.GroupBy(x => new { x.Year, x.Month })
				.Select(g => new ReporteVentasViewModel
				{
					Year = g.Key.Year,
					Month = g.Key.Month,
					TotalSales = g.Sum(x => x.TotalPrice),
					TotalQuantity = g.Sum(x => x.Quantity)
				})
				.OrderBy(x => x.Year)
				.ThenBy(x => x.Month);

			if (year.HasValue)
			{
				var filteredQuery = query.Where(x => x.Year == year.Value);
				ViewBag.SelectedYear = year.Value;
				return View(await filteredQuery.ToListAsync());
			}

			return View(await query.ToListAsync());
		}

		// Reporte de ventas de planes por mes
		public async Task<IActionResult> ReporteVentasPlanes(int? year)
		{
			var query = _context.Sales
				.Include(s => s.saleDetailsPlans)
				.SelectMany(s => s.saleDetailsPlans,
					(s, sdp) => new
					{
						Year = s.DateSale.Year,
						Month = s.DateSale.Month,
						TotalPrice = sdp.TotalPrice,
						Quantity = sdp.Quantity
					})
				.GroupBy(x => new { x.Year, x.Month })
				.Select(g => new ReporteVentasViewModel
				{
					Year = g.Key.Year,
					Month = g.Key.Month,
					TotalSales = g.Sum(x => x.TotalPrice),
					TotalQuantity = g.Sum(x => x.Quantity)
				})
				.OrderBy(x => x.Year)
				.ThenBy(x => x.Month);

			// Filtrar por año si se especifica
			if (year.HasValue)
			{
				var filteredQuery = query.Where(x => x.Year == year.Value);
				ViewBag.SelectedYear = year.Value;
				return View(await filteredQuery.ToListAsync());
			}

			return View(await query.ToListAsync());
		}

		

		// Método para obtener los años disponibles (para filtros)
		public async Task<JsonResult> GetAvailableYears()
		{
			var yearsProductos = await _context.SalesProducts
				.Select(sp => sp.DateSale.Year)
				.Distinct()
				.ToListAsync();

			var yearsPlanes = await _context.Sales
				.Select(s => s.DateSale.Year)
				.Distinct()
				.ToListAsync();

			var allYears = yearsProductos
				.Concat(yearsPlanes)
				.Distinct()
				.OrderByDescending(y => y)
				.ToList();

			return Json(allYears);
		}

	}
}