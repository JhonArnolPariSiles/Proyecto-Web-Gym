using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Gimnasio.Models
{
	public class Plans:AuditData
	{
		[Key]
		public int IdPlan { get; set; }

		[Required(ErrorMessage = "Name is required")]
		[StringLength(50, ErrorMessage = "{0} must be: minimum {2} and maximum {1}", MinimumLength = 5)]
		[Display(Name = "NAME PLAN")]
		[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only letters")]
		public string NamePlan { get; set; }

		[Required(ErrorMessage = "Price is required")]
		[Display(Name = "Price")]
		[DataType(DataType.Currency)]
		[Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
		public double Price { get; set; }

		[Required(ErrorMessage = "Description is required")]
		[StringLength(500, ErrorMessage = "{0} must be: minimum {2} and maximum {1}", MinimumLength = 20)]
		[Display(Name = "DESCRIPTION")]
		[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only letters")]
		public string Description { get; set; }

		[Required(ErrorMessage = "Sart Date date is required")]
		[Display(Name = "START DATE")]
		[DataType(DataType.Date)]
		public DateTime StartDate { get; set; }

		[Required(ErrorMessage = "End Date date is required")]
		[Display(Name = "END DATE")]
		[DataType(DataType.Date)]
		public DateTime EndDate { get; set; }

		//  Person (1–N) plan
		public int IdPerson { get; set; }

		[ForeignKey("IdPerson")]
		public Person Person { get; set; }

		// muchos a muchos de planes y personas
		public ICollection<SaleDetailsPlans>? SaleDetailsPlans { get; set; }

	}
}
