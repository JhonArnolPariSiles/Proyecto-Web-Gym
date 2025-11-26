using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Gimnasio.Models
{
	public class Product:AuditData
	{
		[Key]
		public int IdProduct { get; set; }

		[Required, StringLength(150)]
		public string NameProduct { get; set; }

		public string Image { get; set; }

		[Range(0, int.MaxValue)]
		public int Stock { get; set; }

		[Range(0, double.MaxValue)]
		public double Price { get; set; }

		[DataType(DataType.Date)]
		public DateTime ExpirationDate { get; set; }

		// FK a Category
		[Required]
		public int IdCategory { get; set; }

		[ForeignKey("IdCategory")]
		public Category Category { get; set; }

		// Detalles de venta
		public ICollection<SaleDetailsProducts>? SaleDetailsProducts { get; set; }

	}
}
