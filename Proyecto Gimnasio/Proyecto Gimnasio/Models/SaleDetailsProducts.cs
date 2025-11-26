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

		// 1-m sale 
		public int IdSale { get; set; }

		[ForeignKey("IdSale")]
		public Sale Sale { get; set; }

		// 1-m product
		public int IdProduct { get; set; }

		[ForeignKey("IdProduct")]
		public Product Product { get; set; }
	}
}
