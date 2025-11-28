using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Gimnasio.Models
{
	public class Plans : AuditData
	{
		[Key]
		public int IdPlan { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 5)]
		public string NamePlan { get; set; }

		[Required]
		[DataType(DataType.Currency)]
		public double Price { get; set; }

		[StringLength(500, MinimumLength = 20)]
		public string? Description { get; set; }

		[DataType(DataType.Date)]
		public DateTime StartDate { get; set; }

		[DataType(DataType.Date)]
		public DateTime EndDate { get; set; }

		public ICollection<SaleDetailsPlans>? SaleDetailsPlans { get; set; }
	}
}
