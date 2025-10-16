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
    public class ProductoController : ControllerBase
    {
        private readonly JoyeriaMsBdContext _context;

        public ProductoController(JoyeriaMsBdContext context)
        {
            _context = context;
        }

        // GET: api/producto/lista
        [HttpGet("lista")]
        public async Task<ActionResult<IEnumerable<Producto>>> ListaProductos()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .OrderByDescending(p => p.Id) // <-- Aquí se ordena
                .ToListAsync();

            return Ok(productos);
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Producto>>> BuscarProductos(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return BadRequest("Término de búsqueda inválido");

            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p =>
                    p.Nombre.Contains(termino) ||
                    p.Descripción.Contains(termino) ||
                    p.Categoria.Nombre.Contains(termino) ||
                    p.Material.Contains(termino)
                )
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return Ok(productos);
        }
        // GET: api/producto/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> ObtenerProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return NotFound(new { mensaje = "Producto no encontrado" });
            }

            return Ok(producto);
        }
        // GET: api/producto/buscar/{nombre}
        [HttpGet("buscar/{nombre}")]
        public async Task<ActionResult<IEnumerable<Producto>>> BuscarProductoPorNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest(new { mensaje = "Debe proporcionar un nombre válido para la búsqueda." });
            }

            var productos = await _context.Productos
                .Where(p => p.Nombre.Contains(nombre))
                .Include(p => p.Categoria)
                .ToListAsync();

            if (productos == null || productos.Count == 0)
            {
                return NotFound(new { mensaje = "No se encontraron productos que coincidan con el nombre proporcionado." });
            }

            return Ok(productos);
        }

        // POST: api/producto
        [HttpPost("Crear")]
        public async Task<ActionResult<Producto>> CrearProducto(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (producto.Categoria != null)
                {
                    _context.Entry(producto.Categoria).State = EntityState.Unchanged;
                }

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(ObtenerProducto), new { id = producto.Id }, producto);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al crear el producto",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // PUT: api/producto/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, Producto producto)
        {
            if (id != producto.Id)
            {
                return BadRequest(new { mensaje = "El ID del producto no coincide" });
            }

            var productoExistente = await _context.Productos.FindAsync(id);
            if (productoExistente == null)
            {
                return NotFound(new { mensaje = "Producto no encontrado" });
            }

            productoExistente.Nombre = producto.Nombre;
            productoExistente.Descripción = producto.Descripción;
            productoExistente.Codigo = producto.Codigo;
            productoExistente.CategoriaId = producto.CategoriaId;
            productoExistente.Color = producto.Color;
            productoExistente.Material = producto.Material;
            productoExistente.Peso = producto.Peso;
            productoExistente.PrecioVenta = producto.PrecioVenta;
            productoExistente.ImagenURL = producto.ImagenURL;
            productoExistente.StockActual = producto.StockActual; // ✅ Aquí actualizamos StockActual también

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el producto" });
            }
        }

        // DELETE: api/producto/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound(new { mensaje = "Producto no encontrado" });
            }

            try
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el producto", error = ex.Message });
            }
        }
    }
}
