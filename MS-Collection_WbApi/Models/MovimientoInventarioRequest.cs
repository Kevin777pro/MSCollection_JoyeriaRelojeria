public class MovimientoInventarioRequest
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public bool EsEntrada { get; set; }
    public string Observacion { get; set; }
}