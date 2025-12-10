using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Proyecto_Gimnasio.Models
{
	public class Person : AuditData
	{
		[Key]
		public int IdPerson { get; set; }

		[Required(ErrorMessage = "Name is required")]
		[StringLength(20, ErrorMessage = "{0} must be: minimum {2} and maximum {1}", MinimumLength = 4)]
		[Display(Name = "NAME")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters and spaces")]

        public string Name { get; set; }

		[Required(ErrorMessage = "Last Name is required")]
		[StringLength(30, ErrorMessage = "{0} must be: minimum {2} and maximum {1}", MinimumLength = 5)]
		[Display(Name = "LAST NAME")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last Name must contain only letters and spaces")]

        public string LasName { get; set; }

		[StringLength(30, ErrorMessage = "{0} must be: minimum {2} and maximum {1}", MinimumLength = 5)]
		[Display(Name = "SECOND LAST NAME")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Second Last Name must contain only letters and spaces")]

        public string? SecondLastName { get; set; }

		[Required(ErrorMessage = "Date of birth is required")]
		[DataType(DataType.Date)]
		[Display(Name = "Date of Birth")]
		public DateTime DateBirthay { get; set; }

		[Range(1, int.MaxValue)]
		public int Cnit { get; set; }

		[Required(ErrorMessage = "Gender is required")]
		[RegularExpression("^[MF]$", ErrorMessage = "Solo se permite M o F")]
		public char Gender { get; set; }

		public ICollection<Sale>? Sales { get; set; }

		public ICollection<SaleProduct>? SaleProducts { get; set; }

		[NotMapped]
		public IEnumerable<Plans> PlansEnrolled => Sales?
			.SelectMany(s => s.saleDetailsPlans ?? Enumerable.Empty<SaleDetailsPlans>())
			.Select(sd => sd.Plans)
			.Where(p => p != null)!;

		[NotMapped]
		public IEnumerable<Product> ProductosComprados => SaleProducts?
			.SelectMany(sp => sp.saleDetailsProducts ?? Enumerable.Empty<SaleDetailsProducts>())
			.Select(d => d.Product)
			.Where(p => p != null)!;

		public int UserId { get; set; }

		[ForeignKey("UserId")]
		public User User { get; set; } = null!; // null! para evitar warnings si usas nullable reference types
	}
}