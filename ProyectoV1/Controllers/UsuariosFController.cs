using DataCliente;
using System.Linq;
using System.Web.Http;
using ProyectoV1.Models;
using System;
using System.Data;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Data.Entity.Validation;
using System.Net.Http.Formatting;

namespace ProyectoV1.Controllers
{
    public class UsuariosFController : ApiController
    {
        private tiusr30pl_PeliculasProgra11Entities Users = new tiusr30pl_PeliculasProgra11Entities();

        public UsuariosFController()
        {
            Users.Configuration.LazyLoadingEnabled = true;
            Users.Configuration.ProxyCreationEnabled = true;
        }
        [HttpGet]
        [Route("api/UsuariosF/GetUsuario/{id}")]

        public IHttpActionResult GetUsuario(int id)
        {
            var usuario = Users.Usuarios
                .Where(u => u.UsuarioID == id)
                .Select(u => new
                {
                    u.UsuarioID,
                    u.NombreUsuario,
                    u.Nombre,
                    u.Apellidos,
                    u.Email,
                    u.Contrasena,
                    u.IDEstado,
                    u.Token,
                    u.TokenExpiracion
                })
                .SingleOrDefault();

            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [HttpPost]
        [Route("api/UsuariosF/RegistrarUsuario")]
        public IHttpActionResult RegistrarUsuario(RegistroUsuario registroUsuario)
        {
            try
            {
                if (Users.Usuarios.Any(u => u.NombreUsuario == registroUsuario.NombreUsuario))
                {
                    return Conflict();
                }

                var contrasenaEncriptada = EncriptarContrasena(registroUsuario.Contrasena);

                var nuevoUsuario = new Usuarios
                {
                    NombreUsuario = registroUsuario.NombreUsuario,
                    Nombre = registroUsuario.Nombre,
                    Apellidos = registroUsuario.Apellidos,
                    Email = registroUsuario.Email,
                    Contrasena = contrasenaEncriptada,
                    IDEstado = 1,
                    Token = null,
                    TokenExpiracion = null
                };

                Users.Usuarios.Add(nuevoUsuario);
                Users.SaveChanges();

                var requestUri = Request.RequestUri;
                var newResourceUrl = new Uri(requestUri, $"api/UsuariosF/RegistrarUsuario/{nuevoUsuario.UsuarioID}");

                return Content(HttpStatusCode.Created, nuevoUsuario, new JsonMediaTypeFormatter(), "application/json");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/UsuariosF/EditarUsuario")]
        public IHttpActionResult EditarUsuario(string nombreUsuario, EditarUsuario registroUsuario, int? noReturn)
        {
            var usuarioExistente = Users.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);

            if (usuarioExistente == null)
            {
                return NotFound();
            }

            usuarioExistente.NombreUsuario = registroUsuario.NombreUsuario;
            usuarioExistente.Nombre = registroUsuario.Nombre;
            usuarioExistente.Apellidos = registroUsuario.Apellidos;
            usuarioExistente.Email = registroUsuario.Email;
            usuarioExistente.Contrasena = registroUsuario.Contrasena;

            Users.SaveChanges();

            if (noReturn == 1)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                var usuarioActualizado = new
                {
                    usuarioExistente.UsuarioID,
                    usuarioExistente.NombreUsuario,
                    usuarioExistente.Nombre,
                    usuarioExistente.Apellidos,
                    usuarioExistente.Email,
                    usuarioExistente.Contrasena
                };

                return Ok(usuarioActualizado);
            }
        }

        [HttpDelete]
        [Route("api/UsuariosF/EliminarUsuarioPorNombre")]
        public IHttpActionResult EliminarUsuarioPorNombre(string nombreUsuario, int? returnData)
        {
            var usuarioAEliminar = Users.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);

            if (usuarioAEliminar == null)
            {
                return NotFound();
            }

            Users.Usuarios.Remove(usuarioAEliminar);
            Users.SaveChanges();

            if (returnData == 1)
            {

                return Ok(usuarioAEliminar);
            }
            else
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [HttpPut]
        [Route("api/UsuariosF/ActualizarEstadoUsuario")]
        public IHttpActionResult ActualizarEstadoUsuario(ActivarDesacUser estadoUsuario)
        {
            var usuario = Users.Usuarios.FirstOrDefault(u => u.NombreUsuario == estadoUsuario.NombreUsuario);

            if (usuario == null)
            {
                return NotFound();
            }

            if (usuario.IDEstado != estadoUsuario.IDEstado)
            {
                usuario.IDEstado = estadoUsuario.IDEstado;
                Users.SaveChanges();

                if (estadoUsuario.noReturn == 1)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    var respuesta = new
                    {
                        IDEstado = usuario.IDEstado,
                    };

                    return Ok(respuesta);
                }
            }
            else
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [HttpPut]
        [Route("api/UsuariosF/ValidarLogin")]
        public IHttpActionResult ValidarLogin(loginUsuario login)
        {
            try
            {
                string contraVali = EncriptarContrasena(login.Contrasena);
                var usuario = Users.Usuarios.FirstOrDefault(u => u.NombreUsuario == login.NombreUsuario && u.Contrasena == contraVali);
                var estado = Users.Usuarios.FirstOrDefault(u => u.NombreUsuario == login.NombreUsuario && u.IDEstado != 1);

                if (estado != null)
                {
                    int? idestado = estado.IDEstado;
                    return Ok(idestado);
                }

                if (usuario == null)
                {
                    return NotFound(); // 404: Usuario no encontrado o contraseña incorrecta.
                }

                // Generar el token con una duración de 5 minutos (300 segundos)...
                var keyBytes = new byte[16];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(keyBytes);
                }
                var key = BitConverter.ToString(keyBytes).Replace("-", string.Empty);

                var claims = new[]
                {
            new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
        };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddSeconds(300), // Token expira en 5 minutos (300 segundos)
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                if (key.Length <= 50)
                {
                    usuario.Token = key;
                    usuario.TokenExpiracion = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddSeconds(login.TiempoToken), TimeZoneInfo.Local); // 5 minutos en segundos

                    Users.SaveChanges();

                    var userInfo = new
                    {
                        TokenExpiracion = usuario.TokenExpiracion,
                    };

                    return Ok(userInfo); // 200: Token creado exitosamente.
                }
                else
                {
                    return BadRequest(); // 400: Error al generar el token.
                }
            }
            catch (DbEntityValidationException ex)
            {
                return BadRequest("Error de validación: " + ex.Message); // 400: Error de validación.
            }
            catch (Exception)
            {
                return NotFound(); // 404: Otro error no especificado.
            }
        }

        private string EncriptarContrasena(string contrasena)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] contrasenaBytes = Encoding.UTF8.GetBytes(contrasena);

                byte[] hashBytes = sha256.ComputeHash(contrasenaBytes);
                byte[] hashTruncado = new byte[16];
                Array.Copy(hashBytes, hashTruncado, 16);
                string hashHex = BitConverter.ToString(hashTruncado).Replace("-", "").ToLower();

                return hashHex;
            }
        }
    }
}
