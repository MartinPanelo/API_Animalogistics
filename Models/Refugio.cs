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
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "La dirección es obligatoria.")]
        public string? Direccion { get; set; }
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string? Descripcion { get; set; }
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        public string? Telefono { get; set; }
        [Required(ErrorMessage = "La ubicacion GPS es obligatoria.")]
        public double GPSY { get; set; }
        [Required(ErrorMessage = "La ubicacion GPS es obligatoria.")]
        public double GPSX { get; set; }
        [Required(ErrorMessage = "El rango de accion es obligatorio.")]
        public int GPSRango { get; set; }
        public string? BannerUrl { get; set; }
        [NotMapped]
		public IFormFile? BannerFile { get; set; }
    }
}