using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Animalogistics.Models{

	[Table("usuarios")]
    public class Usuario{
        [Key]
        [Display(Name = "Identificador")]
		public int Id {get; set;}
		[Required(ErrorMessage = "El nombre es obligatorio."),RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ]+$", ErrorMessage = "El nombre debe contener solo letras.")]
		public string? Nombre { get; set; }
		[Required(ErrorMessage = "El apellido es obligatorio."),RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ]+$", ErrorMessage = "El apellido debe contener solo letras.")]	
		public string? Apellido { get; set; }
		[Required(ErrorMessage = "El D.N.I. es obligatorio.")]
		[RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe contener 8 dígitos numericos.")]
		[Display(Name = "D.N.I.")]
		public string? DNI { get; set; }
		[Required(ErrorMessage = "El teléfono es obligatorio."), RegularExpression(@"^[0-9-]+$", ErrorMessage = "El teléfono debe contener solo numeros.")]
		[Display(Name = "Teléfono")]
		public string? Telefono { get; set; }
        [DataType(DataType.Password)]
		[Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string? Contraseña { get; set; }
		[DataType(DataType.EmailAddress, ErrorMessage = "El correo debe contener un formato valido.")]
		[Required(ErrorMessage = "El corre es obligatorio.")]
		public string? Correo { get; set; }  	
        public string? ImgUrl { get; set; } 

        //[Display(Name = "Imagen de perfil")]
		[NotMapped]
		public IFormFile? ImgFile { get; set; }





    }
}