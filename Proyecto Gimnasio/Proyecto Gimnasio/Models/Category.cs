using System.ComponentModel.DataAnnotations;

namespace Proyecto_Gimnasio.Models
{
	public class Category:AuditData
	{
		[Key]
		public int IdCategory { get; set; }

		[Required, StringLength(100)]
		public string NameCategory { get; set; }

		// Una categoría tiene muchos productos
		public ICollection<Product>? Products { get; set; }

	}
}
