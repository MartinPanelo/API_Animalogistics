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
    public class ControllerUsuario(DataContext _contexto, IConfiguration _config) : ControllerBase
    {
        private readonly DataContext _contexto = _contexto;
        private readonly IConfiguration _config = _config;



        [HttpPost("usuarioregistrar")]
        [AllowAnonymous]
		public async Task<IActionResult> UsuarioRegistrar([FromForm] Usuario usuario)
		{
			if (ModelState.IsValid){
			
				try
				{
					string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
									password: usuario.Contraseña, // no es nulo por que ModelState.IsValid exige que el campo no sea nulo
									salt: System.Text.Encoding.ASCII.GetBytes(_config["Salt"]),
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

					if (usuario.FotoFile != null && usuario.Id > 0)
					{
						string PathData = _config["Data:usuarioImg"];
						
						/* if (!Directory.Exists(PathData))
						{
							Directory.CreateDirectory(path);
						} */
						//Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
						string fileName = "avatar_" + usuario.Id + Path.GetExtension(usuario.FotoFile.FileName);
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
			}else{
					return BadRequest("campos invalidos" + "\n" + ModelState);
					
			}
			
		
		}



        [HttpPost("usuariologin")]
        [AllowAnonymous]
        public async Task<IActionResult> UsuarioLogin([FromForm] UsuarioLogin usuariologin){


            if (ModelState.IsValid)
			{
				try
				{


					

					string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
						password: usuariologin.Contraseña,
						salt: System.Text.Encoding.ASCII.GetBytes(_config["Salt"]),
						prf: KeyDerivationPrf.HMACSHA1,
						iterationCount: 1000,
						numBytesRequested: 256 / 8));

					var p = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Correo == usuariologin.Correo);
					if (p == null || p.Contraseña != hashed)
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
							new Claim(ClaimTypes.Name, p.Correo)
						};

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
					return BadRequest("Error :"+ ex +"\nSe produjo un error al tratar de procesar la solicitud");
				}
			}else{
					return BadRequest("campos invalidos" + "\n" + ModelState);
					
			}
		}












    }
}