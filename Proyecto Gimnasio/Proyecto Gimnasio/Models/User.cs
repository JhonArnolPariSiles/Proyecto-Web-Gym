using System.ComponentModel.DataAnnotations;

namespace Proyecto_Gimnasio.Models
{
	public class User
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address")]
		[StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between {2} and {1} characters")]
		[Display(Name = "Email Address")]
		[RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
		ErrorMessage = "Please enter a valid email format")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between {2} and {1} characters")]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
		ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Role is required")]
		[Display(Name = "Role")]
		[RegularExpression(@"^(Admin|User|Employee)$",
		 ErrorMessage = "Role must be Admin, User, or Employee")]
		public string Rol { get; set; }

		// Relación 1–1 con Person
		public Person Person { get; set; }

	}
}
