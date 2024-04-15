using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API_Animalogistics.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API_Animalogistics.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ControllerUsuario(DataContext _contexto, IConfiguration config) : ControllerBase
    {
        private readonly DataContext _contexto = _contexto;
        private readonly IConfiguration config = config;



        [HttpPost("usuarioregistrar")]
        [AllowAnonymous]
		public async Task<IActionResult> UsuarioRegistrar([FromForm] Usuario usuario)
		{
			if (ModelState.IsValid){
			
				try
				{
					string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
									password: usuario.Contraseña, // no es nulo por que ModelState.IsValid exige que el campo no sea nulo
									salt: System.Text.Encoding.ASCII.GetBytes("SalDelHimalaya"),
									prf: KeyDerivationPrf.HMACSHA1,
									iterationCount: 1000,
									numBytesRequested: 256 / 8));
					usuario.Contraseña = hashed;

                    //verifico que el correo no exista
                    if (await _contexto.Usuarios.AnyAsync(x => x.Correo == usuario.Correo))
                    {
                        return BadRequest("El correo ya existe");
                    }
                    _contexto.Usuarios.Add(usuario);
                    await _contexto.SaveChangesAsync();

					if (usuario.ImgFile != null && usuario.Id > 0)
					{
						string PathData = config["Data:usuarioImg"];
						
						/* if (!Directory.Exists(PathData))
						{
							Directory.CreateDirectory(path);
						} */
						//Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
						string fileName = "avatar_" + usuario.Id + Path.GetExtension(usuario.ImgFile.FileName);
						string pathCompleto = Path.Combine(PathData, fileName);
						usuario.ImgUrl = Path.Combine(config["Data:usuarioImg"], fileName);
						// Esta operación guarda la foto en memoria en la ruta que necesitamos
						using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
						{
							usuario.ImgFile.CopyTo(stream);
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
			}else{
					return BadRequest("campos invalidos" + "\n" + ModelState);
					
			}
			
		
		}

















    }
}