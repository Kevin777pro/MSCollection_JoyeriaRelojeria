namespace MS_Collection_WbApi.Models
{
    public class CompraDTO
    {
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public DateTime? FechaCompra { get; set; }
        public decimal? Total { get; set; }

        // Usuario en el DTO, sin serializar las relaciones circulares
        public ProveedorDTO Proveedor { get; set; }
    }

    public class ProveedorDTO
    {
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
    }

}