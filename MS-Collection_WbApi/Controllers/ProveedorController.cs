using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MS_Collection_WbApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MS_Collection_WbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly JoyeriaMsBdContext _context;

        public ProveedorController(JoyeriaMsBdContext context)
        {
            _context = context;
        }

        // GET: api/proveedor/lista
        [HttpGet("lista")]
        public async Task<ActionResult<IEnumerable<Proveedore>>> ListaProveedores()
        {
            var proveedores = await _context.Proveedores
                .OrderByDescending(p => p.Id) // <-- Aquí se ordena
                .ToListAsync();
            return Ok(proveedores);
        }

        // GET: api/proveedor/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedore>> ObtenerProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound();
            }

            return Ok(proveedor);
        }

        // POST: api/proveedor
        [HttpPost]
        public async Task<ActionResult<Proveedore>> CrearProveedor(Proveedore proveedor)
        {
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerProveedor), new { id = proveedor.Id }, proveedor);
        }

        // PUT: api/proveedor/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarProveedor(int id, Proveedore proveedor)
        {
            if (id != proveedor.Id)
            {
                return BadRequest();
            }

            _context.Entry(proveedor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Proveedores.Any(e => e.Id == id))
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

        // DELETE: api/proveedor/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
