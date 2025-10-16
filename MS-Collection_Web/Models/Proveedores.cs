using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MS_Collection_Web.Models;

public partial class Proveedore
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Contacto { get; set; }

    public string? Telefono { get; set; }
    [JsonIgnore]  // 🔹 Evita que esta lista se serialice
    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}