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
    public class PedidoController : ControllerBase
    {
        private readonly JoyeriaMsBdContext _context;

        public PedidoController(JoyeriaMsBdContext context)
        {
            _context = context;
        }

        [HttpPost("Crear")]
        public async Task<ActionResult<Pedido>> CrearPedidoConDetalles(Pedido pedido)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Verifica si el usuario existe
                var usuario = await _context.Usuarios.FindAsync(pedido.ClienteId);
                if (usuario == null)
                {
                    return NotFound(new { mensaje = "Usuario no encontrado" });
                }

                pedido.Usuario = usuario;
                pedido.FechaPedido = DateTime.Now;

                // Desacopla los detalles para guardarlos después de tener el ID del pedido
                var detalles = pedido.DetallePedidos.ToList();

                // Limpia la colección para evitar problemas de tracking
                pedido.DetallePedidos = new List<DetallePedido>();

                // Agrega y guarda el pedido primero
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync(); // Esto genera el ID del pedido

                // Asigna el ID del pedido a cada detalle y actualiza el stock
                foreach (var detalle in detalles)
                {
                    detalle.PedidoId = pedido.Id;

                    // Buscar el producto asociado
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto == null)
                    {
                        return NotFound(new { mensaje = $"Producto con ID {detalle.ProductoId} no encontrado" });
                    }

                    // Verificar si hay suficiente stock
                    if (producto.StockActual < detalle.Cantidad)
                    {
                        return BadRequest(new { mensaje = $"Stock insuficiente para el producto {producto.Nombre}" });
                    }

                    // Restar la cantidad del stock
                    producto.StockActual -= detalle.Cantidad;

                    _context.DetallePedidos.Add(detalle);
                }

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(ObtenerPedido), new { id = pedido.Id }, pedido);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el pedido", error = ex.Message });
            }
        }


        //// GET: api/pedido/lista
        //[HttpGet("lista")]
        //public async Task<ActionResult<IEnumerable<Pedido>>> ListaPedidos()
        //{
        //    var pedidos = await _context.Pedidos
        //        .Include(p => p.Usuario) // Incluir los datos del cliente
        //        .ToListAsync();
        //    return Ok(pedidos);
        //}

        [HttpGet("lista")]
        public async Task<ActionResult<IEnumerable<PedidoDTO>>> ListaPedidos()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Usuario)
                .OrderByDescending(p => p.Id) // <-- Aquí se ordena
                .ToListAsync();

            var pedidoDTOs = pedidos.Select(p => new PedidoDTO
            {
                Id = p.Id,
                ClienteId = p.ClienteId,
                FechaPedido = p.FechaPedido,
                Estado = p.Estado,
                Total = p.Total,
                Usuario = new UsuarioDTO
                {
                    NombreCompleto = p.Usuario?.NombreCompleto,
                    Telefono = p.Usuario?.Telefono,
                    Direccion = p.Usuario?.Direccion
                }
            }).ToList();

            return Ok(pedidoDTOs);
        }

        [HttpGet("detalle/{id}")]
        public async Task<ActionResult<IEnumerable<DetallePedidoDTO>>> ObtenerDetallePedido(int id)
        {
            var detalles = await _context.DetallePedidos
                .Include(d => d.Producto)
                .Where(d => d.PedidoId == id)
                .ToListAsync();

            if (detalles == null || !detalles.Any())
            {
                return NotFound(new { mensaje = "No se encontraron detalles para este pedido" });
            }

            var detalleDTOs = detalles.Select(d => new DetallePedidoDTO
            {
                ProductoId = d.ProductoId,
                NombreProducto = d.Producto?.Nombre,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario
           
            }).ToList();

            return Ok(detalleDTOs);
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<PedidoDTO>>> BuscarPedidos(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return BadRequest("Término de búsqueda inválido");

            var pedidos = await _context.Pedidos
                .Include(p => p.Usuario)
                .Where(p =>
                    p.Usuario.NombreCompleto.Contains(termino) ||
                    p.Usuario.Direccion.Contains(termino) ||
                    p.Id.ToString().Contains(termino) ||
                      p.Estado.ToString().Contains(termino) 

                )
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            var pedidoDTOs = pedidos.Select(p => new PedidoDTO
            {
                Id = p.Id,
                ClienteId = p.ClienteId,
                FechaPedido = p.FechaPedido,
                Estado = p.Estado,
                Total = p.Total,
                Usuario = new UsuarioDTO
                {
                    NombreCompleto = p.Usuario?.NombreCompleto,
                    Telefono = p.Usuario?.Telefono,
                    Direccion = p.Usuario?.Direccion
                }
            }).ToList();

            return Ok(pedidoDTOs);
        }

        // GET: api/pedido/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> ObtenerPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound(new { mensaje = "Pedido no encontrado" });
            }

            return Ok(pedido);
        }

        [HttpPost]
        public async Task<ActionResult<Pedido>> CrearPedido(Pedido pedido)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Buscar el usuario por el clienteId
                var usuario = await _context.Usuarios.FindAsync(pedido.ClienteId);
                if (usuario == null)
                {
                    return NotFound(new { mensaje = "Usuario no encontrado" });
                }

                pedido.Usuario = usuario; // Establecer la relación con el usuario

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(ObtenerPedido), new { id = pedido.Id }, pedido);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el pedido", error = ex.Message });
            }
        }


        // PUT: api/pedido/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarPedido(int id, Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return BadRequest(new { mensaje = "El ID del pedido no coincide" });
            }

            var pedidoExistente = await _context.Pedidos.FindAsync(id);
            if (pedidoExistente == null)
            {
                return NotFound(new { mensaje = "Pedido no encontrado" });
            }

            pedidoExistente.ClienteId = pedido.ClienteId;
            pedidoExistente.FechaPedido = pedido.FechaPedido;
            pedidoExistente.Estado = pedido.Estado;
            pedidoExistente.Total = pedido.Total;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el pedido" });
            }
        }

        // DELETE: api/pedido/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound(new { mensaje = "Pedido no encontrado" });
            }

            try
            {
                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el pedido", error = ex.Message });
            }
        }
    }
}
