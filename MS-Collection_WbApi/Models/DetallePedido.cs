using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MS_Collection_WbApi.Models;

public partial class DetallePedido
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }
    [JsonIgnore]

    public virtual Pedido? Pedido { get; set; }
    [JsonIgnore]

    public virtual Producto? Producto { get; set; }
}