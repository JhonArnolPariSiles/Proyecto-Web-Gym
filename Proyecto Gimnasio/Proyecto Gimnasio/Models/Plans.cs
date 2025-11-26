using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Gimnasio.Models
{
	public class Plans
	{
		[Key]
		public int IdPlan { get; set; }

		[Required, StringLength(200)]
		public string NamePlan { get; set; }

		[Range(0, double.MaxValue)]
		public double Price { get; set; }

		[StringLength(500)]
		public string Description { get; set; }

		[DataType(DataType.Date)]
		public DateTime StartDate { get; set; }

		[DataType(DataType.Date)]
		public DateTime EndDate { get; set; }

		//  Person (1–N) plan
		public int IdPerson { get; set; }

		[ForeignKey("IdPerson")]
		public Person Person { get; set; }

		// muchos a muchos de planes y personas
		public ICollection<SaleDetailsPlans> SaleDetailsPlans { get; set; }

	}
}
