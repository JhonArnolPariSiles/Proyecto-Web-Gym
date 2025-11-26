using System.ComponentModel.DataAnnotations;

namespace Proyecto_Gimnasio.Models
{
	public class Category
	{
		[Key]
		public int IdCategory { get; set; }

		[Required, StringLength(100)]
		public string NameCategory { get; set; }
		//relacion Category 1–N Product
		public ICollection<Product> Products { get; set; }

	}
}
