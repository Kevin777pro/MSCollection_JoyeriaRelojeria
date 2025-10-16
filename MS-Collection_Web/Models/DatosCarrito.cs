using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using System.Collections.Generic;
using MS_Collection_WbApi.Models;

namespace MS_Collection_Web.Models
{
    public static class DatosCarrito
    {
        public static List<Producto> Productos { get; set; } = new List<Producto>();

        public static void Agregar(Producto producto)
        {
            var existente = Productos.Find(p => p.Codigo == producto.Codigo);
            if (existente != null)
            {
                existente.StockActual += 1; // usamos Stock como cantidad temporal
            }
            else
            {
                producto.StockActual = 1; // lo usamos como Cantidad
                Productos.Add(producto);
            }
        }

        public static void Limpiar()
        {
            Productos.Clear();
        }
    }
}