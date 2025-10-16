using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MS_Collection_WbApi.Models
{
    public class VentasPorAnioDTO
    {
        [JsonPropertyName("anio")]
        public int Anio { get; set; }

        [JsonPropertyName("total")]
        public decimal Total { get; set; }
    }

    public class VentasDelDiaDTO
    {
        [JsonPropertyName("producto")]
        public string Producto { get; set; }

        [JsonPropertyName("cantidad")]
        public int Cantidad { get; set; }

        [JsonPropertyName("ingreso")]
        public decimal Ingreso { get; set; }

        [JsonPropertyName("precioVenta")]
        public decimal PrecioVenta { get; set; }
    }

    public class ProductosTopMesDTO
    {
        [JsonPropertyName("producto")]
        public string Producto { get; set; }

        [JsonPropertyName("cantidad")]
        public int Cantidad { get; set; }

        [JsonPropertyName("precioVenta")]
        public decimal PrecioVenta { get; set; }  // <-- Esta línea es la clave

        [JsonPropertyName("ingreso")]
        public decimal Ingreso { get; set; }
    }


    public class VentasPorMesDTO
    {
        [JsonPropertyName("mes")]
        public int Mes { get; set; }

        [JsonPropertyName("total")]
        public decimal Total { get; set; }
    }
}
