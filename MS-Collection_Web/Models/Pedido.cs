using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MS_Collection_Web.Models;

public partial class Pedido
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public DateTime? FechaPedido { get; set; }

    public string? Estado { get; set; }

    public decimal? Total { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
   // [JsonIgnore]  // 🔹 Evita que esta lista se serialice
    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
}