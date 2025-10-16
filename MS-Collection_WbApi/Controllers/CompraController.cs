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
    public class CompraController : ControllerBase
    {
        private readonly JoyeriaMsBdContext _context;

        public CompraController(JoyeriaMsBdContext context)
        {
            _context = context;
        }

        [HttpPost("Crear")]
        public async Task<ActionResult<Compra>> CrearCompraConDetalles(Compra compra)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Verifica si el proveedor existe
                var proveedor = await _context.Proveedores.FindAsync(compra.ProveedorId);
                if (proveedor == null)
                {
                    return NotFound(new { mensaje = "Proveedor no encontrado" });
                }

                compra.Proveedor = proveedor;
                compra.FechaCompra = DateTime.Now;

                // Desacopla los detalles para guardarlos después de tener el ID de la compra
                var detalles = compra.DetalleCompras.ToList();

                // Limpia la colección para evitar problemas de tracking
                compra.DetalleCompras = new List<DetalleCompra>();

                // Agrega y guarda la compra primero
                _context.Compras.Add(compra);
                await _context.SaveChangesAsync(); // Esto genera el ID de la compra

                // Asigna el ID de la compra a cada detalle y actualiza el stock
                foreach (var detalle in detalles)
                {
                    detalle.CompraId = compra.Id;

                    // Buscar el producto asociado
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto == null)
                    {
                        return NotFound(new { mensaje = $"Producto con ID {detalle.ProductoId} no encontrado" });
                    }

                    // Sumar la cantidad al stock
                    producto.StockActual += detalle.Cantidad;

                    _context.DetalleCompras.Add(detalle);
                }

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(ObtenerCompra), new { id = compra.Id }, compra);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear la compra", error = ex.Message });
            }
        }


        //// GET: api/pedido/lista
        //[HttpGet("lista")]
        //public async Task<ActionResult<IEnumerable<Pedido>>> ListaPedidos()
        //{
        //    var pedidos = await _context.Pedidos
        //        .Include(p => p.Proveedor) // Incluir los datos del proveedor
        //        .ToListAsync();
        //    return Ok(pedidos);
        //}

        [HttpGet("lista")]
        public async Task<ActionResult<IEnumerable<CompraDTO>>> ListaCompras()
        {
            var compras = await _context.Compras
                .Include(p => p.Proveedor)
                .OrderByDescending(p => p.Id)
                // Asegúrate de incluir los datos del proveedor
                .ToListAsync();

            var compraDTOs = compras.Select(p => new CompraDTO
            {
                Id = p.Id,
                ProveedorId = p.ProveedorId,
                FechaCompra = p.FechaCompra,
                Total = p.Total,
                Proveedor = new ProveedorDTO
                {
                    Nombre = p.Proveedor?.Nombre,
                    Telefono = p.Proveedor?.Telefono,
                }
            }).ToList();

            return Ok(compraDTOs);
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<CompraDTO>>> BuscarCompras(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return BadRequest("Término de búsqueda inválido");

            var compras = await _context.Compras
                .Include(p => p.Proveedor)
                .Where(p =>
                    p.Id.ToString().Contains(termino) ||
                    p.Proveedor.Nombre.Contains(termino) ||
                     p.FechaCompra.ToString().Contains(termino)
                )
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            var compraDTOs = compras.Select(p => new CompraDTO
            {
                Id = p.Id,
                ProveedorId = p.ProveedorId,
                FechaCompra = p.FechaCompra,
                Total = p.Total,
                Proveedor = new ProveedorDTO
                {
                    Nombre = p.Proveedor?.Nombre,
                    Telefono = p.Proveedor?.Telefono,
                }
            }).ToList();

            return Ok(compraDTOs);
        }



        // GET: api/compra/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Compra>> ObtenerCompra(int id)
        {
            var compra = await _context.Compras
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (compra == null)
            {
                return NotFound(new { mensaje = "Compra no encontrado" });
            }

            return Ok(compra);
        }

        [HttpGet("detalle/{id}")]
        public async Task<ActionResult<IEnumerable<DetalleCompraDTO>>> ObtenerDetalleCompra(int id)
        {
            var detalles = await _context.DetalleCompras
                .Include(d => d.Producto)
                .Where(d => d.CompraId == id)
                .ToListAsync();

            if (detalles == null || !detalles.Any())
            {
                return NotFound(new { mensaje = "No se encontraron detalles para este compra" });
            }

            var detalleDTOs = detalles.Select(d => new DetalleCompraDTO
            {
                ProductoId = (int)d.ProductoId,
                NombreProducto = d.Producto?.Nombre,
                Cantidad = (int)d.Cantidad,
                PrecioUnitario = (decimal)d.PrecioUnitario

            }).ToList();

            return Ok(detalleDTOs);
        }


        [HttpPost]
        public async Task<ActionResult<Compra>> CrearCompra(Compra compra)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Buscar el proveedor por el proveedorId
                var proveedor = await _context.Proveedores.FindAsync(compra.ProveedorId);
                if (proveedor == null)
                {
                    return NotFound(new { mensaje = "Proveedor no encontrado" });
                }

                compra.Proveedor = proveedor; // Establecer la relación con el proveedor

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(ObtenerCompra), new { id = compra.Id }, compra);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el compra", error = ex.Message });
            }
        }


        // PUT: api/compra/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCompra(int id, Compra compra)
        {
            if (id != compra.Id)
            {
                return BadRequest(new { mensaje = "El ID del compra no coincide" });
            }

            var compraExistente = await _context.Compras.FindAsync(id);
            if (compraExistente == null)
            {
                return NotFound(new { mensaje = "Compra no encontrado" });
            }

            compraExistente.ProveedorId = compra.ProveedorId;
            compraExistente.FechaCompra = compra.FechaCompra;
            compraExistente.Total = compra.Total;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el compra" });
            }
        }

        // DELETE: api/compra/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCompra(int id)
        {
            var compra = await _context.Compras.FindAsync(id);
            if (compra == null)
            {
                return NotFound(new { mensaje = "Compra no encontrado" });
            }

            try
            {
                _context.Compras.Remove(compra);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el compra", error = ex.Message });
            }
        }
    }
}
