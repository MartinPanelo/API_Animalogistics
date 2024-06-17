/* using API_Animalogistics.Models;
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
		private readonly IConfiguration _config = _config; */

		/* [HttpGet("listarVoluntariadosDisponbilesDeUnRefugio")]
        [Authorize]
        public async Task<IActionResult> ListarVoluntariadosDisponbilesDeUnRefugio(int refugioId)
        {

            try
            {
               
                var voluntariados = await _contexto.Voluntarios
                                              .Include(v => v.Tarea)
                                              .Where(v => v.RefugioId == refugioId && v.Usuario == null)
                                             
                                              .ToListAsync();
                if (voluntariados == null || !voluntariados.Any())
                {
                  
                    return NotFound(new { mensaje ="No se encontraron voluntariados disponibles para este refugio."});
                }
                return Ok(voluntariados);

            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }
        } */
        


		/* // a este endpoint solo lo puede acceder el duenio del refugio
		[HttpGet("listarTodosLosVoluntariadosDeUnRefugio")]
        [Authorize]
		 public async Task<IActionResult> ListarTodosLosVoluntariadosDeUnRefugio(int refugioId)
        {
            try
            {

				var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);
				if (usuario == null)
				{
					return BadRequest("No se encontro el usuario");
				}

				//¿es dueño del refugio?
				var refugio = await _contexto.Refugios
					.Include(r => r.Id == refugioId && r.Usuario == usuario).FirstOrDefaultAsync();
					
					
				if (refugio == null)
				{
					return NotFound(new { permiso = "Solo el duenio del refugio puede gestionar a los voluntariados." });
				}
               
                var voluntariados = await _contexto.Voluntarios
                                              .Include(v => v.Tarea)
                                              .Where(v => v.RefugioId == refugioId && v.Usuario == null)
                                             
                                              .ToListAsync();
                if (voluntariados == null || !voluntariados.Any())
                {
                  
                    return NotFound(new { mensaje ="No se encontraron voluntariados disponibles para este refugio."});
                }
                return Ok(voluntariados);

            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }
        } */



/* 




        [HttpPut("anotarseComoVoluntario")]
		[Authorize]
		public async Task<IActionResult> AnotarseComoVoluntario(int voluntarioId)
		{

			try
			{
				
				 var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }
               
				var voluntariado = await _contexto.Voluntarios    
											  .Include(v => v.Refugio)                  
                                              .Where(v => v.Id == voluntarioId)										 
											  .FirstOrDefaultAsync();

              

				if (voluntariado == null )
				{
					// Si no se encuentran refugios
					return NotFound(new { mensaje = "No se encontro el voluntariado." });
				}

				voluntariado.Usuario = usuario;
				

               
				_contexto.Voluntarios.Update(voluntariado);
				_contexto.SaveChanges();

				return Ok(voluntariado);
			}
			catch (Exception ex)
			{
				// mensaje informativo en caso de error
				return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
			}
		}

    }
}
 */