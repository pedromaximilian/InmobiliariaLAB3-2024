using Azure.Core;
using Inmobiliaria.Context;
using Inmobiliaria.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

using System.Net.Sockets;
using System.Security.Claims;
using System.Text;


namespace Inmobiliaria.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly MyContext _context;


        public AuthController(IConfiguration config, MyContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: loginModel.Password,
                    salt: System.Text.Encoding.ASCII.GetBytes(_config["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                var p = await _context.Propietarios.FirstOrDefaultAsync(x => x.Email == loginModel.Email);
                if (p == null || p.Password != hashed)
                {
                    return BadRequest("Nombre de usuario o clave incorrecta");
                }
                else
                {
                    var key = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(_config["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, p.Email),
                        new Claim("Id", p.Id.ToString()),
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
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reset")]
        [AllowAnonymous]
        public async Task<IActionResult> EnviarEmail([FromForm] string email)
        {
            try
            {
                var propietario = await _context.Propietarios.FirstOrDefaultAsync(x => x.Email == email);
                if (propietario == null)
                {
                    return NotFound();
                }
                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["TokenAuthentication:SecretKey"]));
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
                {
                    new Claim("Id", propietario.Id.ToString()),
                };
                var token = new JwtSecurityToken(
                    issuer: _config["TokenAuthentication:Issuer"],
                    audience: _config["TokenAuthentication:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: credenciales
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                var dominio = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
                var resetLink = Url.Action("restoredPassword", "Auth");
                var rutaCompleta = Request.Scheme + "://" + GetLocalIpAddress() + ":" + Request.Host.Port + resetLink;
                var message = new MimeMessage();
                message.To.Add(new MailboxAddress(propietario.Nombre, propietario.Email));
                message.From.Add(new MailboxAddress("Sistema", _config["SMTPUser"]));
                message.Subject = "Restablecimiento de Contraseña";
                message.Body = new TextPart("html")
                {
                    Text = $@"<h1>Hola {propietario.Nombre},</h1>
						   <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.
							<p>Para restaurar su contraseña haga clic aqui:</p>
						   <a href='{rutaCompleta}?access_token={tokenString}'>{rutaCompleta}?access_token={tokenString}</a>"
                };
                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync("sandbox.smtp.mailtrap.io", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_config["SMTPUser"], _config["SMTPPass"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("restoredPassword")]
        public async Task<IActionResult> CambiarPassword()
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["TokenAuthentication:SecretKey"]);
                var symmetricKey = new SymmetricSecurityKey(key);
                Random rand = new Random(Environment.TickCount);
                string randomChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
                string nuevaClave = "";
                for (int i = 0; i < 8; i++)
                {
                    nuevaClave += randomChars[rand.Next(0, randomChars.Length)];
                }
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: nuevaClave,
                    salt: Encoding.ASCII.GetBytes(_config["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                var p = await _context.Propietarios.FirstOrDefaultAsync(x => x.Id == int.Parse(User.FindFirstValue("Id")));
                if (p == null)
                {
                    return BadRequest("Nombre de usuario incorrecto");
                }
                else
                {
                    p.Password = hashed;
                    _context.Propietarios.Update(p);
                    await _context.SaveChangesAsync();
                    var message = new MimeMessage();
                    message.To.Add(new MailboxAddress(p.Nombre, p.Email));
                    message.From.Add(new MailboxAddress("Sistema", _config["SMTPUser"]));
                    message.Subject = "Restablecimiento de Contraseña";
                    message.Body = new TextPart("html")
                    {
                        Text = $"<h1>Hola {p.Nombre},</h1>" +
                               $"<p>Has cambiado tu contraseña de forma correcta. " +
                               $"Tu nueva contraseña es la siguiente: {nuevaClave}</p>"
                    };
                    using var client = new SmtpClient();
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    await client.ConnectAsync("sandbox.smtp.mailtrap.io", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_config["SMTPUser"], _config["SMTPPass"]);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);

                    return Ok("Se ha restablecido la contraseña correctamente.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private string GetLocalIpAddress()
        {
            string localIp = null;
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIp = ip.ToString();
                    break;
                }
            }
            return localIp;
        }

    }

}
