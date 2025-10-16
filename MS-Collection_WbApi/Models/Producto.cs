using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MS_Collection_WbApi.Models;

public partial class Producto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripción { get; set; } = null!;

    public string Codigo { get; set; } = null!;

    public int CategoriaId { get; set; }

    public string? Color { get; set; }

    public string? Material { get; set; }

    public string? Peso { get; set; }
    public int? StockActual { get; set; }
    public decimal PrecioVenta { get; set; }

    public string ImagenURL { get; set; } = null!;

    public virtual Categorium? Categoria { get; set; }

    [JsonIgnore]
    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    [JsonIgnore]
    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    [JsonIgnore]
    public virtual ICollection<Inventario> Inventarios { get; set; } = new List<Inventario>();
}