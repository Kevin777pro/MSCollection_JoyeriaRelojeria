using MS_Collection_WbApi.Models;
using System;
using System.Collections.Generic;

namespace MS_Collection_Web.Models;

public partial class DetalleCompra
{
    public int Id { get; set; }

    public int? CompraId { get; set; }

    public int? ProductoId { get; set; }

    public int? Cantidad { get; set; }

    public decimal? PrecioUnitario { get; set; }

    public virtual Compra? Compra { get; set; }

    public virtual Producto? Producto { get; set; }
}