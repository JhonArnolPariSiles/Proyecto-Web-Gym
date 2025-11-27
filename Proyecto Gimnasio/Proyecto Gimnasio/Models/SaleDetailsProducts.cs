using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Gimnasio.Models
{
	public class SaleDetailsProducts
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Quantity is required")]
		[Display(Name = "Quantity")]
		[Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
		public int Quantity { get; set; }

		[Required(ErrorMessage = "Total Price is required")]
		[Display(Name = "Total Price")]
		[DataType(DataType.Currency)]
		[Range(0.01, double.MaxValue, ErrorMessage = "Total Price must be greater than 0")]
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
