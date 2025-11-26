using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Gimnasio.Models
{
	public class SaleDetailsPlans
	{
		[Key]
		public int Id { get; set; }

		[Range(1, int.MaxValue)]
		public int Quantity { get; set; }

		[Range(0, double.MaxValue)]
		public double TotalPrice { get; set; }

		// 1-m sale y details
		public int IdSale { get; set; }

		[ForeignKey("IdSale")]
		public Sale Sale { get; set; }

		// 1-m plam
		public int IdPlan { get; set; }

		[ForeignKey("IdPlan")]
		public Plans Plans { get; set; }
	}
}
