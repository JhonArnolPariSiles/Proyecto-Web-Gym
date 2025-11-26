using System.ComponentModel.DataAnnotations;

namespace Proyecto_Gimnasio.Models
{
	public class Sale
	{
		[Key]
		public int IdSale { get; set; }

		[Range(0, double.MaxValue)]
		public double Total { get; set; }

		[DataType(DataType.Date)]
		public DateTime DateSale { get; set; }
		//m-m sale y productos 
		public ICollection<SaleDetailsProducts> SaleDetailsProducts { get; set; }

	}
}
