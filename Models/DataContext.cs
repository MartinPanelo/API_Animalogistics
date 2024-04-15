
using Microsoft.EntityFrameworkCore;

namespace API_Animalogistics.Models
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{

		}
		public DbSet<Usuario> Usuarios { get; set; }
		public DbSet<Refugio> Refugios { get; set; }
		public DbSet<Voluntario> Voluntarios { get; set; }

	}
}