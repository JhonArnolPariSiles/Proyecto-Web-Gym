using Microsoft.EntityFrameworkCore;
using Proyecto_Gimnasio.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Proyecto_Gimnasio.Data
{
	public class AppDbContext : DbContext
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

		// Sobrescribimos SaveChanges para manejar la auditoría
		public override int SaveChanges()
		{
			UpdateAuditFields();
			return base.SaveChanges();
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			UpdateAuditFields();
			return base.SaveChangesAsync(cancellationToken);
		}

		private void UpdateAuditFields()
		{
			var entries = ChangeTracker.Entries()
				.Where(e => e.Entity is AuditData &&
						   (e.State == EntityState.Added || e.State == EntityState.Modified));

			foreach (var entry in entries)
			{
				var entity = (AuditData)entry.Entity;

				// Usuario estático por ahora
				entity.UserId = 1;

				if (entry.State == EntityState.Added)
				{
					entity.RegisterDate = DateTime.Now;
					entity.Status = true; // Siempre activo al crear
				}

				if (entry.State == EntityState.Modified)
				{
					entity.LastDate = DateTime.Now;
				}
			}
		}
	}
}
