using API_Animalogistics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Animalogistics.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ControllerEvento(DataContext _contexto, IConfiguration _config) : ControllerBase
    {
        private readonly DataContext _contexto = _contexto;
        private readonly IConfiguration _config = _config;



        [HttpGet("listarEventos")]// Se obtienen todos los eventos
        [Authorize]
        public async Task<IActionResult> ListarEventos(){

            try
            {
                // reviso que sea un usuario valido
                var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }
                var eventos = await _contexto.Eventos
                                              .Include(e => e.Refugio)
                                              /* .Where(e => e.RefugioId == evento.RefugioId) */
                                              .ToListAsync();
                if (eventos == null || !eventos.Any())
                {
                    return NotFound("No se encontraron eventos para este refugio.");
                }
                return Ok(eventos);
                
            }catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }
        }

        [HttpGet("listarEventosDeUnRefugio")]// Se obtienen todos los eventos de un refugio
        [Authorize]
        public async Task<IActionResult> ListarEventosDeUnRefugio([FromForm] Refugio refugio){

            try
            {
                // reviso que sea un usuario valido
                var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }
                var eventos = await _contexto.Eventos
                                              .Include(e => e.Refugio)
                                              .Where(e => e.RefugioId == refugio.Id)
                                              .ToListAsync();
                if (eventos == null || !eventos.Any())
                {
                    return NotFound("No se encontraron eventos para este refugio.");
                }
                return Ok(eventos);
                
            }catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }
        }



        [HttpPost("crearEvento")]// Se crea un evento
        [Authorize(Roles = "Administrador, Eventos")]
        public async Task<IActionResult> CrearEvento([FromForm] Evento evento)
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
                                              .SingleOrDefaultAsync(e => e.Id == evento.RefugioId);
                if (refugio == null)
                {
                    return BadRequest("Refugio no encontrado.");
                }

                // compruebo que usuario es voluntario del refugio


                var Uvoluntario = await _contexto.Voluntarios
                                    .Include(a => a.Usuario)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(v => v.UsuarioId == usuario.Id && v.RefugioId == evento.RefugioId);
                if (Uvoluntario == null)
                {
                    return BadRequest("Voluntario no encontrado o no pertenece a este refugio.");
                }

                _contexto.Eventos.Add(evento);
                await _contexto.SaveChangesAsync();
                return Ok(evento);
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }            
        }




        [HttpPut("actualizarEvento")]// Se actualiza un evento
        [Authorize(Roles = "Administrador, Eventos")]
        public async Task<IActionResult> ActualizarEvento([FromForm] Evento evento)
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
                                              .SingleOrDefaultAsync(e => e.Id == evento.RefugioId);
                if (refugio == null)
                {
                    return BadRequest("Refugio no encontrado.");
                }
                var eventoActual = await _contexto.Eventos
                                              .AsNoTracking()
                                              .SingleOrDefaultAsync(e => e.Id == evento.Id);
                if (eventoActual == null)
                {
                    return NotFound("Noticia no encontrada.");
                }


                //comprobar que el voluntario esta aditando una noticiaa de un refugio al que pertenece
                var Uvoluntario = await _contexto.Voluntarios
                                    .Include(a => a.Usuario)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(v => v.UsuarioId == usuario.Id && v.RefugioId == evento.RefugioId);
                if (Uvoluntario == null)
                {
                    return BadRequest("Voluntario no encontrado o no pertenece a este refugio.");
                }

                _contexto.Eventos.Update(evento);
                await _contexto.SaveChangesAsync();
                return Ok(evento);
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }
        }




        [HttpDelete("eliminarEvento")]// Se elimina un evento
        [Authorize(Roles = "Administrador, Eventos")]
        public async Task<IActionResult> EliminarEvento([FromForm] Evento evento)
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
                                              .SingleOrDefaultAsync(e => e.Id == evento.RefugioId);
                if (refugio == null)
                {
                    return BadRequest("Refugio no encontrado.");
                }
                var eventoActual = await _contexto.Eventos
                                              .AsNoTracking()
                                              .SingleOrDefaultAsync(e => e.Id == evento.Id);
                if (eventoActual == null)
                {
                    return NotFound("Evento no encontrada.");
                }
                //comprobar que el voluntario esta aditando un evento de un refugio al que pertenece
                var Uvoluntario = await _contexto.Voluntarios
                                    .Include(a => a.Usuario)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(v => v.UsuarioId == usuario.Id && v.RefugioId == evento.RefugioId);
                if (Uvoluntario == null)
                {
                    return BadRequest("Voluntario no encontrado o no pertenece a este refugio.");
                } 

                _contexto.Eventos.Remove(evento);
                await _contexto.SaveChangesAsync();
                return Ok(evento);
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }
        }









    }

}