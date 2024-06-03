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
    public class InquilinosController : ControllerBase
    {
        private readonly MyContext _context;

        public InquilinosController(MyContext context)
        {
            _context = context;
        }



        [HttpGet("Inmueble/{id}")]
        public async Task<ActionResult<Inquilino>> GetInquilinoFromInmueble(int id)
        {
            try
            {
                var alquilerVigente = await _context.Alquilers
                    .Where(a => a.InmuebleId == id && a.FechaInicio <= DateTime.Now && a.FechaFin >= DateTime.Now)
                    .FirstOrDefaultAsync();

                if (alquilerVigente == null)
                {
                    return NotFound("No hay alquiler vigente para este inmueble.");
                }

                var inquilino = await _context.Inquilinos
                    .Where(i => i.Id == alquilerVigente.InquilinoId)
                    .FirstOrDefaultAsync();

                if (inquilino == null)
                {
                    return NotFound("No se encontró el inquilino para el alquiler vigente.");
                }

                return Ok(inquilino);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



       

        private bool InquilinoExists(int id)
        {
            return _context.Inquilinos.Any(e => e.Id == id);
        }
    }
}
