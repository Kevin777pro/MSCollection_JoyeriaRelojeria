namespace MS_Collection_WbApi.Models
{
    public class PedidoDTO
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public DateTime? FechaPedido { get; set; }
        public string? Estado { get; set; }
        public decimal? Total { get; set; }

        // Usuario en el DTO, sin serializar las relaciones circulares
        public UsuarioDTO Usuario { get; set; }
    }

    public class UsuarioDTO
    {
        public string? NombreCompleto { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
    }

}