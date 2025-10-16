using System;
using System.Collections.Generic;

namespace MS_Collection_WbApi.Models;

public partial class MetodosDePago
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<PagoFactura> PagoFacturas { get; set; } = new List<PagoFactura>();
}
