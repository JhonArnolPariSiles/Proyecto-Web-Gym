namespace Proyecto_Gimnasio.Models
{
	public class Product
	{
		public int IdProduct { get; set; }
		public string NameProduct { get; set; }
		public string Image { get; set; }
		public int Stock { get; set; }
		public double Price { get; set; }
		public DateTime ExpirationDate  { get; set; }

	}
}
