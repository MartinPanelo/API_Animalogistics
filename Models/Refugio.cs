using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Animalogistics.Models
{

    [Table("refugios")]
    public class Refugio
    {
        [Key]
        [Display(Name = "Identificador")]
        public int Id { get; set; }

        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }
        public string? Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Descripcion { get; set; }
        public string? Telefono { get; set; }
        public string? GPSY { get; set; }
        public string? GPSX { get; set; }
        public string? GPSRango { get; set; }
        public string? BannerUrl { get; set; }
        [NotMapped]
		public IFormFile? BannerFile { get; set; }
    }
}