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
		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<Plans> Planss { get; set; }
		public DbSet<Sale> Sales { get; set; }
		public DbSet<SaleDetailsPlans> saleDetailsPlans { get; set; }
		public DbSet<SaleDetailsProducts> saleDetailsProducts { get; set; }
		public DbSet<Person> Persons { get; set; }
	}
}
