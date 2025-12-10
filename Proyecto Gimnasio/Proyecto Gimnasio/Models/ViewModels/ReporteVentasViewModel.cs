using System.ComponentModel.DataAnnotations;

namespace Proyecto_Gimnasio.Models.ViewModels
{
	public class ReporteVentasViewModel
	{
		[Display(Name = "Año")]
		public int Year { get; set; }

		[Display(Name = "Mes")]
		public int Month { get; set; }

		[Display(Name = "Total Ventas")]
		[DataType(DataType.Currency)]
		public double TotalSales { get; set; }

		[Display(Name = "Cantidad Total")]
		public int TotalQuantity { get; set; }

		// Propiedad calculada para mostrar el nombre del mes
		public string MonthName
		{
			get
			{
				return new DateTime(Year, Month, 1).ToString("MMMM");
			}
		}
	}
}
