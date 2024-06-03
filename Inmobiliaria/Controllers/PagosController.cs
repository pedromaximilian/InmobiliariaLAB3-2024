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
    public class PagosController : ControllerBase
    {
        private readonly MyContext _context;

        public PagosController(MyContext context)
        {
            _context = context;
        }

        // GET: api/Pagos
        [HttpGet("Alquiler/{id}")]
        public async Task<ActionResult<IEnumerable<Pago>>> GetPagos(int id)
        {
            return await _context.Pagos.ToListAsync();
        }

    }
}
