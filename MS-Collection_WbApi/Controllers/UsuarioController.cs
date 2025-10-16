using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MS_Collection_WbApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace MS_Collection_WbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly JoyeriaMsBdContext _context;
        private readonly IConfiguration _configuration;
        public UsuarioController(JoyeriaMsBdContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // 🔹 Obtener todos los usuarios
        [HttpGet("lista")]
        public async Task<ActionResult<IEnumerable<Usuario>>> ListaUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new {
                    u.Id,
                    u.NombreCompleto,
                    u.Email,
                    u.User,
                    u.Rol,
                    u.Estado,
                    u.Telefono,
                    u.Cedula,
                    u.Direccion

                }) // Evita exponer la contraseña
                .OrderByDescending(u => u.Id)
                .ToListAsync();
            return Ok(usuarios);
        }

        // 🔹 Obtener un usuario por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> ObtenerUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.Id == id)
                .Select(u => new {
                    u.Id,
                    u.NombreCompleto,
                    u.Email,
                    u.User,
                    u.Rol,
                    u.Estado,
                    u.Telefono
                }) // Evita exponer la contraseña
                .FirstOrDefaultAsync();

            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado" });
            }
            return Ok(usuario);
        }

        // 🔹 Crear un nuevo usuario
        [HttpPost("crear")]
        public async Task<ActionResult<Usuario>> CrearUsuario([FromBody] Usuario usuario)
        {
            if (usuario == null ||
                string.IsNullOrEmpty(usuario.NombreCompleto) ||
                string.IsNullOrEmpty(usuario.Email) ||
                string.IsNullOrEmpty(usuario.User) ||
                string.IsNullOrEmpty(usuario.Contraseña)) // Asegurate de que la contraseña venga en el body
            {
                return BadRequest(new { mensaje = "Datos inválidos. Nombre, Email, User y Password son obligatorios." });
            }

            try
            {
                // Hasheamos la contraseña antes de guardar
                var passwordHasher = new PasswordHasher<Usuario>();
                usuario.Contraseña = passwordHasher.HashPassword(usuario, usuario.Contraseña);

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Evitamos retornar el hash
                return CreatedAtAction(nameof(ObtenerUsuario), new { id = usuario.Id }, new
                {
                    usuario.Id,
                    usuario.NombreCompleto,
                    usuario.Email,
                    usuario.User,
                    usuario.Rol,
                    usuario.Estado,
                    usuario.Telefono,
                    usuario.Contraseña,
                    usuario.Direccion,
                    usuario.Cedula
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { mensaje = "Error al guardar el usuario", error = ex.Message });
            }
        }



        // 🔹 Actualizar un usuario
        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] Usuario usuarioActualizado)
        {
            if (id != usuarioActualizado.Id)
            {
                return BadRequest(new { mensaje = "El ID proporcionado no coincide con el usuario" });
            }

            try
            {
                var usuarioExistente = await _context.Usuarios.FindAsync(id);
                if (usuarioExistente == null)
                {
                    return NotFound(new { mensaje = "Usuario no encontrado" });
                }

                // Actualización segura (evita cambiar la contraseña accidentalmente)
                usuarioExistente.NombreCompleto = usuarioActualizado.NombreCompleto;
                usuarioExistente.Email = usuarioActualizado.Email;
                usuarioExistente.User = usuarioActualizado.User;
                usuarioExistente.Rol = usuarioActualizado.Rol;
                usuarioExistente.Estado = usuarioActualizado.Estado;
                usuarioExistente.Telefono = usuarioActualizado.Telefono;
                usuarioExistente.Cedula = usuarioActualizado.Cedula;
                usuarioExistente.Direccion = usuarioActualizado.Direccion;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { mensaje = "Error de concurrencia al actualizar el usuario" });
            }
        }

        [HttpPut("CambiarContraseñaPorId/")]
        public async Task<IActionResult> CambiarContraseñaPorId(int id, [FromBody] CambioContraseñaDTO datos)
        {
            if (string.IsNullOrWhiteSpace(datos.NuevaContrasena))
            {
                return BadRequest(new { mensaje = "La nueva contraseña no puede estar vacía." });
            }

            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { mensaje = "Usuario no encontrado" });
                }

                // Usar el mismo hasher de ASP.NET Core Identity
                var passwordHasher = new PasswordHasher<Usuario>();
                usuario.Contraseña = passwordHasher.HashPassword(usuario, datos.NuevaContrasena);

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Contraseña actualizada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la contraseña", error = ex.Message });
            }
        }

        [HttpPut("CambiarContrasenaPorUsuario")]
        public async Task<IActionResult> CambiarContraseñaPorUsuario([FromBody] CambioContraseñaPorUsuarioDTO datos)
        {
            if (string.IsNullOrWhiteSpace(datos.NuevaContrasena) || string.IsNullOrWhiteSpace(datos.User))
            {
                return BadRequest(new { mensaje = "El usuario y la nueva contraseña no pueden estar vacíos." });
            }

            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.User == datos.User);
                if (usuario == null)
                {
                    return NotFound(new { mensaje = "Usuario no encontrado" });
                }

                var passwordHasher = new PasswordHasher<Usuario>();
                usuario.Contraseña = passwordHasher.HashPassword(usuario, datos.NuevaContrasena);

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Contraseña actualizada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la contraseña", error = ex.Message });
            }
        }

        [HttpGet("BuscarPorUsuario/{nombreUsuario}")]
        public async Task<IActionResult> BuscarPorUsuario(string nombreUsuario)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.User.ToLower() == nombreUsuario.Trim().ToLower());

            if (usuario == null) return NotFound("No se encontró el usuario");

            return Ok(usuario);
        }

        // 🔹 Eliminar un usuario
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado" });
            }

            try
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Usuario eliminado correctamente" });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el usuario", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] Login loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.User) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new { mensaje = "Usuario y contraseña son obligatorios." });
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.User == loginRequest.User);

            if (usuario == null)
            {
                return Unauthorized(new { mensaje = "Credenciales inválidas." });
            }

            var passwordHasher = new PasswordHasher<Usuario>();
            var resultado = passwordHasher.VerifyHashedPassword(usuario, usuario.Contraseña, loginRequest.Password);

            if (resultado == PasswordVerificationResult.Success)
            {
                var jwtHelper = new JwtHelper(_configuration);
                var token = jwtHelper.GenerarToken(usuario);

                return Ok(new
                {
                    token = token,
                    user = usuario.User,
                    id = usuario.Id, // 👈 Aquí agregamos el ID
                    rol = usuario.Rol
                });
            }

            return Unauthorized(new { mensaje = "Credenciales inválidas." });
        }

        // 🔹 Actualizar datos del perfil sin contraseña
        [HttpPut("actualizar-perfil/{id}")]
        public async Task<IActionResult> ActualizarPerfil(int id, [FromBody] Usuario usuarioActualizado)
        {
            if (id != usuarioActualizado.Id)
            {
                return BadRequest(new { mensaje = "El ID proporcionado no coincide con el usuario" });
            }

            try
            {
                var usuarioExistente = await _context.Usuarios.FindAsync(id);
                if (usuarioExistente == null)
                {
                    return NotFound(new { mensaje = "Usuario no encontrado" });
                }

                // Solo actualiza los campos del perfil (sin tocar la contraseña)
                usuarioExistente.NombreCompleto = usuarioActualizado.NombreCompleto;
                usuarioExistente.Email = usuarioActualizado.Email;
                usuarioExistente.User = usuarioActualizado.User;
                usuarioExistente.Rol = usuarioActualizado.Rol;
                usuarioExistente.Estado = usuarioActualizado.Estado;
                usuarioExistente.Cedula = usuarioActualizado.Cedula;
                usuarioExistente.Direccion = usuarioActualizado.Direccion;
                usuarioExistente.Telefono = usuarioActualizado.Telefono;

                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Perfil actualizado correctamente" });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { mensaje = "Error de concurrencia al actualizar el perfil" });
            }
        }

        // Métodos de validación

        [HttpGet("existeCedula/{cedula}")]
        public async Task<ActionResult<bool>> ExisteCedula(string cedula)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.Cedula == cedula);
            return Ok(existe);
        }

        [HttpGet("existeCorreo/{correo}")]
        public async Task<ActionResult<bool>> ExisteCorreo(string correo)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.Email == correo);
            return Ok(existe);
        }

        [HttpGet("existeUser/{user}")]
        public async Task<ActionResult<bool>> ExisteUsuario(string user)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.User == user);
            return Ok(existe);
        }

        [HttpGet("existeTelefono/{telefono}")]
        public async Task<ActionResult<bool>> ExisteTelefono(string telefono)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.Telefono == telefono);
            return Ok(existe);
        }
    }
}
