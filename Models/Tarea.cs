using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Animalogistics.Models
{

    [Table("tareas")]
    public class Tarea
    {
        [Key]
        [Display(Name = "Identificador")]
        public int Id { get; set; }



        
        [Display(Name = "Usuario")]
        public int? UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }


                
        [Display(Name = "Refugio")]
        public int? RefugioId { get; set; }
        [ForeignKey(nameof(RefugioId))]
        public Refugio? Refugio { get; set; }




        public string? Actividad { get; set; }
        public string? Descripcion { get; set; }




    }
}