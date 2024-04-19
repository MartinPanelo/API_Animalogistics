using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Animalogistics.Models
{

    [Table("eventos")]
    public class Evento
    {
        [Key]
        [Display(Name = "Identificador")]
        public int Id { get; set; }

        [Display(Name = "Refugio")]
        public int? RefugioId { get; set; }
        [ForeignKey(nameof(RefugioId))]
        public Refugio? Refugio { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public string? Descripcion { get; set; }
    }
}