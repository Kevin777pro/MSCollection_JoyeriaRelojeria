using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MS_Collection_WbApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MS_Collection_WbApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly JoyeriaMsBdContext _context;
        private readonly IConfiguration _configuration;
        public ReportesController(JoyeriaMsBdContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("ventas-por-anio")]
        public async Task<ActionResult<IEnumerable<VentasPorAnioDTO>>> GetVentasPorAnio()
        {
            var datos = await _context.Pedidos
                .Join(_context.DetallePedidos,
                      p => p.Id,
                      dp => dp.PedidoId,
                      (p, dp) => new { p.FechaPedido, dp.Cantidad, dp.PrecioUnitario })
                .GroupBy(x => x.FechaPedido.Value.Year)
                .Select(g => new VentasPorAnioDTO
                {
                    Anio = g.Key,
                    Total = g.Sum(x => x.Cantidad * x.PrecioUnitario)
                })
                .ToListAsync();

            return datos;
        }

        [HttpGet("ventas-del-dia")]
        public async Task<ActionResult<IEnumerable<VentasDelDiaDTO>>> GetVentasDelDia()
        {
            var hoy = DateTime.Today;
            var datos = await _context.Pedidos
                .Where(p => p.FechaPedido.Value.Date == hoy)
                .Join(_context.DetallePedidos, p => p.Id, dp => dp.PedidoId, (p, dp) => dp)
                .Join(_context.Productos, dp => dp.ProductoId, pr => pr.Id, (dp, pr) => new { dp, pr.Nombre, pr.PrecioVenta })
                .GroupBy(x => x.Nombre)
                .Select(g => new VentasDelDiaDTO
                {
                    Producto = g.Key,
                    Cantidad = g.Sum(x => x.dp.Cantidad),
                    PrecioVenta = g.First().PrecioVenta,
                    Ingreso = g.Sum(x => x.dp.Cantidad * x.PrecioVenta)
                })
                .ToListAsync();

            return datos;
        }

        [HttpGet("productos-top-mes")]
        public async Task<ActionResult<IEnumerable<ProductosTopMesDTO>>> GetProductosTopMes()
        {
            var fecha = DateTime.Today;
            var datos = await _context.Pedidos
                .Where(p => p.FechaPedido.Value.Month == fecha.Month && p.FechaPedido.Value.Year == fecha.Year)
                .Join(_context.DetallePedidos, p => p.Id, dp => dp.PedidoId, (p, dp) => dp)
                .Join(_context.Productos, dp => dp.ProductoId, pr => pr.Id, (dp, pr) => new { dp, pr.Nombre, pr.PrecioVenta })
                .GroupBy(x => x.Nombre)
                .Select(g => new ProductosTopMesDTO
                {
                    Producto = g.Key,
                    Cantidad = g.Sum(x => x.dp.Cantidad),
                    PrecioVenta = g.First().PrecioVenta,
                    Ingreso = g.Sum(x => x.dp.Cantidad * x.PrecioVenta)
                })
                .OrderByDescending(x => x.Cantidad)
                .Take(6)
                .ToListAsync();

            return datos;
        }

        [HttpGet("ventas-por-mes")]
        public async Task<ActionResult<IEnumerable<VentasPorMesDTO>>> GetVentasPorMes()
        {
            var anio = DateTime.Today.Year;
            var datos = await _context.Pedidos
                .Where(p => p.FechaPedido.Value.Year == anio)
                .Join(_context.DetallePedidos, p => p.Id, dp => dp.PedidoId, (p, dp) => new { p.FechaPedido, dp.Cantidad, dp.PrecioUnitario })
                .GroupBy(x => x.FechaPedido.Value.Month)
                .Select(g => new VentasPorMesDTO
                {
                    Mes = g.Key,
                    Total = g.Sum(x => x.Cantidad * x.PrecioUnitario)
                })
                .OrderBy(x => x.Mes)
                .ToListAsync();

            return datos;
        }
    }
}