using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Zoo.Models;

namespace Zoo.Data
{
	public class ZooContext : DbContext
	{
		public ZooContext(DbContextOptions<ZooContext> options) : base(options) { }

		public DbSet<Animal> Animals { get; set; }
		public DbSet<Enclosure> Enclosures { get; set; }
        public DbSet<Category> Categories { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Configureer relaties en constraints
			modelBuilder.Entity<Animal>()
				.HasOne(a => a.Prey)
				.WithMany()
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}