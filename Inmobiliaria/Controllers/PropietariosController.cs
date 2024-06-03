using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Context;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
using Inmobiliaria.Models.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PropietariosController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly MyContext _context;

        public PropietariosController(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }



        // GET: api/Propietarios/
        [HttpGet]
        public async Task<ActionResult<Propietario>> GetPropietario()
        {
            var id = int.Parse(User.FindFirstValue("Id"));

            if (id == null)
            {
                return BadRequest();
            }
            var propietario = await _context.Propietarios.FindAsync(id);

            if (propietario == null)
            {
                return NotFound();
            }
            propietario.Password = "";
            return propietario;
        }

        // PUT: api/Propietarios/

        [HttpPut]
        public async Task<IActionResult> PutPropietario([FromBody] PropietarioDTO propietarioDTO)
        {
            var id = int.Parse(User.FindFirstValue("Id"));
            if (id == null)
            {
                return BadRequest();
            }

            var loggedPropietario = _context.Propietarios.AsNoTracking().FirstOrDefault(x => x.Id == id);
            loggedPropietario.Apellido = propietarioDTO.Apellido;
            loggedPropietario.Nombre = propietarioDTO.Nombre;
            loggedPropietario.Dni = propietarioDTO.Dni;
            loggedPropietario.Email = propietarioDTO.Email;
            loggedPropietario.Telefono = propietarioDTO.Telefono;

            _context.Entry(loggedPropietario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                loggedPropietario.Password = "";
                return Ok(loggedPropietario);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropietarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        [HttpPut("updatePass")]
        public async Task<IActionResult> ChangePassword([FromBody] String pass)
        {
            var id = int.Parse(User.FindFirstValue("Id"));
            if (id == null)
            {
                return BadRequest();
            }

            var loggedPropietario = _context.Propietarios.AsNoTracking().FirstOrDefault(x => x.Id == id);

            loggedPropietario.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: pass,
                salt: System.Text.Encoding.ASCII.GetBytes(_config["Salt"]),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));

            _context.Entry(loggedPropietario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                loggedPropietario.Password = "";
                return Ok("Succes");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropietarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        private bool PropietarioExists(int id)
        {
            return _context.Propietarios.Any(e => e.Id == id);
        }
    }
}
