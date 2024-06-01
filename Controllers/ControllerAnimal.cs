using API_Animalogistics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Animalogistics.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ControllerAnimal(DataContext _contexto, IConfiguration _config) : ControllerBase
    {
        private readonly DataContext _contexto = _contexto;
        private readonly IConfiguration _config = _config;



        [HttpPost("animalAgregar")]// Un usuario registra un animal 
        [Authorize]
        public async Task<IActionResult> AnimalAgregar([FromForm] Animal animal)
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

                Console.WriteLine(animal.GPSX + " | " + animal.GPSY);


                animal.UsuarioId = usuario.Id;

                if (animal.FotoFile != null)
                {
                    var animalImagen = Guid.NewGuid().ToString() + Path.GetExtension(animal.FotoFile.FileName);

                    string pathCompleto = _config["Data:animalImg"] + animalImagen;

                    animal.FotoUrl = pathCompleto;
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        animal.FotoFile.CopyTo(stream);
                    }
                }


                _contexto.Animales.Add(animal);
                _contexto.SaveChanges();
                return Ok(animal);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }


        }


        //listar animales (no asociados a refugios) de un usuario
        [HttpGet("animalListarPorUsuario")]
        [Authorize]
        public async Task<IActionResult> AnimalListarPorUsuario()
        {

            try
            {
                var usuarioActual = User.Identity.Name;
                // reviso que lo registre un usuario valido
                var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }
                var animales = await _contexto.Animales
                                              .Include(e => e.Usuario)
                                              .Where(e => e.Usuario.Correo == usuarioActual && e.RefugioId == null)
                                              .ToListAsync();

                if (animales == null || !animales.Any())
                {
                    // Si no se encuentran animales
                    return NotFound("No se encontraron animales para el usuario actual.");
                }

                return Ok(animales);
            }
            catch (Exception ex)
            {
                // mensaje informativo en caso de error
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }
        }

        //listar animales de un refugio
        [HttpGet("animalListarPorRefugio")]
        [Authorize]
        public async Task<IActionResult> AnimalListarPorRefugio([FromForm] int refugioId)
        {

            try
            {
                var animales = await _contexto.Animales
                                              .Include(a => a.Usuario)
                                              .Where(a => a.RefugioId == refugioId)
                                              .ToListAsync();

                if (animales == null || !animales.Any())
                {
                    // Si no se encuentran animales
                    return NotFound("No se encontraron animales para el refugio actual.");
                }

                return Ok(animales);
            }
            catch (Exception ex)
            {
                // mensaje informativo en caso de error
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }
        }


        //listar animales disponibles para adoptar de un refugio 
        [HttpGet("listarAnimalesDisponiblesParaAdoptarPorRefugio")]
        [Authorize]
        public async Task<IActionResult> ListarAnimalesDisponiblesParaAdoptarPorRefugio(int refugioId)
        {

            try
            {
                var animales = await _contexto.Animales
                                              .Include(a => a.Usuario)
                                              .Where(a => a.RefugioId == refugioId && (
                                                a.Estado == "En adopcion" || a.Estado == "En recuperacion"))
                                              .ToListAsync();

                if (animales == null || animales.Count == 0)
                {
                    // Si no se encuentran animales
                    return NotFound(new { message = "No se encontraron animales para el refugio actual."});
                }

                return Ok(animales);
            }
            catch (Exception ex)
            {
                // mensaje informativo en caso de error
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }
        }










        //listar todos los animales que no tengan refugio
        [HttpGet("animalListarSinRefugio")]
        [Authorize]
        public async Task<IActionResult> AnimalListarSinRefugio()
        {

            try
            {
                var animales = await _contexto.Animales
                                              .Include(e => e.Usuario)
                                              .Where(e => e.RefugioId == null)
                                              .ToListAsync();

                if (animales == null || !animales.Any())
                {
                    // Si no se encuentran animales
                    return NotFound("No se encontraron animales.");
                }

                return Ok(animales);
            }
            catch (Exception ex)
            {
                // mensaje informativo en caso de error
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }
        }


        //editar animal que sea de un usuario y no tenga refugio

        [HttpPut("animalEditarDeUsuario")]
        [Authorize]
        public async Task<IActionResult> AnimalEditarDeUsuario([FromForm] Animal animalEditado)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // reviso que lo registre un usuario valido
                var usuarioActual = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuarioActual == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }

                // Verifico si el animal fue registrado por el usuario actual
                var animalExiste = await _contexto.Animales
                                    .Include(a => a.Usuario)
                                    
                                    .SingleOrDefaultAsync(i => i.Id == animalEditado.Id && i.Usuario.Correo == usuarioActual.Correo && i.RefugioId == null);

                if (animalExiste != null)
                {
                    //actualizo el animal
                    Console.WriteLine("Animal Editado: " + animalEditado.GPSX + " | " + animalEditado.GPSY);
                    Console.WriteLine("Animal Existe: " + animalExiste.GPSX + " | " + animalExiste.GPSY);

                  
                    animalExiste.UsuarioId = usuarioActual.Id;

                    animalExiste.Nombre = animalEditado.Nombre;
                    animalExiste.Edad = animalEditado.Edad;
                    animalExiste.Tipo = animalEditado.Tipo;
                    animalExiste.Tamano = animalEditado.Tamano;
                    animalExiste.Collar = animalEditado.Collar;
                    animalExiste.Genero = animalEditado.Genero;
                    animalExiste.Comentarios = animalEditado.Comentarios;
                    animalExiste.GPSX = animalEditado.GPSX;
                    animalExiste.GPSY = animalEditado.GPSY;


                    string basePath = AppDomain.CurrentDomain.BaseDirectory;

                    if(animalEditado.FotoFile != null){


                        if(animalExiste.FotoUrl != null){
                            string fullPath = Path.Combine(basePath, animalExiste.FotoUrl);
                            Console.WriteLine(fullPath);
                            if (System.IO.File.Exists(animalExiste.FotoUrl)/*  && !animalEditado.FotoUrl.Contains("Defaultanimal.jpeg") */)
                            {
                                System.IO.File.Delete(animalExiste.FotoUrl);
                            }

                        }
                        
                        var animalImagen = Guid.NewGuid().ToString() + Path.GetExtension(animalEditado.FotoFile.FileName);

                        string pathCompleto = _config["Data:animalImg"] + animalImagen;

                        animalExiste.FotoUrl = pathCompleto;
                        using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                        {
                            animalEditado.FotoFile.CopyTo(stream);
                        }


                    }
                    

                    

                    

                    await _contexto.SaveChangesAsync();

                    return Ok(animalEditado);
                }
                else
                {
                    return NotFound("No se encontro el animal.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }


        }


        [HttpPut("animalEditarDeRefugio")]
        [Authorize(Roles = "Administrador, Animales")]
        public async Task<IActionResult> AnimalEditarDeRefugio([FromForm] Animal animalEditado)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // reviso que lo registre un usuario valido
                var usuarioActual = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuarioActual == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }
                // reviso que el usuario sea voluntario del refugio que va a editar el animal


                var Uvoluntario = await _contexto.Voluntarios
                                    .Include(a => a.Usuario)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(v => v.UsuarioId == usuarioActual.Id);


                if (Uvoluntario == null)
                {
                    return BadRequest("Este usuario no es voluntario de este refugio.");
                }


                //
                var animalExiste = (from animal in _contexto.Animales
                                    join voluntario in _contexto.Voluntarios on animal.RefugioId equals voluntario.RefugioId
                                    where animal.Id == animalEditado.Id && voluntario.Id == Uvoluntario.Id
                                    select animal).FirstOrDefault();


                if (animalExiste != null)
                {
                    //actualizo el animal

                    _contexto.Animales.Update(animalEditado);
                    await _contexto.SaveChangesAsync();

                    return Ok(animalEditado);
                }
                else
                {
                    return NotFound("No se encontro el animal.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message + "\n" + ex.InnerException);
            }


        }


        [HttpPut("animalEditarSinRefugio")]
        [Authorize(Roles = "Administrador, Animales")]
        public async Task<IActionResult> AnimalEditarSinRefugio([FromForm] Animal animalEditado)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // reviso que lo registre un usuario valido
                var usuarioActual = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuarioActual == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }
                // reviso que el usuario sea voluntario del refugio que va a editar el animal


                var Uvoluntario = await _contexto.Voluntarios
                                    .Include(a => a.Usuario)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(v => v.UsuarioId == usuarioActual.Id);


                if (Uvoluntario == null)
                {
                    return BadRequest("Este usuario no es voluntario de este refugio.");
                }


                //
                var animalExiste = await _contexto.Animales
                                    .Include(a => a.Usuario)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(a => a.Id == animalEditado.Id && a.RefugioId == null);




                if (animalExiste != null)
                {
                    //actualizo el animal

                    //desde el front a animalEditado le seteo el refugioId 

                    _contexto.Animales.Update(animalEditado);
                    await _contexto.SaveChangesAsync();

                    return Ok(animalEditado);
                }
                else
                {
                    return NotFound("No se encontro el animal.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message + "\n" + ex.InnerException);
            }

        }
/* 

        [HttpPut("animalEditarFoto")]
        [Authorize(Roles = "Administrador, Animales")]
        public async Task<IActionResult> AnimalEditarFoto(IFormFile? Foto, [FromForm] int AnimalId)
        {
            try
            {
                var UsuarioLogeado = User.Identity.Name;
                Usuario usuario = await _contexto.Usuarios.SingleOrDefaultAsync(u => u.Correo == UsuarioLogeado);

                Animal animal = await _contexto.Animales.SingleOrDefaultAsync(a => a.Id == AnimalId);

                if (usuario == null)
                {
                    return NotFound("Usuario no encontrado");
                }
                if (animal == null)
                {
                    return NotFound("animal no encontrado");
                }



                if (Foto == null)
                { //la quiero borrar entonces le seteo una por default


                    if (System.IO.File.Exists(animal.FotoUrl) && !animal.FotoUrl.Contains("Defaultanimal.jpeg"))
                    {
                        System.IO.File.Delete(animal.FotoUrl);
                    }

                    string pathBannerDefault = Path.Combine(_config["Data:animalImg"], "Defaultanimal.jpeg");
                    animal.FotoUrl = pathBannerDefault;

                }
                else
                {



                    if (System.IO.File.Exists(animal.FotoUrl) && !animal.FotoUrl.Contains("Defaultanimal.jpeg"))
                    {
                        System.IO.File.Delete(animal.FotoUrl);
                    }


                    var FotoUrl = Guid.NewGuid().ToString() + Path.GetExtension(Foto.FileName);

                    string pathCompleto = Path.Combine(_config["Data:animalImg"], FotoUrl);
                    animal.FotoUrl = pathCompleto;


                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        Foto.CopyTo(stream);
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

 */
        [HttpDelete("animalBorrarDeUsuario")]
        [Authorize]
        public async Task<IActionResult> AnimalBorrarDeUsuario(int animalId)
        {
            try
            {
                var usuarioActual = User.Identity.Name;
                // reviso que lo registre un usuario valido
                var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(e => e.Correo == User.Identity.Name);

                if (usuario == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                var animal = await _contexto.Animales
                                    .Include(a => a.Usuario)
                                    
                                    .FirstOrDefaultAsync(a => a.Id == animalId && a.RefugioId == null);


                if (animal == null)
                {
                    return NotFound("No se encontro el animal.");
                }

                if (animal.UsuarioId != usuario.Id)
                {
                    return BadRequest("No puede borrar un animal que no le pertenece.");
                }

                _contexto.Animales.Remove(animal);
                await _contexto.SaveChangesAsync();
                return Ok(animal);

            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }

        }





        // solo el usuario puede borrar animales que el halla registrado
        // y que no esten asociados a un refugio
        //-
        // un refugio no puede borrar animales, puedo asocialos a ellos y desasociarlos



    }

}