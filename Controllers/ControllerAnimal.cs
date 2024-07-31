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
        public async Task<IActionResult> AnimalListarPorRefugio(int refugioId)
        {

            try
            {
                var animales = await _contexto.Animales
                                              .Include(a => a.Usuario)
                                              .Include(a => a.Refugio)
                                              .Where(a => a.RefugioId == refugioId)
                                              .ToListAsync();

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
                    return NotFound(new { message = "No se encontraron animales para el refugio actual." });
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



                return Ok(animales);
            }
            catch (Exception ex)
            {
                // mensaje informativo en caso de error
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }
        }


        //editar animal que sea de un usuario y no tenga refugio

        [HttpPut("animalEditar")]
        [Authorize]
        public async Task<IActionResult> AnimalEditar([FromForm] Animal animalEditado)
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
                                    .Include(a => a.Refugio)
                                    .SingleOrDefaultAsync(i => (i.Id == animalEditado.Id) &&
                                                                ((i.Usuario == usuarioActual && i.RefugioId == null) ||
                                                                 (i.Refugio.Usuario == usuarioActual)));

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
                    animalExiste.Estado = animalEditado.Estado;




                    if (animalEditado.FotoFile != null)
                    {


                        if (animalExiste.FotoUrl != null)
                        {
                            string basePath = AppDomain.CurrentDomain.BaseDirectory;

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
                    return NotFound("No puede editar el animal.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }


        }


        [HttpDelete("animalBorrar")]
        [Authorize]
        public async Task<IActionResult> AnimalBorrar(int animalId)
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
                                    .Include(a => a.Refugio)
                                    .FirstOrDefaultAsync(a => a.Id == animalId);


                if (animal == null)
                {
                    return NotFound("No se encontro el animal.");
                }

                if ((animal.UsuarioId == usuario.Id && animal.RefugioId == null) || (animal.Refugio.Usuario == usuario))
                {
                    _contexto.Animales.Remove(animal);
                    await _contexto.SaveChangesAsync();
                    return Ok(animal);
                }
                else
                {
                    return NotFound("No puede borrar un animal que no gestiona.");
                }



            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
            }

        }




        [HttpPut("animalAgregarARefugio")]
        [Authorize]
        public async Task<IActionResult> AnimalAgregarARefugio(int animalId, int refugioId)
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

                // Verifico si el animal existe
                var animalExiste = await _contexto.Animales
                                    .Include(a => a.Usuario)
                                    .Include(a => a.Refugio)
                                    .SingleOrDefaultAsync(i => i.Id == animalId);

                var refugioExiste = await _contexto.Refugios
                                    .Include(r => r.Usuario)
                                    .SingleOrDefaultAsync(r => r.Id == refugioId && r.UsuarioId == usuarioActual.Id);

                if (animalExiste != null)
                {

                    animalExiste.Refugio = refugioExiste;
                    animalExiste.Estado = "En recuperacion";

                    _contexto.Animales.Update(animalExiste);
                    await _contexto.SaveChangesAsync();

                    return Ok(animalExiste);
                }
                else
                {
                    return NotFound("No puede editar el animal.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
            }


        }

    }

}