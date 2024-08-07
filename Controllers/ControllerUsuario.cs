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
	public class ControllerUsuario(DataContext _contexto, IConfiguration _config) : ControllerBase
	{
		private readonly DataContext _contexto = _contexto;
		private readonly IConfiguration _config = _config;



		[HttpPost("usuarioRegistrar")]
		[AllowAnonymous]
		public async Task<IActionResult> Registrar([FromForm] Usuario usuario)
		{
			if (ModelState.IsValid)
			{

				if (string.IsNullOrEmpty(usuario.Contrasena))
				{
					ModelState.AddModelError("Contrasena", "La contraseña es obligatoria.");
					return BadRequest(ModelState);
				}
				else
				{
					try
					{
						string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
										password: usuario.Contrasena, // no es nulo por que ModelState.IsValid exige que el campo no sea nulo
										salt: System.Text.Encoding.ASCII.GetBytes(_config["Salt"]),
										prf: KeyDerivationPrf.HMACSHA1,
										iterationCount: 1000,
										numBytesRequested: 256 / 8));
						usuario.Contrasena = hashed;

						//verifico que el correo no exista
						if (await _contexto.Usuarios.AnyAsync(x => x.Correo == usuario.Correo))
						{
							return BadRequest("El correo ya existe");
						}
						_contexto.Usuarios.Add(usuario);
						await _contexto.SaveChangesAsync();

						if (usuario.FotoFile != null && usuario.Id > 0)
						{
							string PathData = _config["Data:usuarioImg"];

							/* if (!Directory.Exists(PathData))
							{
								Directory.CreateDirectory(path);
							} */
							//Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir

							string fileName = "avatar_" + usuario.Id + Guid.NewGuid().ToString() + Path.GetExtension(usuario.FotoFile.FileName);

							string pathCompleto = Path.Combine(PathData, fileName);
							usuario.FotoUrl = Path.Combine(_config["Data:usuarioImg"], fileName);
							// Esta operación guarda la foto en memoria en la ruta que necesitamos
							using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
							{
								usuario.FotoFile.CopyTo(stream);
							}
							_contexto.Usuarios.Update(usuario);

							await _contexto.SaveChangesAsync();
							//repositorioUsuario.Modificacion(u);
						}

						return Ok(usuario);
					}
					catch (Exception ex)
					{
						return BadRequest("Se produjo un error al procesar la solicitud." + "\n" + ex.Message + "\n" + ex.InnerException);
					}
				}
			}
			else
			{
				return BadRequest("campos invalidos" + "\n" + ModelState);

			}


		}



		[HttpPost("usuarioLogin")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromForm] UsuarioLogin usuariologin)
		{


			if (ModelState.IsValid)
			{
				try
				{




					string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
						password: usuariologin.Contrasena,
						salt: System.Text.Encoding.ASCII.GetBytes(_config["Salt"]),
						prf: KeyDerivationPrf.HMACSHA1,
						iterationCount: 1000,
						numBytesRequested: 256 / 8));

					var p = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Correo == usuariologin.Correo);


					/* IList<Permiso> permisos = await  _contexto.Permisos
							.Join(
								_contexto.Voluntarios,
								permiso => permiso.VoluntarioId,
								voluntario => voluntario.Id,
								(permiso, voluntario) => new { Permiso = permiso, Voluntario = voluntario }
							)
							.Join(
								_contexto.Usuarios,
								x => x.Voluntario.UsuarioId,
								usuario => usuario.Id,
								(x, usuario) => new { x.Permiso, Usuario = usuario }
							)
							.Where(x => x.Usuario.Id == p.Id)
							.Select(x => x.Permiso)
							.ToListAsync(); */



					if (p == null || p.Contrasena != hashed)
					{
						return BadRequest("Nombre de usuario y/o clave incorrecta");
					}
					else
					{
						var key = new SymmetricSecurityKey(
							System.Text.Encoding.ASCII.GetBytes(_config["TokenAuthentication:SecretKey"]));
						var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


						var claims = new List<Claim>
						{
							new Claim(ClaimTypes.Name, p.Correo),
							
							//asisgnar los permisos revisando la tabla de permisos
						};
						// Agregar permisos como claims de tipo Role
						/* 	foreach (var rol in permisos)
							{
								claims.Add(new Claim(ClaimTypes.Role, rol.Rol));
							} */




						var token = new JwtSecurityToken(
							issuer: _config["TokenAuthentication:Issuer"],
							audience: _config["TokenAuthentication:Audience"],
							claims: claims,
							expires: DateTime.Now.AddMinutes(60),
							signingCredentials: credenciales
						);
						return Ok(new JwtSecurityTokenHandler().WriteToken(token));
					}
				}
				catch (Exception ex)
				{
					return BadRequest("Error :" + ex + "\nSe produjo un error al tratar de procesar la solicitud");
				}
			}
			else
			{
				return BadRequest("campos invalidos" + "\n" + ModelState);

			}
		}





		[HttpGet("usuarioObtenerPerfil")]
		[Authorize]
		public async Task<ActionResult<Usuario>> ObtenerPerfil()
		{
			try
			{
				var UsuarioLogeado = User.Identity.Name;

				Usuario usuario = await _contexto.Usuarios.SingleOrDefaultAsync(x => x.Correo == UsuarioLogeado);

				if (usuario == null)
				{
					// El perfil no se encontró en la base de datos
					return NotFound("Perfil no encontrado");
				}

				return Ok(usuario);
			}
			catch (Exception ex)
			{
				return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
			}
		}




		[HttpPut("usuarioActualizarPerfil")]
		[Authorize]
		public async Task<IActionResult> ActualizarPerfil([FromForm] Usuario UsuarioEditado)
		{
			try
			{

				/* if (ModelState.IsValid)
				{ */

				var UsuarioLogeado = User.Identity.Name;

				Usuario usuarioOriginal = await _contexto.Usuarios.SingleOrDefaultAsync(x => x.Correo == UsuarioLogeado);


				if (usuarioOriginal == null)
				{
					return NotFound("Usuario no encontrado");
				}

				if (!string.IsNullOrEmpty(UsuarioEditado.Contrasena))
				{

					UsuarioEditado.Contrasena = Convert.ToBase64String(KeyDerivation.Pbkdf2(
						password: UsuarioEditado.Contrasena,
						salt: System.Text.Encoding.ASCII.GetBytes(_config["Salt"]),
						prf: KeyDerivationPrf.HMACSHA1,
						iterationCount: 1000,
						numBytesRequested: 256 / 8));
					usuarioOriginal.Contrasena = UsuarioEditado.Contrasena;
				}

				usuarioOriginal.Nombre = UsuarioEditado.Nombre;
				usuarioOriginal.Apellido = UsuarioEditado.Apellido;
				usuarioOriginal.DNI = UsuarioEditado.DNI;
				usuarioOriginal.Telefono = UsuarioEditado.Telefono;
				usuarioOriginal.Correo = UsuarioEditado.Correo;

				if (UsuarioEditado.FotoFile != null)
				{

					//primero borro la foto que tiene el usuario si es que tiene
					string PathData = _config["Data:usuarioImg"];

					if (!string.IsNullOrEmpty(usuarioOriginal.FotoUrl))
					{


						//string DirActual = Path.Combine(PathData, usuarioOriginal.FotoUrl);



						if (System.IO.File.Exists(usuarioOriginal.FotoUrl))
						{
							System.IO.File.Delete(usuarioOriginal.FotoUrl);
						}
					}



					// le registro la nueva foto

					string DirNueva = "avatar_" + usuarioOriginal.Id + Guid.NewGuid().ToString() + Path.GetExtension(usuarioOriginal.FotoUrl);
					string DirNuevaCompleta = Path.Combine(PathData, DirNueva);
					usuarioOriginal.FotoUrl = DirNuevaCompleta;


					using (FileStream stream = new FileStream(DirNuevaCompleta, FileMode.Create))
					{
						UsuarioEditado.FotoFile.CopyTo(stream);
					}

				}



				_contexto.Usuarios.Update(usuarioOriginal);
				await _contexto.SaveChangesAsync();
				return Ok(usuarioOriginal);
			}
			catch (Exception ex)
			{
				return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
			}
		}





		[HttpPut("usuarioActualizarFotoPerfil")]
		[Authorize]
		public async Task<IActionResult> ActualizarFoto(IFormFile? Foto)
		{
			try
			{
				var UsuarioLogeado = User.Identity.Name;
				Usuario usuario = await _contexto.Usuarios.SingleOrDefaultAsync(x => x.Correo == UsuarioLogeado);
				if (usuario == null)
				{
					return NotFound("Usuario no encontrado");
				}

				string PathData = _config["Data:usuarioImg"];

				if (Foto == null)
				{ //la quiero borrar entonces le seteo una por default


					string pathCompleto = Path.Combine(PathData, "DefaultUsuario.jpg");




					string DirAvatar = "avatar_" + usuario.Id + Path.GetExtension(usuario.FotoUrl);
					string DirAvatarCompleto = Path.Combine(PathData, DirAvatar);

					Console.WriteLine(DirAvatarCompleto);
					if (System.IO.File.Exists(DirAvatarCompleto))
					{
						System.IO.File.Delete(DirAvatarCompleto);
					}


					usuario.FotoUrl = pathCompleto;

				}
				else
				{


					string DirAvatarViejo = "avatar_" + usuario.Id + Path.GetExtension(usuario.FotoUrl);//avatar_8.jpeg
					string pathCompletoViejo = Path.Combine(PathData, DirAvatarViejo);
					if (System.IO.File.Exists(pathCompletoViejo))
					{
						System.IO.File.Delete(pathCompletoViejo);
					}


					string fileName = "avatar_" + usuario.Id + Path.GetExtension(Foto.FileName);

					string pathCompleto = Path.Combine(PathData, fileName);
					usuario.FotoUrl = pathCompleto;


					using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
					{
						Foto.CopyTo(stream);
					}

				}
				_contexto.Usuarios.Update(usuario);

				await _contexto.SaveChangesAsync();

				return Ok("Foto moficada correctamente");

			}
			catch (Exception ex)
			{
				return BadRequest("Se produjo un error al tratar de procesar la solicitud: " + ex.Message);
			}
		}


	}
}