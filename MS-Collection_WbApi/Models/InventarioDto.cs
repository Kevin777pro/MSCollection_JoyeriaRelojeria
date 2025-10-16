public class InventarioDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public bool EsEntrada { get; set; }
    public string? Observacion { get; set; }
    public DateTime FechaRegistro { get; set; }
    public int UsuarioId { get; set; }
}
