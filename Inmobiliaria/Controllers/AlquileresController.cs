using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Context;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AlquileresController : ControllerBase
    {
        private readonly MyContext _context;

        public AlquileresController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("Inmueble/{id}")]
        public async Task<ActionResult<Alquiler>> GetAlquilerFromInmueble(int id)
        {
            var today = DateTime.Today;
            var alquiler = await _context.Alquilers
                .Where(a => a.FechaInicio <= today && a.FechaFin >= today && a.InmuebleId == id)
                .Include(a => a.Inmueble)
                .Include(a => a.Inquilino)
                .FirstOrDefaultAsync();

            if (alquiler == null)
            {
                return NotFound();
            }

            return alquiler;
        }

    }
}
