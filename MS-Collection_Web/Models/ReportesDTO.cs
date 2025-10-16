namespace MS_Collection_Web.Models
{
    public class VentasPorAnioDTO
    {
        public int Anio { get; set; }
        public decimal Total { get; set; }
    }

    public class ProductosTopMesDTO
    {
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }  // <-- ESTA propiedad
        public decimal Ingreso { get; set; }
    }

    public class VentasDelDiaDTO
    {
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }  // <-- ESTA propiedad
        public decimal Ingreso { get; set; }
    }


    public class VentasPorMesDTO
    {
        public int Mes { get; set; }
        public decimal Total { get; set; }
    }
}
