using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace API_Animalogistics.Models
{
    [Table("permisos")]
    public class Permiso
    {
        [Key]
        [Display(Name = "Identificador")]
        public int Id { get; set; }
        [Display(Name = "Voluntario")]
        public int VoluntarioId { get; set; }
        [ForeignKey(nameof(VoluntarioId))]
        public Voluntario? Voluntario { get; set; }
        public string? Rol { get; set; }



    }
}
