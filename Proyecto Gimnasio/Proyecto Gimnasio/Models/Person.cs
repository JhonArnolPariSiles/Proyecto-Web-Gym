using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Gimnasio.Models
{
	public class Person:AuditData
	{
		[Key]
		public int IdPerson { get; set; }

		[Required, StringLength(100)]
		public string Name { get; set; }

		[Required, StringLength(100)]
		public string LasName { get; set; }

		[Required, StringLength(100)]
		public string SecondLastName { get; set; }

		[DataType(DataType.Date)]
		[Required]
		public DateTime DateBirthay { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "El CI debe ser un número válido.")]
		public int Cnit { get; set; }

		[Required]
		[RegularExpression("^[MF]$", ErrorMessage = "El género debe ser M o F.")]
		public char Gender { get; set; }

		// Relación 1-1 con User
		public int UserId { get; set; }
		[ForeignKey("UserId")]
		public User User { get; set; }

		// Ventas asociadas
		public ICollection<Sale> Sales { get; set; }
	}
}
