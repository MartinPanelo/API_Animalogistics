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



        [HttpPost("noticiaAgregar")]// Un usuario registra una noticia 
        [Authorize(Roles = "Administrador, Noticias")]
        public async Task<IActionResult> NoticiaAgregar([FromForm] Noticia noticia)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                // reviso que lo registre un usuario valido
                var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }

                // revisar que el refugio exista
                var refugio = await _contexto.Refugios
                                              .Include(e => e.Usuario)
                                              .SingleOrDefaultAsync(e => e.Id == noticia.RefugioId);
                if (refugio == null)
                {
                    return BadRequest("Refugio no encontrado.");
                }

                // revisar que el voluntario exista en este refugio
                var voluntario = await _contexto.Voluntarios
                                              .Include(e => e.Refugio)
                                              .SingleOrDefaultAsync(e => e.Id == noticia.VoluntarioId && e.RefugioId == refugio.Id);
                if (voluntario == null)
                {
                    return BadRequest("Voluntario no encontrado o no pertenece a este refugio.");
                }


                var noticiaImagen = Guid.NewGuid().ToString() + Path.GetExtension(noticia.BannerFile.FileName);

                string pathCompleto = _config["Data:noticiaImg"] + noticiaImagen;

                noticia.BannerUrl = pathCompleto;
                //   noticia.VoluntarioId = usuario.Id;

                using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                {
                    noticia.BannerFile.CopyTo(stream);
                }
                _contexto.Noticias.Add(noticia);
                _contexto.SaveChanges();
                return Ok(noticia);

            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }

        }



        //listar todas las noticias por categoria
        [HttpGet("noticiaListarPorCategoria")]
        [Authorize(Roles = "Administrador, Noticias")]
        public async Task<IActionResult> NoticiaListarPorCategoria([FromForm] string categoria)
        {
            try
            {
                var noticia = await _contexto.Noticias
                                              .Include(e => e.Refugio)
                                              .Include(e => e.Voluntario)
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


        //listar todas las noticias por categoria de un refugio
        [HttpGet("noticiaListarPorRefugio")]
        [Authorize(Roles = "Administrador, Noticias")]
        public async Task<IActionResult> NoticiaListarPorRefugio([FromForm] int refugioId)
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

                //comprobar que el voluntario esta viendo noticias de un refugio al que pertenece                
                var voluntario = await _contexto.Voluntarios.
                                              Include(e => e.Refugio)
                                              .SingleOrDefaultAsync(v => v.Usuario.Correo == User.Identity.Name && v.RefugioId == refugioId);
                if (voluntario == null)
                {
                    return BadRequest("Voluntario no encontrado o no pertenece a este refugio.");
                }


                var noticia = await _contexto.Noticias
                                              .Include(e => e.Refugio)
                                              .Include(e => e.Voluntario)
                                              .Where(e => e.RefugioId == refugioId)
                                              .ToListAsync();
                if (noticia == null || !noticia.Any())
                {
                    return NotFound("No se encontraron noticias para este refugio.");
                }
                return Ok(noticia);
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }
        }


        //Editar una noticia
        [HttpPut("noticiaEditar")]
        [Authorize(Roles = "Administrador, Noticias")]
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
                    return BadRequest("Usuario no encontrado.");
                }

                //comprobar que el voluntario esta aditando una noticiaa de un refugio al que pertenece                
                var voluntario = await _contexto.Voluntarios.
                                              Include(e => e.Refugio)
                                              .SingleOrDefaultAsync(v => v.Usuario.Correo == User.Identity.Name && v.RefugioId == noticia.RefugioId);
                if (voluntario == null)
                {
                    return BadRequest("Voluntario no encontrado o no pertenece a este refugio.");
                }


                var noticiaActual = await _contexto.Noticias
                                              .AsNoTracking()
                                              .Include(e => e.Refugio)
                                              .Include(e => e.Voluntario)
                                              .SingleOrDefaultAsync(e => e.Id == noticia.Id);
                if (noticiaActual == null)
                {
                    return NotFound("Noticia no encontrada.");
                }

                // noticia.BannerUrl = noticiaActual.BannerUrl; // no viene del front ??

                _contexto.Noticias.Update(noticia);
                await _contexto.SaveChangesAsync();
                return Ok(noticia);

            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }
        }




        [HttpPut("noticiaEditarBanner")]
        [Authorize(Roles = "Administrador, Noticias")]
        public async Task<IActionResult> NoticiaEditarBanner(IFormFile? Banner, [FromForm] int NoticiaId)
        {
            try
            {
                var UsuarioLogeado = User.Identity.Name;
                Usuario usuario = await _contexto.Usuarios.SingleOrDefaultAsync(u => u.Correo == UsuarioLogeado);

                Noticia noticia = await _contexto.Noticias.SingleOrDefaultAsync(r => r.Id == NoticiaId);

                if (usuario == null)
                {
                    return NotFound("Usuario no encontrado");
                }
                if (noticia == null)
                {
                    return NotFound("noticia no encontrada");
                }



                if (Banner == null)
                { //la quiero borrar entonces le seteo una por default


                    if (System.IO.File.Exists(noticia.BannerUrl) && !noticia.BannerUrl.Contains("DefaultNoticia.jpg"))
                    {
                        System.IO.File.Delete(noticia.BannerUrl);
                    }

                    string pathBannerDefault = Path.Combine(_config["Data:noticiaImg"], "DefaultNoticia.jpg");
                    noticia.BannerUrl = pathBannerDefault;

                }
                else
                {



                    if (System.IO.File.Exists(noticia.BannerUrl) && !noticia.BannerUrl.Contains("DefaultNoticia.jpg"))
                    {
                        System.IO.File.Delete(noticia.BannerUrl);
                    }


                    var BannerUrl = Guid.NewGuid().ToString() + Path.GetExtension(Banner.FileName);

                    string pathCompleto = Path.Combine(_config["Data:noticiaImg"], BannerUrl);
                    noticia.BannerUrl = pathCompleto;


                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        Banner.CopyTo(stream);
                    }

                }
                //_contexto.Refugios.Update(refugio);

                await _contexto.SaveChangesAsync();

                return Ok("Banner moficado correctamente");

            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }
        }




        [HttpDelete("noticiaEliminar")]
        [Authorize(Roles = "Administrador, Noticias")]
        public async Task<IActionResult> NoticiaEliminar([FromForm] int NoticiaId)
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

                var noticiaActual = await _contexto.Noticias

                                                              .Include(e => e.Refugio)
                                                              .Include(e => e.Voluntario)
                                                              .SingleOrDefaultAsync(e => e.Id == NoticiaId);
                if (noticiaActual == null)
                {
                    return NotFound("Noticia no encontrada.");
                }
                //comprobar que el voluntario esta borrand una noticiaa de un refugio al que pertenece                
                var voluntario = await _contexto.Voluntarios.
                                              Include(e => e.Refugio)
                                              .SingleOrDefaultAsync(v => v.Usuario.Correo == User.Identity.Name && v.RefugioId == noticiaActual.RefugioId);
                if (voluntario == null)
                {
                    return BadRequest("Voluntario no encontrado o no pertenece a este refugio.");
                }

                if (System.IO.File.Exists(noticiaActual.BannerUrl))
                {
                    System.IO.File.Delete(noticiaActual.BannerUrl);
                }

                _contexto.Noticias.Remove(noticiaActual);
                await _contexto.SaveChangesAsync();
                return Ok(noticiaActual);

            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }

        }


    }
}