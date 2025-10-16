using System;
using System.Collections.Generic;

namespace MS_Collection_WbApi.Models;

public partial class Inventario
{
    public int Id { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public bool EsEntrada { get; set; } // true = entrada, false = salida

    public string? Observacion { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public int UsuarioId { get; set; }

    public virtual Producto Producto { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
}
