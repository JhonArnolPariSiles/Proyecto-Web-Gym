using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Gimnasio.Models
{
	public class Person
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
		public DateTime DateBirthay { get; set; }

		[Range(1, int.MaxValue)]
		public int Cnit { get; set; }

		[RegularExpression("^[MF]$", ErrorMessage = "Solo se permite M o F")]
		public char Gender { get; set; }
		//relacion con User (1–1)
		// FK hacia User
		public int UserId { get; set; }

		[ForeignKey("UserId")]
		public User User { get; set; }
		// relacion persona 1-M planes
		public ICollection<Plans> Plans { get; set; }
	}
}
