using System.Text.Json.Serialization;

public class CambioContrasenaDTO
{
    [JsonPropertyName("user")]
    public string User { get; set; }

    [JsonPropertyName("nuevaContrasena")]
    public string NuevaContrasena { get; set; }
}