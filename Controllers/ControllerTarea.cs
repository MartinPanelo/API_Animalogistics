using API_Animalogistics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Animalogistics.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ControllerTarea(DataContext _contexto, IConfiguration _config) : ControllerBase
    {
        private readonly DataContext _contexto = _contexto;
        private readonly IConfiguration _config = _config;


        // Se obtienen todas las tareas disponibles de un refugio
        // una tarea esta disponible si no tiene un usuario como voluntario asociado
        // estas van a ser los voluntariados disponibles para que un usuario se haga voluntario de un refugio
        [HttpGet("listarTareasDisponbilesDeUnRefugio")]
        [Authorize]
        public async Task<IActionResult> ListarTareasDisponbilesDeUnRefugio(int refugioId)
        {

            try
            {
               
                var tareas = await _contexto.Tareas
                                              .Include(e => e.Voluntario)
                                              .Where(e => e.Voluntario.RefugioId == refugioId && e.Voluntario.Usuario == null)
                                             
                                              .ToListAsync();
                if (tareas == null || !tareas.Any())
                {
                  
                    return NotFound(new { mensaje ="No se encontraron tareas disponibles para este refugio."});
                }
                return Ok(tareas);

            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }
        }



/* 
        [HttpPost("crearTarea")]// Se crea una nueva tarea
        [Authorize]
        public async Task<IActionResult> CrearTarea([FromForm] Tarea tarea)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                // reviso que el usuario es valido
                var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }
                //Comprobar que el refugio exista
                var refugio = await _contexto.Refugios
                                              .Include(e => e.Usuario)
                                              .SingleOrDefaultAsync(e => e.Id == tarea.RefugioId);
                if (refugio == null)
                {
                    return BadRequest("Refugio no encontrado.");
                }
                // reviso que el usuario sea voluntario del refugio 

                var Uvoluntario = await _contexto.Voluntarios
                                    .Include(a => a.Usuario)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(v => v.UsuarioId == usuario.Id && v.RefugioId == refugio.Id);
                if (Uvoluntario == null)
                {
                    return BadRequest("Voluntario no encontrado o no pertenece a este refugio.");
                }

                // reviso que tenga el permiso de gestion de tareas

                var permiso = await _contexto.Permisos
                                              .Include(e => e.Voluntario)
                                              .AsNoTracking()
                                              .FirstOrDefaultAsync(p => p.VoluntarioId == Uvoluntario.Id && p.Rol == "Tareas");
                if (permiso == null)
                {
                    return BadRequest("No tiene permiso para gestionar tareas.");
                }


                _contexto.Tareas.Add(tarea);
                _contexto.SaveChanges();
                return Ok(tarea);
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }

        }



        [HttpPut("tareaEditar")]// Se edita una tarea
        [Authorize]
        public async Task<IActionResult> TareaEditar([FromForm] Tarea tarea)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                // reviso que el usuario es valido
                var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }
                //Comprobar que el refugio exista
                var refugio = await _contexto.Refugios
                                              .Include(e => e.Usuario)
                                              .SingleOrDefaultAsync(e => e.Id == tarea.RefugioId);
                if (refugio == null)
                {
                    return BadRequest("Refugio no encontrado.");
                }
                // reviso que el usuario sea voluntario del refugio

                var Uvoluntario = await _contexto.Voluntarios
                                    .Include(a => a.Usuario)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(v => v.UsuarioId == usuario.Id && v.RefugioId == refugio.Id);
                if (Uvoluntario == null)
                {
                    return BadRequest("Voluntario no encontrado o no pertenece a este refugio.");
                }


                // reviso que tenga el permiso de gestion de tareas

                var permiso = await _contexto.Permisos
                                              .Include(e => e.Voluntario)
                                              .AsNoTracking()
                                              .FirstOrDefaultAsync(p => p.VoluntarioId == Uvoluntario.Id && p.Rol == "Tareas");
                if (permiso == null)
                {
                    return BadRequest("No tiene permiso para gestionar tareas.");
                }

                var tareaActual = await _contexto.Tareas
                                              .AsNoTracking()
                                              .Include(e => e.Refugio)
                                              .Include(e => e.Voluntario)
                                              .SingleOrDefaultAsync(e => e.Id == tarea.Id);
                if (tareaActual == null)
                {
                    return NotFound("Tarea no encontrada.");
                }
                _contexto.Entry(tareaActual).CurrentValues.SetValues(tarea);
                _contexto.SaveChanges();
                return Ok(tarea);
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }
        }

 */




    }
}