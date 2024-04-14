using System.ComponentModel.DataAnnotations;


namespace API_Animalogistics.Models
{
	public class UsuarioLogin
	{
		[DataType(DataType.EmailAddress)]
		public string Correo { get; set; }
		[DataType(DataType.Password)]
		public string Contraseña { get; set; }
	}
}
