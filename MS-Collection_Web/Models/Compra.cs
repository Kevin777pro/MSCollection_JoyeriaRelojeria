using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MS_Collection_Web.Models;

public partial class Compra
{
    public int Id { get; set; }

    public int ProveedorId { get; set; }

    public DateTime FechaCompra { get; set; }

    public decimal Total { get; set; }
    [JsonIgnore]  // 🔹 Evita que esta lista se serialice
    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual Proveedore Proveedor { get; set; } = null!;
}