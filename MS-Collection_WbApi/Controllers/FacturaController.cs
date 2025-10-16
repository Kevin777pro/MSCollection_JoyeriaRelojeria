using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MS_Collection_WbApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MS_Collection_WbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturaController : ControllerBase
    {
        private readonly JoyeriaMsBdContext _context;

        public FacturaController(JoyeriaMsBdContext context)
        {
            _context = context;
        }

        // GET: api/factura/lista
        [HttpGet("lista")]
        public async Task<ActionResult<IEnumerable<object>>> ListaFacturas()
        {
            var facturas = await _context.Facturas
                .Include(f => f.PagoFacturas)
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Usuario)      // 🔹 Ahora sí obtenemos el usuario (cliente)
                .Select(f => new
                {
                    f.Id,
                    f.Fecha,
                    f.Total,
                    Cliente = new
                    {
                        f.Pedido.Usuario.Id,
                        f.Pedido.Usuario.NombreCompleto,
                        f.Pedido.Usuario.Email
                    },
                    Pedido = new
                    {
                        f.Pedido.Id
                    }
                })
                .ToListAsync();

            return Ok(facturas);
        }


        // GET: api/factura/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> ObtenerFactura(int id)
        {
            var factura = await _context.Facturas
                .Include(f => f.PagoFacturas)
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Usuario)      // 🔹 Incluir el cliente a través del Pedido
                .FirstOrDefaultAsync(f => f.Id == id);

            if (factura == null)
            {
                return NotFound(new { mensaje = "Factura no encontrada" });
            }

            return Ok(new
            {
                factura.Id,
                factura.Fecha,
                factura.Total,
                Cliente = factura.Pedido.Usuario != null ? new
                {
                    factura.Pedido.Usuario.Id,
                    factura.Pedido.Usuario.NombreCompleto,
                    factura.Pedido.Usuario.Email
                } : null,  // 🔹 Si no hay usuario, devuelve null
                Pedido = new
                {
                    factura.Pedido.Id
                }
            });
        }


        // POST: api/factura
        [HttpPost]
        public async Task<ActionResult<Factura>> CrearFactura(Factura factura)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerFactura), new { id = factura.Id }, factura);
        }

        // PUT: api/factura/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarFactura(int id, Factura factura)
        {
            if (id != factura.Id)
            {
                return BadRequest();
            }

            _context.Entry(factura).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Facturas.Any(e => e.Id == id))
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

        // DELETE: api/factura/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarFactura(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null)
            {
                return NotFound();
            }

            _context.Facturas.Remove(factura);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
