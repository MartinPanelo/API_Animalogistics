
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
	/* 	public DbSet<Voluntario> Voluntarios { get; set; } */
/* 		public DbSet<Permiso> Permisos { get; set; } */
		public DbSet<Animal> Animales { get; set; }
		public DbSet<Noticia> Noticias { get; set; }
		public DbSet<Evento> Eventos { get; set; }
		public DbSet<Tarea> Tareas { get; set; }

	}
}