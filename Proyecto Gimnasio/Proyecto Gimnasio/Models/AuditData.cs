namespace Proyecto_Gimnasio.Models
{
	public class AuditData
	{
		public int UserId { get; set; }
		public DateTime RegisterDate { get; set; }
		public DateTime LastDate { get; set; }
		public bool Status { get; set; }
	}
}
