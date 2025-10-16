using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MS_Collection_WbApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MS_Collection_WbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly JoyeriaMsBdContext _context;

        public CategoriaController(JoyeriaMsBdContext context)
        {
            _context = context;
        }

        // Obtener todas las categorías
        [HttpGet("lista")]
        public async Task<ActionResult<IEnumerable<Categorium>>> GetCategorias()
        {
            return await _context.Categoria
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        // Obtener una categoría por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Categorium>> GetCategoria(int id)
        {
            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return categoria;
        }

        // Crear una nueva categoría
        [HttpPost]
        public async Task<ActionResult<Categorium>> PostCategoria(Categorium categoria)
        {
            _context.Categoria.Add(categoria);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, categoria);
        }

        // Actualizar una categoría
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, Categorium categoria)
        {
            if (id != categoria.Id)
            {
                return BadRequest();
            }

            _context.Entry(categoria).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> ActualizarCategoria(int id, Categorium categoria)
        //{
        //    if (id != categoria.Id)
        //    {
        //        return BadRequest("El ID de la categoría no coincide.");
        //    }

        //    _context.Entry(categoria).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!_context.Categoria.Any(e => e.Id == id))
        //        {
        //            return NotFound("La categoría no existe.");
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}


        // Eliminar una categoría
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            _context.Categoria.Remove(categoria);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
