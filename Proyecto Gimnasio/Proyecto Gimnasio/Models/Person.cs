using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters and spaces")]


        public string LasName { get; set; }

		[StringLength(30, ErrorMessage = "{0} must be: minimum {2} and maximum {1}", MinimumLength = 5)]
		[Display(Name = "SECOND LAST NAME")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters and spaces")]


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

		// Relación 1-N con ventas
		public ICollection<Sale>? Sales { get; set; }
		[NotMapped] // Opcional: solo si no quieres mapearlo directamente
		public IEnumerable<Plans> Plans => Sales?
		.SelectMany(s => s.saleDetailsPlans)
		.Select(sd => sd.Plans);
		//relacion con User (1–1)
		// FK hacia User
		public int UserId { get; set; }

		[ForeignKey("UserId")]
		public User User { get; set; }
		// relacion persona 1-M planes
	}
}
