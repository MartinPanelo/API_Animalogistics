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

        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }



        [Display(Name = "Refugio")]
        public int RefugioId { get; set; }
        [ForeignKey(nameof(RefugioId))]
        public Refugio? Refugio { get; set; }
        public string? Categoria { get; set; }
        public string? Titulo { get; set; }
        public string? Contenido { get; set; }
        public string? BannerUrl { get; set; }
        [NotMapped]
        public IFormFile? BannerFile { get; set; }
    }
}