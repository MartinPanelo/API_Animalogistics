using API_Animalogistics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

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
        
 
        [HttpGet("listarTareasDisponiblesDeUnRefugio")]
        [Authorize]
        public async Task<IActionResult> ListarTareasDisponiblesDeUnRefugio(int refugioId)
        {

            try
            {
               
                var tareas = await _contexto.Tareas
                                              .Include(v => v.Refugio)
                                              /* .Include(v => v.Usuario) */
                                              .Where( v =>v.RefugioId == refugioId && v.Usuario == null)                                            
                                              .ToListAsync();


                if (tareas == null || tareas.Count == 0)
                {
                  
                    return NotFound(new { mensaje ="No se encontraron tareas disponibles para este refugio."+refugioId});
                }
                return Ok(tareas);

            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }
        } 




        [HttpGet("listarTareasDeUnRefugio")]
        [Authorize]
        public async Task<IActionResult> ListarTareasDeUnRefugio(int refugioId)
        {

            try
            {

           

              


                var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }
                //Comprobar que el refugio exista
                var refugio = await _contexto.Refugios
                                              .Include(e => e.Usuario)
                                              .SingleOrDefaultAsync(e => e.Id == refugioId);
                if (refugio == null)
                {
                    return BadRequest("Refugio no encontrado.");
                }
/* 
                //El usuario es dueno del refugio?
                var tareas = new List<Tarea>();
                if(refugio.Usuario == usuario){
 */
                    var tareas = await _contexto.Tareas
                                              .Include(v => v.Refugio)       
                                              .Include(v => v.Usuario)                                      
                                              .Where( v =>v.RefugioId == refugioId)                                            
                                              .ToListAsync();

           /*      } */

              /*   // esta siendo voluntario de un refugio?

                else
                {
                    tareas = await _contexto.Tareas
                                              .Include(v => v.Refugio)       
                                              .Include(v => v.Usuario)                                      
                                              .Where( v =>v.RefugioId == refugioId && v.Usuario == usuario)                                            
                                              .ToListAsync();

                }
 */



                
                if (tareas == null)
                {
                  
                    return BadRequest(new { mensaje ="Se produjo un error al tratar de procesar la solicitud."+refugioId});
                }

                Console.WriteLine(tareas.Count);
                return Ok(tareas);

            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }
        } 



        [HttpDelete("borrarTareaRefugio")]
        [Authorize]
        public async Task<IActionResult> BorrarTareaRefugio(int tareaId)
        {

            try
            {

              // si es dueño del refugio puede borrar cualquier tarea

              // si es voluntario no puede borrar tareas a las que el se a anotado, si no que puede desasociarse desde editar

              // si no es dueno ni voluntario no se que hace aca asdasdadsada


                var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }



                // conprobar que la tarea exista
               

                var tarea = await _contexto.Tareas
                                              .Include(v => v.Refugio)       
                                              .Include(v => v.Usuario)                                      
                                              .Where( v =>v.Id == tareaId)                                            
                                              .FirstOrDefaultAsync();


                if (tarea == null )
                {
                  
                    return NotFound(new { mensaje ="No se encontro la tarea a borrar."});
                }


                // esa tarea puede ser borrada? si  es dueno  ,si


                if(tarea.Refugio.Usuario == usuario){

                    _contexto.Tareas.Remove(tarea);
                    _contexto.SaveChanges();
                    return Ok(tarea);
                }
               /*  else if(tarea.Usuario == usuario ){

                    tarea.Usuario = null;
                    _contexto.SaveChanges();
                    return Ok(tarea);    
                
                } */
                else{

                    return BadRequest(new { permiso ="No tiene permisos para borrar esta tarea."});
                }


                
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }
        } 





        [HttpPut("anotarseAUnaTarea")]
		[Authorize]
		public async Task<IActionResult> AnotarseAUnaTarea(int tareaId)
		{

			try
			{
				
				 var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado .");
                }
               
				var tarea = await _contexto.Tareas.Include(t => t.Refugio).FirstOrDefaultAsync(t => t.Id == tareaId);
                if (tarea == null)
                {
                    return NotFound("Tarea no encontrada.");
                }

                tarea.Usuario = usuario;

				
				

               
				_contexto.Tareas.Update(tarea);
				_contexto.SaveChanges();

				return Ok(tarea);
			}
			catch (Exception ex)
			{
				// mensaje informativo en caso de error
				return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
			}
		}


        // solo si es dueno del refugio donde esta la tarea
        // solo si es el encargado de la tarea
        [HttpGet("tareaPorId")]
		[Authorize]
        public async Task<IActionResult> TareaPorId(int tareaId){

            try
            {

                var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }

                var tarea = await _contexto.Tareas
                                              .Include(v => v.Refugio)       
                                              .Include(v => v.Usuario)                                      
                                              .Where( v =>v.Id == tareaId && (v.Refugio.Usuario == usuario || v.Usuario == usuario))                                            
                                              .FirstOrDefaultAsync(); 

                if (tarea == null )
                {
                  
                    return NotFound(new { mensaje ="No se encontro la tarea."});
                }

                return Ok(tarea);

            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }
        }


        

        // Se edita una tarea
        // si es dueno del refugio puede editar cualquier parametro de cualquier tarea
        // si es voluntario del refugio solo puede editar si estar a cargo de la tarea
        [HttpPut("tareaEditar")]
        [Authorize]
        public async Task<IActionResult> TareaEditar([FromBody] Tarea tarea)
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
                    return NotFound(new { mensaje = "Usuario no encontrado."});
                }

                 var tareaOriginal = await _contexto.Tareas
                                   .Include(t => t.Refugio)
                                   .Include(t => t.Usuario)
                                    .FirstOrDefaultAsync(v =>  (v.Refugio.Usuario.Id ==  usuario.Id || v.Usuario == usuario) && ( v.Id == tarea.Id));

                // reviso que el usuario sea el dueno del refugio o el encargado de la tarea

                if (tareaOriginal == null)
                {
                    return NotFound(new { mensaje = "No se encontro la tarea."});
                }
            

                // si el usuario es el duenio del refugio puedo editar todos los parametros de la tarea
                // si el usuario es el encargado o dueno de la tarea puedo editar el compromiso con la tarea ( liberar la tarea)


                if(tareaOriginal.Refugio.Usuario == usuario){
                    tareaOriginal.Actividad = tarea.Actividad;
                    tareaOriginal.Descripcion = tarea.Descripcion;
                }
                if(tarea.Usuario == null){
                    tareaOriginal.Usuario = null ;
                }         

             _contexto.Update(tareaOriginal);

                await _contexto.SaveChangesAsync();
                
               return Ok(tareaOriginal);
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }
        }







    
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
                    return BadRequest(new { mensaje = "Usuario no encontrado." });
                }
                //Comprobar que el refugio exista
                var refugio = await _contexto.Refugios
                                              .Include(e => e.Usuario)
                                              .SingleOrDefaultAsync(e => e.Id == tarea.RefugioId && e.Usuario == usuario);
                if (refugio == null)
                {
                    return BadRequest(new { mensaje = "Refugio no encontrado o no es el duenio." });
                }

                
                
                _contexto.Tareas.Add(tarea);
                await _contexto.SaveChangesAsync();
                return Ok(tarea);
            }
            catch (Exception ex)
            {
                return BadRequest( new { mensaje ="Se produjo un error al tratar de procesar la solicitud: " + ex.Message });
            }

        }


     





    }
}

