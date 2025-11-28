using System.ComponentModel.DataAnnotations;

namespace Proyecto_Gimnasio.Models
{
	public class AuditData
	{
		[ScaffoldColumn(false)]
		public int UserId { get; set; }

		[ScaffoldColumn(false)]
		public DateTime RegisterDate { get; set; }

		[ScaffoldColumn(false)]
		public DateTime? LastDate { get; set; }

		[ScaffoldColumn(false)]
		public bool Status { get; set; }
	}
}
