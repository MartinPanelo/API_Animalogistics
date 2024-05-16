using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Animalogistics.Models
{

    [Table("noticias")]
    public class Noticia
    {
        [Key]
        [Display(Name = "Identificador")]
        public int Id { get; set; }

       /*  [Display(Name = "Refugio")]
        public int? RefugioId { get; set; }
        [ForeignKey(nameof(RefugioId))]
        public Refugio? Refugio { get; set; } */

        [Display(Name = "Voluntario")]
        public int VoluntarioId { get; set; }
        [ForeignKey(nameof(VoluntarioId))]
        public Voluntario? Voluntario { get; set; }
        public string? Categoria { get; set; }
        public string? Titulo { get; set; }
        public string? Contenido { get; set; }
        public string? BannerUrl { get; set; }
        [NotMapped]
		public IFormFile? BannerFile { get; set; }
    }
}