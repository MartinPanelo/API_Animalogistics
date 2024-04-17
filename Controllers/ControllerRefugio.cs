using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API_Animalogistics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API_Animalogistics.Controllers
{

	[ApiController]
	[Route("[controller]")]
	public class ControllerRefugio(DataContext _contexto, IConfiguration _config) : ControllerBase
	{
		private readonly DataContext _contexto = _contexto;
		private readonly IConfiguration _config = _config;



		[HttpGet("refugioObtenerPorDueño")]// obtengo los refugios de los que soy dueño
		[Authorize]
		public async Task<IActionResult> RefugiosPorDueño()
		{

			try
			{
				var UsuarioLogeado = User.Identity.Name;

				var refugios = await _contexto.Refugios
											  /* .Include(r => r.Usuario) */ // con este include agrego el objeto usuario
											  .Where(r => r.Usuario.Correo == UsuarioLogeado)
											  .ToListAsync();

				if (refugios == null || refugios.Count == 0)
				{
					// Si no se encuentran refugios para el usuario actual
					return NotFound("No se encontraron refugios para el usuario actual.");
				}

				return Ok(refugios);
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

			try
			{
				var UsuarioLogeado = User.Identity.Name;

				var refugios = await _contexto.Voluntarios
											  .Include(r => r.Refugio) // con este include agrego el objeto usuario
											  .Where(v => v.Usuario.Correo == UsuarioLogeado && v.Refugio.UsuarioId != v.UsuarioId)
											  .ToListAsync();

				if (refugios == null || refugios.Count == 0)
				{
					// Si no se encuentran refugios para el usuario actual
					return NotFound("No se encontraron refugios para el usuario actual.");
				}

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

				string PathData = _config["Data:refugioImg"];

				var BannerUrl = Guid.NewGuid().ToString() + Path.GetExtension(refugio.BannerFile.FileName);

				string pathCompleto = PathData + BannerUrl;
				refugio.BannerUrl = pathCompleto;

				using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
				{
					refugio.BannerFile.CopyTo(stream);
				}
				_contexto.Refugios.Add(refugio);
				_contexto.SaveChanges();
				return Ok(refugio);


			}
			catch (Exception ex)
			{
				return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message);
			}

		}



		[HttpPut("refugioEditarPerfil")]
		[Authorize(Policy = "Administrador")]
		public async Task<IActionResult> RefugioEditarPerfil([FromForm] Refugio refugioEditado)
		{
			try
			{
				var usuarioActual = User.Identity.Name;

				// Verifico si el modelo recibido es válido
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				// Verifico si el refugio pertenece al usuario actual
				var RefugioExistente = await _contexto.Refugios
					/* .Include(r => r.Usuario) */
					.AsNoTracking()
					.FirstOrDefaultAsync(r => r.Id == refugioEditado.Id  && r.Usuario.Correo == usuarioActual);

				if (RefugioExistente == null)
				{
					return NotFound("No se pudo encontrar el refugio o no tienes permiso para editarlo.");
				}

				refugioEditado.UsuarioId = RefugioExistente.UsuarioId;
				refugioEditado.BannerUrl = RefugioExistente.BannerUrl;


				/* // controlo si ademas de editar la informacion quiere editar el banner

				if(refugioEditado.BannerFile != null)
				{
					string PathData = _config["Data:refugioImg"];

					int ultimoIndiceDelPunto = refugioEditado.BannerUrl.LastIndexOf('.');
					string nombreBannerUrlSinExtension = refugioEditado.BannerUrl.Substring(0, ultimoIndiceDelPunto);

					var BannerUrl = nombreBannerUrlSinExtension + Path.GetExtension(refugioEditado.BannerFile.FileName);

					string pathCompleto = PathData + BannerUrl;
					refugioEditado.BannerUrl = pathCompleto;

					using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
					{
						refugioEditado.BannerFile.CopyTo(stream);
					}

				} */
				

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
		[Authorize(Policy = "Administrador")]
		public async Task<IActionResult> RefugioEditarBanner(IFormFile? Banner,[FromForm] int RefugioId)
		{
			try
			{
				var UsuarioLogeado = User.Identity.Name;
				Usuario usuario = await _contexto.Usuarios.SingleOrDefaultAsync(u => u.Correo == UsuarioLogeado);

				Refugio refugio = await _contexto.Refugios.SingleOrDefaultAsync(r => r.Id == RefugioId);

				if (usuario == null)
				{
					return NotFound("Usuario no encontrado");
				}
				if (refugio == null)
				{
					return NotFound("Refugio no encontrado");
				}

			

				if (Banner == null)
				{ //la quiero borrar entonces le seteo una por default

				
					if (System.IO.File.Exists(refugio.BannerUrl) && !refugio.BannerUrl.Contains("DefaultRefugio.jpg"))
					{
						System.IO.File.Delete(refugio.BannerUrl);
					}

					string pathBannerDefault = Path.Combine( _config["Data:refugioImg"], "DefaultRefugio.jpg");
					refugio.BannerUrl = pathBannerDefault;

				}
				else
				{


					
					if (System.IO.File.Exists(refugio.BannerUrl) && !refugio.BannerUrl.Contains("DefaultRefugio.jpg"))
					{
						System.IO.File.Delete(refugio.BannerUrl);
					}



				//	string fileBanner = "avatar_" + usuario.Id + Path.GetExtension(Foto.FileName);

					var BannerUrl = Guid.NewGuid().ToString() + Path.GetExtension(Banner.FileName);

					string pathCompleto = Path.Combine( _config["Data:refugioImg"], BannerUrl);
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
