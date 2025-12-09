using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Gimnasio.Models
{
	public class SaleProduct
	{
		[Key]
		public int IdSaleProduct { get; set; }


		[Required(ErrorMessage = "Total is required")]
		[Display(Name = "Total ")]
		[DataType(DataType.Currency)]
		[Range(0.01, double.MaxValue, ErrorMessage = "Total  must be greater than 0")]
		public double Total { get; set; }

		[Required(ErrorMessage = "Date Sale date is required")]
		[Display(Name = "DATE SALE")]
		[DataType(DataType.Date)]
		public DateTime DateSale { get; set; }

		// FK a persona
		public int IdPerson { get; set; }

		[ForeignKey("IdPerson")]
		public Person? Person { get; set; }

		public ICollection<SaleDetailsProducts>? saleDetailsProducts { get; set; }
	}
}
