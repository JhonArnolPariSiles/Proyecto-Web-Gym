using System.ComponentModel.DataAnnotations;

namespace Proyecto_Gimnasio.Models
{
	public class User
	{
		[Key]
		public int Id { get; set; }

		[Required, EmailAddress]
		public string Email { get; set; }

		[Required, StringLength(100)]
		public string Password { get; set; }

		[Required]
		public string Rol { get; set; }

		// Relación 1–1 con Person
		public Person Person { get; set; }

	}
}
