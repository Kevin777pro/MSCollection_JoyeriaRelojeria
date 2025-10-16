using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MS_Collection_WbApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MS_Collection_WbApi.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly JoyeriaMsBdContext _context;

        public InventarioController(JoyeriaMsBdContext context)
        {
            _context = context;
        }

        [HttpPost("registrar-movimiento")]
        public async Task<IActionResult> RegistrarMovimiento([FromBody] Inventario movimiento)
        {
            var producto = await _context.Productos.FindAsync(movimiento.ProductoId);
            if (producto == null)
                return NotFound("Producto no encontrado");

            if (!movimiento.EsEntrada && producto.StockActual < movimiento.Cantidad)
                return BadRequest("StockActual insuficiente para realizar la salida");

            // Actualizar StockActual
            producto.StockActual += movimiento.EsEntrada ? movimiento.Cantidad : -movimiento.Cantidad;

            movimiento.FechaRegistro = DateTime.Now;

            _context.Inventarios.Add(movimiento);
            await _context.SaveChangesAsync();

            return Ok("Movimiento registrado correctamente");
        }

        [HttpPost]
        public async Task<IActionResult> CrearInventario([FromBody] InventarioDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Obtener el producto
            var producto = await _context.Productos.FindAsync(dto.ProductoId);
            if (producto == null)
                return NotFound("Producto no encontrado");

            // Crear inventario
            var inventario = new Inventario
            {
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                EsEntrada = dto.EsEntrada,
                Observacion = dto.Observacion,
                FechaRegistro = dto.FechaRegistro,
                UsuarioId = dto.UsuarioId
            };

            _context.Inventarios.Add(inventario);

            // Actualizar el StockActualActual del producto
            if (dto.EsEntrada == true)
            {
                producto.StockActual += dto.Cantidad;
            }
            else
            {
                if (producto.StockActual < dto.Cantidad)
                    return BadRequest("No hay suficiente StockActual para esta salida.");

                producto.StockActual -= dto.Cantidad;
            }

            // Guardar cambios
            await _context.SaveChangesAsync();

            return Ok(inventario);
        }

        [HttpPost("movimientos-multiples")]
        public async Task<IActionResult> RegistrarMovimientosMultiples([FromBody] MovimientoInventarioMultipleRequest request)
        {
            foreach (var movimiento in request.Movimientos)
            {
                var inventario = new Inventario
                {
                    ProductoId = movimiento.ProductoId,
                    Cantidad = movimiento.Cantidad,
                    EsEntrada = movimiento.EsEntrada,
                    Observacion = movimiento.Observacion,
                    FechaRegistro = DateTime.UtcNow,
                    UsuarioId = request.UsuarioId
                };

                _context.Inventarios.Add(inventario);

                var producto = await _context.Productos.FindAsync(movimiento.ProductoId);
                if (producto == null) return NotFound($"Producto con ID {movimiento.ProductoId} no encontrado.");

                producto.StockActual += movimiento.EsEntrada ? movimiento.Cantidad : -movimiento.Cantidad;

                // Asegurarse que el StockActual no sea negativo (opcional)
                if (producto.StockActual < 0)
                    producto.StockActual = 0;
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Movimientos registrados con éxito" });
        }

        [HttpGet("movimientos")]
        public async Task<IActionResult> ObtenerMovimientos()
        {
            var movimientos = await _context.Inventarios
                .Include(i => i.Producto) // si querés incluir info del producto
                .OrderByDescending(i => i.FechaRegistro)
                .Select(i => new
                {
                    i.Id,
                    i.ProductoId,
                    ProductoNombre = i.Producto.Nombre, // asumiendo que tenés una propiedad Nombre
                    i.Cantidad,
                    i.EsEntrada,
                    i.Observacion,
                    i.FechaRegistro,
                    i.UsuarioId
                })
                .ToListAsync();

            return Ok(movimientos);
        }
    }

}