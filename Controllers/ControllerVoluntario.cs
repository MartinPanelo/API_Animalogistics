using API_Animalogistics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Animalogistics.Controllers
{

	[ApiController]
	[Route("[controller]")]
	public class ControllerVoluntario(DataContext _contexto, IConfiguration _config) : ControllerBase
	{
		private readonly DataContext _contexto = _contexto;
		private readonly IConfiguration _config = _config;


        // obtengo todas los voluntariados disponibles de un refugio
        // estan disponibles cuando el voluntario es null  NO SIRVE
        [HttpPut("anotarseComoVoluntario")]
		[Authorize]
		public async Task<IActionResult> AnotarseComoVoluntario(int tareaId)
		{

			try
			{
				Console.WriteLine("tareaId: " + tareaId);
				 var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }
               
				var tarea = await _contexto.Tareas
                                              .Include(v => v.Voluntario)
											  .Include(v => v.Voluntario.Refugio)
                                              .Where(v => v.Id == tareaId)										 
											  .FirstOrDefaultAsync();

				if (tarea == null )
				{
					// Si no se encuentran refugios
					return NotFound(new { mensaje = "No se encontro la tarea." });
				}

				tarea.Voluntario.Usuario = usuario;


				_contexto.Tareas.Update(tarea);
				_contexto.SaveChanges();

				return Ok(tarea.Voluntario);
			}
			catch (Exception ex)
			{
				// mensaje informativo en caso de error
				return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
			}
		}
    }
}
