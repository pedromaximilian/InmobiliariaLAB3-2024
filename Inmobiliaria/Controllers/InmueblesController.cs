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
using System.Security.Claims;
using Microsoft.Extensions.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Inmobiliaria.Models.DTOs;

namespace Inmobiliaria.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class InmueblesController : ControllerBase
    {
        private readonly MyContext _context;
        private readonly IWebHostEnvironment _environment;

        public InmueblesController(MyContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inmueble>>> GetProperties()
        {

            var id = int.Parse(User.FindFirstValue("Id"));

            try
            {

                var usuario = User.Identity.Name;
                var inmuebles = _context.Inmuebles
                .Include(t => t.Tipo)
                .Include(u => u.Uso)
                .Where(e => e.Propietario.Id == id);


                return Ok(inmuebles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("ContratoVigente")]
        public async Task<ActionResult<IEnumerable<Inmueble>>> GetInmueblesAlquilerVigentes()
        {
            var id = int.Parse(User.FindFirstValue("Id"));

            try
            {
                var inmuebles = await _context.Inmuebles
                    .Where(e => e.PropietarioId == id &&
                                _context.Alquilers.Any(a => a.InmuebleId == e.Id &&
                                                             a.FechaInicio <= DateTime.Now &&
                                                             a.FechaFin >= DateTime.Now))
                    .ToListAsync();

                return Ok(inmuebles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [HttpPost]
        public async Task<IActionResult> Post([FromForm] InmuebleInputDTO inmuebleInputDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Inmueble inmueble = new Inmueble();

                    var id = int.Parse(User.FindFirstValue("Id"));
                    inmueble.PropietarioId = id;
                    inmueble.Disponible = false;
                    inmueble.Precio = inmuebleInputDTO.Precio;
                    inmueble.Direccion = inmuebleInputDTO.Direccion;
                    inmueble.Ambientes = inmuebleInputDTO.Ambientes;
                    inmueble.TipoId = inmuebleInputDTO.TipoId;
                    inmueble.UsoId = inmuebleInputDTO.UsoId;

                    _context.Inmuebles.Add(inmueble);
                    await _context.SaveChangesAsync();

                    if (inmuebleInputDTO.Imagen != null)
                    {
                        var resizedImagePath = await ProcesarImagenAsync(inmuebleInputDTO.Imagen);
                        if (resizedImagePath == null)
                        {
                            return BadRequest("Formato de imagen no válido.");
                        }

                        inmueble.ImagenUrl = resizedImagePath;
                        await _context.SaveChangesAsync(); // Guarda los cambios
                    }

                    return CreatedAtAction(nameof(GetProperties), new { id = inmueble.Id }, inmueble);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task<string?> ProcesarImagenAsync(IFormFile imagen)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".jfif", ".bmp" };
            var extension = Path.GetExtension(imagen.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                return "Formato de imagen no válido.";
            }

            var imagePath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            // Renombrar el archivo y obtener su nueva ruta
            var inmuebleFileName = $"{Guid.NewGuid().ToString()}{extension}";
            var inmuebleFilePath = Path.Combine(imagePath, inmuebleFileName);

            // Guardar el archivo en el directorio 'uploads'
            using (var stream = new FileStream(inmuebleFilePath, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            
            // Retorna la ruta de la imagen redimensionada con separadores correctos
            return "uploads/" + inmuebleFileName;
        }



        [HttpPut("CambiarDisponible/{id}")]
        public async Task<IActionResult> CambiarDisponible(int id)
        {
            try
            {
                var inmueble = _context.Inmuebles.Include(e => e.Propietario).FirstOrDefault(e => e.Id == id);
                if (inmueble != null)
                {
                    Console.WriteLine(inmueble.Disponible);
                    if (inmueble.Disponible)
                    {
                        inmueble.Disponible = false;
                    }
                    else
                    {
                        inmueble.Disponible = true;
                    }
                    _context.Inmuebles.Update(inmueble);
                    _context.SaveChanges();
                    return Ok(inmueble.Disponible);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    }
