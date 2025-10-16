using System.Text.Json.Serialization;

public class CambioContraseñaPorUsuarioDTO
{
    [JsonPropertyName("user")]
    public string User { get; set; }

    [JsonPropertyName("nuevaContrasena")]
    public string NuevaContrasena { get; set; }
}