using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Gimnasio.Models
{
	public class Plans:AuditData
	{
		[Key]
		public int IdPlan { get; set; }

		[Required, StringLength(150)]
		public string NamePlan { get; set; }

		[Range(0, double.MaxValue)]
		public double Price { get; set; }

		[StringLength(300)]
		public string Description { get; set; }

		[DataType(DataType.Date)]
		public DateTime StartDate { get; set; }

		[DataType(DataType.Date)]
		public DateTime EndDate { get; set; }

		public ICollection<SaleDetailsPlans>? SaleDetailsPlans { get; set; }

	}
}
