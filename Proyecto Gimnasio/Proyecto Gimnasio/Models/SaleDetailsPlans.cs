using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Gimnasio.Models
{
	public class SaleDetailsPlans
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


		public int IdSale { get; set; }

		[ForeignKey("IdSale")]
		public Sale Sale { get; set; }

	
		public int IdPlan { get; set; }

		[ForeignKey("IdPlan")]
		public Plans Plans { get; set; }
	}
}
