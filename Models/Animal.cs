using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Animalogistics.Models
{

    [Table("animales")]
    public class Animal
    {
        [Key]
        [Display(Name = "Identificador")]
        public int Id { get; set; }

        [Display(Name = "Refugio")]
        public int? RefugioId { get; set; }
        [ForeignKey(nameof(RefugioId))]
        public Refugio? Refugio { get; set; }

        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }
        public string? Nombre { get; set; }
        public string? Edad { get; set; }
        public string? Tipo { get; set; }
        public string? Tamano { get; set; }
        public bool Collar { get; set; }
        public string? Genero { get; set; }
        public string? Comentarios { get; set; }
        public string? GPSY { get; set; }
        public string? GPSX { get; set; }
        public string? Estado { get; set; }
        public string? FotoUrl { get; set; }
        [NotMapped]
		public IFormFile? FotoFile { get; set; }
    }
}