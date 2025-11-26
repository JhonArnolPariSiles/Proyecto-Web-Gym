using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Gimnasio.Models
{
	public class SaleDetailsProducts
	{
		[Key]
		public int Id { get; set; }

		[Range(1, int.MaxValue)]
		public int Quantity { get; set; }

		[Range(0, double.MaxValue)]
		public double TotalPrice { get; set; }

		// Venta asociada
		public int IdSale { get; set; }
		public Sale Sale { get; set; }

		// Producto asociado
		public int IdProduct { get; set; }
		public Product Product { get; set; }

	}
}
