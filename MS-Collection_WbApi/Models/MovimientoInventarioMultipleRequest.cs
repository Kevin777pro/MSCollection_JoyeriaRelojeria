public class MovimientoInventarioMultipleRequest
{
    public int UsuarioId { get; set; }
    public List<MovimientoInventarioRequest> Movimientos { get; set; }
}
