using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MS_Collection_WbApi.Models;

public partial class Categorium
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    [JsonIgnore]  // 🔹 Evita que esta lista se serialice
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
