using API_Animalogistics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Animalogistics.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ControllerNoticia(DataContext _contexto, IConfiguration _config) : ControllerBase
    {
        private readonly DataContext _contexto = _contexto;
        private readonly IConfiguration _config = _config;


        //listar todas las noticias por categoria
        [HttpGet("noticiaListarPorCategoria")]
        [Authorize]
        public async Task<IActionResult> NoticiaListarPorCategoria(string categoria)
        {
            try
            {

                var noticia = await _contexto.Noticias
                                            .Include(e => e.Usuario)
                                            .Include(e => e.Refugio)
                                            .Where(e => e.Categoria == categoria)
                                            .ToListAsync();
                if (noticia == null || !noticia.Any())
                {
                    return NotFound("No se encontraron noticias para esta categoria.");
                }
                return Ok(noticia);
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }
        }


        //listar todas las noticias
        [HttpGet("noticiaLista")]
        [Authorize]
        public async Task<IActionResult> NoticiaListar()
        {
            try
            {
                var noticia = await _contexto.Noticias

                                              .Include(e => e.Usuario)
                                              .Include(e => e.Refugio)
                                               .ToListAsync();
                if (noticia == null || !noticia.Any())
                {
                    return NotFound("No se encontraron noticias.");
                }
                return Ok(noticia);
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }
        }

        //listar todas las noticias de un refugio
        [HttpGet("noticiaListarPorRefugio")]
        [Authorize]
        public async Task<IActionResult> NoticiaListarPorRefugio(int refugioId)
        {
            try
            {

                //Comprobar que el refugio exista
                var refugio = _contexto.Refugios
                                              .Include(e => e.Usuario)
                                              .FirstOrDefault(e => e.Id == refugioId);
                if (refugio == null)
                {
                    return BadRequest("Refugio no encontrado." + refugioId);
                }
                // revisar que el usuario exista 
                var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }



                var noticias = await _contexto.Noticias
                                              .Include(e => e.Refugio)
                                              .Include(e => e.Usuario)
                                              .Where(e => e.RefugioId == refugioId)
                                              .ToListAsync();
                
                return Ok(noticias);
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }
        }


        /*    //listar todas las noticias de un refugio que es dueño o voluntario
          [HttpGet("noticiaListarPorRefugioGestion")]
          [Authorize]
          public async Task<IActionResult> NoticiaListarPorRefugioGestion(int refugioId)
          {
              try
              {

                  //Comprobar que el refugio exista
                  var refugio =  _contexto.Refugios
                                                .Include(e => e.Usuario)
                                                .FirstOrDefault(e => e.Id == refugioId);
                  if (refugio == null)
                  {
                      return BadRequest("Refugio no encontrado."+ refugioId);
                  }
                  // revisar que el usuario exista 
                  var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                  if (usuario == null)
                  {
                      return BadRequest("Usuario no encontrado.");
                  }

                  //El usuario es dueno del refugio?
                  var noticias = new List<Noticia>();
                  if(refugio.Usuario == usuario){

                       noticias = await _contexto.Noticias
                                                .Include(v => v.Refugio)       
                                                .Include(v => v.Usuario)                                      
                                                .Where( v =>v.RefugioId == refugioId)                                            
                                                .ToListAsync();

                  } // esta siendo voluntario de un refugio?

                  else
                  {
                      noticias = await _contexto.Noticias
                                                .Include(v => v.Refugio)       
                                                .Include(v => v.Usuario)                                      
                                                .Where( v =>v.RefugioId == refugioId && v.Usuario == usuario)                                            
                                                .ToListAsync();

                  }

                  return Ok(noticias);
              }
              catch (Exception ex)
              {
                  return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
              }
          }
   */


        //listar todas las noticias por categoria de un refugio
        [HttpGet("noticiaListarPorRefugioPorCategoria")]
        [Authorize]
        public async Task<IActionResult> NoticiaListarPorRefugioPorCategoria(string categoria, int refugioId)
        {
            try
            {

                //Comprobar que el refugio exista
                var refugio = await _contexto.Refugios
                                              .Include(e => e.Usuario)
                                              .SingleOrDefaultAsync(e => e.Id == refugioId);
                if (refugio == null)
                {
                    return BadRequest("Refugio no encontrado.");
                }
                // revisar que el usuario exista 
                var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }


                var noticias = await _contexto.Noticias
                                              .Include(e => e.Refugio)
                                              .Include(e => e.Usuario)
                                              .Where(e => e.RefugioId == refugioId
                                              && e.Categoria == categoria)
                                              .ToListAsync();
                if (noticias == null || !noticias.Any())
                {
                    return NotFound("No se encontraron noticias para este refugio.");
                }
                return Ok(noticias);
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }
        }


        [HttpPost("crearNoticia")]// Un usuario registra una noticia 
        [Authorize]
        public async Task<IActionResult> CrearNoticia([FromForm] Noticia noticia)
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
                                              .SingleOrDefaultAsync(e => e.Id == noticia.RefugioId);
                if (refugio == null)
                {
                    return BadRequest(new { mensaje = "Refugio no encontrado." });
                }

                // puede crear la noticia si es el duenio del refugio o si tiene una tarea asignada

                var RefugioVoluntarioPorTarea = _contexto.Tareas
                                .Include(t => t.Refugio)
                                .Include(t => t.Usuario)
                                .Where(t => t.Usuario == usuario || t.Refugio.Usuario == usuario)
                                .Select(t => t.Refugio)
                                .Distinct()
                                .ToList();

                if (RefugioVoluntarioPorTarea.Any(r => r.Id == refugio.Id))
                {

                    noticia.Usuario = usuario;

                    if (noticia.BannerFile != null)
                    {

                        var dirNoticiaBanner = Guid.NewGuid().ToString() + Path.GetExtension(noticia.BannerFile.FileName);

                        string pathCompleto = _config["Data:noticiaImg"] + dirNoticiaBanner;

                        noticia.BannerUrl = pathCompleto;
                        using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                        {
                            noticia.BannerFile.CopyTo(stream);
                        }
                    }

                    _contexto.Noticias.Add(noticia);
                    await _contexto.SaveChangesAsync();
                    return Ok(noticia);
                }else{

                    return BadRequest(new { mensaje = "No eres dueño del refugio o tiene una tarea asignada." });
                }



            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }

        }

        //Editar una noticia


        // solo si es dueno del refugio donde esta la noticia
        // solo si es el que redacto la notica
        [HttpGet("noticiaPorId")]
        [Authorize]
        public async Task<IActionResult> NoticiaPorId(int noticiaId)
        {

            try
            {

                var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }

                var noticia = await _contexto.Noticias
                                              .Include(v => v.Refugio)
                                              .Include(v => v.Usuario)
                                              .Where(v => v.Id == noticiaId && (v.Refugio.Usuario == usuario || v.Usuario == usuario))
                                              .FirstOrDefaultAsync();

                if (noticia == null)
                {

                    return NotFound(new { mensaje = "No se encontro la noticia." });
                }

                return Ok(noticia);

            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }
        }


        [HttpPut("noticiaEditar")]
        [Authorize]
        public async Task<IActionResult> NoticiaEditar([FromForm] Noticia noticia)
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
                    return NotFound(new { mensaje = "Usuario no encontrado." });
                }

                var noticiaOriginal = await _contexto.Noticias
                                  .Include(t => t.Refugio)
                                  .Include(t => t.Usuario)
                                   .FirstOrDefaultAsync(v => (v.Refugio.Usuario.Id == usuario.Id || v.Usuario == usuario) && (v.Id == noticia.Id));

                // reviso que el usuario sea el dueno del refugio o el que redacto la noticia

                if (noticiaOriginal == null)
                {
                    return NotFound(new { mensaje = "No se encontro la noticia." });
                }

                //solo se pueden editar el titulo, categoria, contenido y banner


                noticiaOriginal.Titulo = noticia.Titulo;
                noticiaOriginal.Categoria = noticia.Categoria;
                noticiaOriginal.Contenido = noticia.Contenido;


                // esto es para el banner
                string basePath = AppDomain.CurrentDomain.BaseDirectory;

                if (noticia.BannerFile != null)
                {


                    if (noticiaOriginal.BannerUrl != null)
                    {
                        string fullPath = Path.Combine(basePath, noticiaOriginal.BannerUrl);

                        if (System.IO.File.Exists(noticiaOriginal.BannerUrl))
                        {
                            System.IO.File.Delete(noticiaOriginal.BannerUrl);
                        }

                    }

                    var dirNoticiaBanner = Guid.NewGuid().ToString() + Path.GetExtension(noticia.BannerFile.FileName);

                    string pathCompleto = _config["Data:noticiaImg"] + dirNoticiaBanner;

                    noticiaOriginal.BannerUrl = pathCompleto;
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        noticia.BannerFile.CopyTo(stream);
                    }


                }

                _contexto.Update(noticiaOriginal);

                await _contexto.SaveChangesAsync();

                return Ok(noticiaOriginal);
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }
        }



        [HttpDelete("noticiaEliminar")]
        [Authorize]
        public async Task<IActionResult> NoticiaEliminar(int NoticiaId)
        {
            try
            {

                // si es dueño del refugio puede borrar cualquier Noticia

                // si es voluntario no puede borrar noticias que no a redactado, pero puede borrar las propias

                // si no es dueno ni voluntario no se que hace aca asdasdadsada


                var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }



                // conprobar que la noticia exista


                var noticia = await _contexto.Noticias
                                              .Include(v => v.Refugio)
                                              .Include(v => v.Usuario)
                                              .Where(v => v.Id == NoticiaId)
                                              .FirstOrDefaultAsync();


                if (noticia == null)
                {

                    return NotFound(new { mensaje = "No se encontro la noticia a borrar." + noticia.Id });
                }


                // esa noticia puede ser borrada? si es dueno o el que la redacto hace puede, si no, no


                if (noticia.Usuario == usuario || noticia.Refugio.Usuario == usuario)
                {

                    _contexto.Noticias.Remove(noticia);
                    _contexto.SaveChanges();
                    return Ok(noticia);
                }
                else
                {

                    return BadRequest("No tiene permisos para borrar esta noticia.");
                }



            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }
        }




    }
}