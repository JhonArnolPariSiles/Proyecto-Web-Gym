using System.ComponentModel.DataAnnotations;

namespace Proyecto_Gimnasio.Models
{
	public class Sale:AuditData
	{
		[Key]
		public int IdSale { get; set; }

		[Range(0, double.MaxValue)]
		public double Total { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime DateSale { get; set; }

		// Cliente asociado a la venta
		public int IdPerson { get; set; }
		public Person Person { get; set; }

		// Detalles
		public ICollection<SaleDetailsProducts>? SaleDetailsProducts { get; set; }
		public ICollection<SaleDetailsPlans>? SaleDetailsPlans { get; set; }

	}
}
