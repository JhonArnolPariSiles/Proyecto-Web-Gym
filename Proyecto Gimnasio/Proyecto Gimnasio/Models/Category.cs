using System.ComponentModel.DataAnnotations;

namespace Proyecto_Gimnasio.Models
{
	public class Category:AuditData
	{
		[Key]
		public int IdCategory { get; set; }

		[Required(ErrorMessage = "Name is required")]
		[StringLength(50, ErrorMessage = "{0} must be: minimum {2} and maximum {1}", MinimumLength = 5)]
		[Display(Name = "NAME  CATEGORY")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters and spaces")]

        public string NameCategory { get; set; }

		// Una categoría tiene muchos productos
		public ICollection<Product>? Products { get; set; }

	}
}
