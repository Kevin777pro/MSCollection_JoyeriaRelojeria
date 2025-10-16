using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MS_Collection_WbApi.Models
{
    public partial class Factura
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int PedidoId { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal? Total { get; set; }

        // 🔹 Corrección: Pedido es de tipo Pedido, no Usuario
        public virtual Pedido Pedido { get; set; } = null!;

        [JsonIgnore]  // 🔹 Evita que esta lista se serialice
        public virtual ICollection<PagoFactura> PagoFacturas { get; set; } = new List<PagoFactura>();
    }
}
