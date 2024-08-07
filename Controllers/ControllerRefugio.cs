using API_Animalogistics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Animalogistics.Controllers
{

	[ApiController]
	[Route("[controller]")]
	public class ControllerRefugio(DataContext _contexto, IConfiguration _config) : ControllerBase
	{
		private readonly DataContext _contexto = _contexto;
		private readonly IConfiguration _config = _config;


		[HttpGet("refugioLista")]// obtengo todos los refugios
		[Authorize]
		public async Task<IActionResult> RefugioLista()
		{

			try
			{
				var UsuarioLogeado = User.Identity.Name;

				// reviso que el usuario exista
				var usuario = await _contexto.Usuarios
											  .AsNoTracking()
											  .FirstOrDefaultAsync(u => u.Correo == UsuarioLogeado);
				if (usuario == null)
				{
					// Si el usuario no existe
					return NotFound("Usuario no encontrado.");
				}

				var refugios = await _contexto.Refugios
											  .Include(r => r.Usuario) // con este include agrego el objeto usuario											 
											  .ToListAsync();

				if (refugios == null || refugios.Count == 0)
				{
					// Si no se encuentran refugios
					return NotFound("No se encontraron refugios.");
				}

				return Ok(refugios);
			}
			catch (Exception ex)
			{
				// mensaje informativo en caso de error
				return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
			}
		}

		[HttpGet("refugioObtenerPorDueno")]// obtengo los refugios de los que soy dueño
		[Authorize]
		public async Task<IActionResult> RefugiosPorDueno()
		{

			try
			{
				var UsuarioLogeado = User.Identity.Name;

				// reviso que el usuario exista
				var usuario = await _contexto.Usuarios
											  .AsNoTracking()
											  .FirstOrDefaultAsync(u => u.Correo == UsuarioLogeado);
				if (usuario == null)
				{
					// Si el usuario no existe
					return NotFound("Usuario no encontrado.");
				}

				var refugios = await _contexto.Refugios
											  /* .Include(r => r.Usuario) */ // con este include agrego el objeto usuario
											  .Where(r => r.Usuario.Correo == UsuarioLogeado)
											  .ToListAsync();

				/* 	if (refugios == null || refugios.Count == 0)
					{
						// Si no se encuentran refugios para el usuario actual
							return NotFound(new { mensaje = "No se encontraron refugios para el usuario actual." });

					} */

				return Ok(refugios);
			}
			catch (Exception ex)
			{
				// mensaje informativo en caso de error
				return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
			}
		}


		[HttpGet("refugioPorId")]// refugio por id y dueno
		[Authorize]
		public async Task<IActionResult> RefugioPorId(int refugioId)
		{

			try
			{
				var UsuarioLogeado = User.Identity.Name;

				// reviso que el usuario exista
				var usuario = await _contexto.Usuarios
											  .AsNoTracking()
											  .FirstOrDefaultAsync(u => u.Correo == UsuarioLogeado);
				if (usuario == null)
				{
					// Si el usuario no existe
					return NotFound("Usuario no encontrado.");
				}

				var refugio = await _contexto.Refugios
											  .Include(r => r.Usuario)
											  .Where(r => r.Usuario == usuario && r.Id == refugioId)
											  .FirstOrDefaultAsync();

				if (refugio == null)
				{
					// Si no se encuentran refugios para el usuario actual
					return NotFound(new { mensaje = "No se encontraron refugios para el usuario actual." });

				}

				return Ok(refugio);
			}
			catch (Exception ex)
			{
				// mensaje informativo en caso de error
				return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
			}
		}

		[HttpGet("refugioObtenerPorVoluntario")]// obtengo los refugios en los que soy voluntario
		[Authorize]
		public async Task<IActionResult> RefugiosPorVoluntario()
		{
			// ES VOLUNTARIO SI TIENE UNA TAREA , ~EVENTO~ , NOTICIA ASOCIADO A ESTE REFUGIO
			try
			{
				var UsuarioLogeado = User.Identity.Name;



				var voluntarioPorTarea = _contexto.Tareas
								.Include(t => t.Refugio)
								.Include(t => t.Usuario)
								.Where(t => t.Usuario.Correo == UsuarioLogeado && t.Refugio.Usuario.Correo != UsuarioLogeado)
								.Select(t => t.Refugio)
								.Distinct()
								.ToList();

				var voluntarioPorNoticia = _contexto.Noticias
								.Include(t => t.Refugio)
								.Include(t => t.Usuario)
								.Where(t => t.Usuario.Correo == UsuarioLogeado && t.Refugio.Usuario.Correo != UsuarioLogeado)
								.Select(t => t.Refugio)
								.Distinct()
								.ToList();

				var refugios = voluntarioPorTarea.Union(voluntarioPorNoticia).Distinct().ToList();



				return Ok(refugios);
			}
			catch (Exception ex)
			{
				// mensaje informativo en caso de error
				return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
			}
		}




		[HttpPost("refugioAgregar")]
		[Authorize]
		public async Task<IActionResult> RefugioAgregar([FromForm] Refugio refugio)
		{

			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var UsuarioLogeado = await _contexto.Usuarios.SingleOrDefaultAsync(u => u.Correo == User.Identity.Name);

				if (UsuarioLogeado == null)
				{
					return BadRequest("usuario no encontrado.");
				}


				if (refugio.BannerFile != null)
				{
					string PathData = _config["Data:refugioImg"];

					var BannerUrl = Guid.NewGuid().ToString() + Path.GetExtension(refugio.BannerFile.FileName);

					string pathCompleto = PathData + BannerUrl;
					refugio.BannerUrl = pathCompleto;

					using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
					{
						refugio.BannerFile.CopyTo(stream);
					}
				}
				refugio.Usuario = UsuarioLogeado;
				_contexto.Refugios.Add(refugio);
				_contexto.SaveChanges();
				return Ok(refugio);


			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
			}

		}



		[HttpPut("refugioEditarPerfil")]
		[Authorize]
		public async Task<IActionResult> RefugioEditarPerfil([FromForm] Refugio refugioEditado)
		{
			try
			{
				var usuarioActual = User.Identity.Name;

				// Verifico si lo recibido es válido
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				// Verifico si el usuario existe
				var usuario = await _contexto.Usuarios
					.AsNoTracking()
					.FirstOrDefaultAsync(u => u.Correo == usuarioActual);
				if (usuario == null)
				{
					return NotFound("Usuario no encontrado.");
				}

				// Verifico si el refugio pertenece al usuario actual
				var RefugioExistente = await _contexto.Refugios
					.Include(r => r.Usuario)
					.AsNoTracking()
					.FirstOrDefaultAsync(r => r.Id == refugioEditado.Id && r.Usuario == usuario);

				if (RefugioExistente == null)
				{
					return NotFound("No se pudo encontrar el refugio o este refugio no le pertenece.");
				}

				refugioEditado.UsuarioId = RefugioExistente.UsuarioId;

				if (refugioEditado.BannerFile != null)
				{


					if (RefugioExistente.BannerUrl != null)
					{
						string basePath = AppDomain.CurrentDomain.BaseDirectory;

						string fullPath = Path.Combine(basePath, RefugioExistente.BannerUrl);

						Console.WriteLine(fullPath);
						if (System.IO.File.Exists(RefugioExistente.BannerUrl))
						{
							System.IO.File.Delete(RefugioExistente.BannerUrl);
						}

					}

					var refugioImagen = Guid.NewGuid().ToString() + Path.GetExtension(refugioEditado.BannerFile.FileName);

					string pathCompleto = _config["Data:refugioImg"] + refugioImagen;

					//  RefugioExistente.BannerUrl = pathCompleto;
					using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
					{
						refugioEditado.BannerFile.CopyTo(stream);
					}
					refugioEditado.BannerUrl = pathCompleto;


				}


				// Actualizo el refugio
				_contexto.Refugios.Update(refugioEditado);
				await _contexto.SaveChangesAsync();

				return Ok(refugioEditado);

			}
			catch (Exception ex)
			{

				return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message + "\n" + ex.InnerException);
			}
		}




		[HttpPut("refugioEditarBanner")]
		[Authorize]
		public async Task<IActionResult> RefugioEditarBanner(IFormFile? Banner, [FromForm] int RefugioId)
		{
			try
			{
				var UsuarioLogeado = User.Identity.Name;

				Usuario usuario = await _contexto.Usuarios.SingleOrDefaultAsync(u => u.Correo == UsuarioLogeado);
				if (usuario == null)
				{
					return NotFound("Usuario no encontrado");
				}

				Refugio refugio = await _contexto.Refugios.SingleOrDefaultAsync(r => r.Id == RefugioId);
				if (refugio == null)
				{
					return NotFound("Refugio no encontrado");
				}

				// Verifico si el refugio pertenece al usuario actual
				if (refugio.UsuarioId != usuario.Id)
				{
					return BadRequest("No se puede editar el banner de un refugio que no le pertenece");
				}



				if (Banner == null)
				{ //la quiero borrar entonces le seteo una por default


					if (System.IO.File.Exists(refugio.BannerUrl) && !refugio.BannerUrl.Contains("DefaultRefugio.jpg"))
					{
						System.IO.File.Delete(refugio.BannerUrl);
					}

					string pathBannerDefault = Path.Combine(_config["Data:refugioImg"], "DefaultRefugio.jpg");
					refugio.BannerUrl = pathBannerDefault;

				}
				else
				{



					if (System.IO.File.Exists(refugio.BannerUrl) && !refugio.BannerUrl.Contains("DefaultRefugio.jpg"))
					{
						System.IO.File.Delete(refugio.BannerUrl);
					}


					var BannerUrl = Guid.NewGuid().ToString() + Path.GetExtension(Banner.FileName);

					string pathCompleto = Path.Combine(_config["Data:refugioImg"], BannerUrl);
					refugio.BannerUrl = pathCompleto;


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


	}
}
