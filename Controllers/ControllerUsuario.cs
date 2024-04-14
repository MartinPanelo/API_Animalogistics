using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API_Animalogistics.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API_Animalogistics.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ControllerUsuario : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;

        public ControllerUsuario(DataContext contexto, IConfiguration config)
        {
            this.contexto = contexto;
            this.config = config;
        }

        [Authorize]
        [HttpGet("test")]
        public ActionResult<string> Test()
        {
            try
            {

                return "Ok";
            }
            catch (Exception ex)
            {
                return "Error: " + ex;
            }
        }
        //[Authorize(Policy = "Administrador")]
        [Authorize(Roles = "Administrador, Eventos")]
        [HttpGet("testPoly")]
        public ActionResult<string> TestPoly()
        {
            try
            {

                return "Ok";
            }
            catch (Exception ex)
            {
                return "Error: " + ex;
            }
        }



        [HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login()
		{
			try
			{
				
					var key = new SymmetricSecurityKey(
						System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
					var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Role, "Administrador"),
					};

					var token = new JwtSecurityToken(
						issuer: config["TokenAuthentication:Issuer"],
						audience: config["TokenAuthentication:Audience"],
						claims: claims,
						expires: DateTime.Now.AddMinutes(60),
						signingCredentials: credenciales
					);
					return Ok(new JwtSecurityTokenHandler().WriteToken(token));
				
			}
			catch (Exception ex)
			{
				return BadRequest("Error :"+ ex +"\nSe produjo un error al tratar de procesar la solicitud");
			}
		}





    }
}