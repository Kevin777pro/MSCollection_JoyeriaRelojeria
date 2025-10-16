using MS_Collection_WbApi.Models;
using System.Text.Json.Serialization;

public partial class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime? FechaPedido { get; set; }
    public string? Estado { get; set; }
    public decimal? Total { get; set; }

    [JsonIgnore]  // Esto evitará la serialización circular
    public virtual Usuario? Usuario { get; set; }  // Usar un tipo nullable

    public List<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

}