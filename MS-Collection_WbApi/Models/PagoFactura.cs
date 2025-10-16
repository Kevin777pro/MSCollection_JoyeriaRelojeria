using System;
using System.Collections.Generic;

namespace MS_Collection_WbApi.Models;

public partial class PagoFactura
{
    public int Id { get; set; }

    public int? FacturaId { get; set; }

    public int? MetodoPagoId { get; set; }

    public decimal? Monto { get; set; }

    public virtual Factura? Factura { get; set; }

    public virtual MetodosDePago? MetodoPago { get; set; }
}
