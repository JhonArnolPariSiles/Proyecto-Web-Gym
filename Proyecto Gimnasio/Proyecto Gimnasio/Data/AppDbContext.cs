using Microsoft.EntityFrameworkCore;
using Proyecto_Gimnasio.Models;

namespace Proyecto_Gimnasio.Data
{
	public class AppDbContext:DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{

		}
		public DbSet<User> Users { get; set; }
	}
}
