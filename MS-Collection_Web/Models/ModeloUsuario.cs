
    public class ModeloUsuario
    {
    public int Id { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Direccion { get; set; }
    public string? Cedula { get; set; }
    public string? Email { get; set; }
    public string? User { get; set; }
    public string? Contraseña { get; set; }
    public string? Rol { get; set; } = "cliente";
    public bool? Estado { get; set; }
}

