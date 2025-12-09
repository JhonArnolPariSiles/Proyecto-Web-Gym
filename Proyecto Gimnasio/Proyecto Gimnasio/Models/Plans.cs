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

		[StringLength(500, MinimumLength = 5)]
        [RegularExpression(@"^[a-zA-Z0-9]+(?:\s+[a-zA-Z0-9]+)*$", ErrorMessage = "Name must contain only letters, numbers and spaces (no leading/trailing spaces)")]

        public string? Description { get; set; }

		[Required(ErrorMessage = "Sart Date date is required")]
		[Display(Name = "START DATE")]
		[DataType(DataType.Date)]

		public DateTime StartDate { get; set; }

		[Required(ErrorMessage = "End Date date is required")]
		[Display(Name = "END DATE")]
		[DataType(DataType.Date)]
		public DateTime EndDate { get; set; }

		public ICollection<SaleDetailsPlans>? SaleDetailsPlans { get; set; }
	}
}
