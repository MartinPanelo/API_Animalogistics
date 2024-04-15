using System.ComponentModel.DataAnnotations;


namespace API_Animalogistics.Models
{
	public class UsuarioLogin
	{
		[Required(ErrorMessage = "El corre es obligatorio.")]
		[DataType(DataType.EmailAddress)]
		public string Correo { get; set; }
		[Required(ErrorMessage = "La contraseña es obligatoria.")]
		[DataType(DataType.Password)]
		public string Contraseña { get; set; }
	}
}
