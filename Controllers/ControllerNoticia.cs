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




















    }
}