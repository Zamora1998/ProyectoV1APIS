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
            if (Users.Usuarios.Any(u => u.NombreUsuario == registroUsuario.NombreUsuario))
            {
                return BadRequest("El nombre de usuario ya está en uso.");
            }

            // Encriptar la contraseña utilizando SHA-256 y truncar a 128 bits (16 bytes)
            var contrasenaEncriptada = EncriptarContrasena(registroUsuario.Contrasena);

            var nuevoUsuario = new Usuarios
            {
                NombreUsuario = registroUsuario.NombreUsuario,
                Nombre = registroUsuario.Nombre,
                Apellidos = registroUsuario.Apellidos,
                Email = registroUsuario.Email,
                Contrasena = contrasenaEncriptada, // Almacenar la contraseña encriptada
                IDEstado = 1,
                Token = null,
                TokenExpiracion = null
            };

            Users.Usuarios.Add(nuevoUsuario);
            Users.SaveChanges();

            return Ok();
        }

        // Función para encriptar la contraseña utilizando SHA-256 y truncar a 128 bits
        private string EncriptarContrasena(string contrasena)
        {
            using (var sha256 = SHA256.Create())
            {
                // Convierte la contraseña en una matriz de bytes
                byte[] contrasenaBytes = Encoding.UTF8.GetBytes(contrasena);

                // Calcula el hash SHA-256 de la contraseña
                byte[] hashBytes = sha256.ComputeHash(contrasenaBytes);

                // Trunca el hash a 16 bytes (128 bits)
                byte[] hashTruncado = new byte[16];
                Array.Copy(hashBytes, hashTruncado, 16);

                // Convierte el hash truncado en una cadena hexadecimal
                string hashHex = BitConverter.ToString(hashTruncado).Replace("-", "").ToLower();

                return hashHex;
            }
        }



        [HttpPut]
        [Route("api/UsuariosF/EditarUsuario")]
        public IHttpActionResult EditarUsuario(string nombreUsuario, EditarUsuario registroUsuario)
        {
            var usuarioExistente = Users.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);

            if (usuarioExistente == null)
            {
                return BadRequest();
            }

            // Actualizar los campos del usuario con los nuevos valores
            usuarioExistente.NombreUsuario = registroUsuario.NombreUsuario;
            usuarioExistente.Nombre = registroUsuario.Nombre;
            usuarioExistente.Apellidos = registroUsuario.Apellidos;
            usuarioExistente.Email = registroUsuario.Email;
            usuarioExistente.Contrasena = registroUsuario.Contrasena;

            // Guardar los cambios en la base de datos (si es necesario)
            Users.SaveChanges();

            return Ok();
        }


        [HttpDelete]
        [Route("api/UsuariosF/EliminarUsuarioPorNombre")]
        public IHttpActionResult EliminarUsuarioPorNombre(string nombreUsuario)
        {
            var usuarioAEliminar = Users.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);

            if (usuarioAEliminar == null)
            {
                return NotFound();
            }

            Users.Usuarios.Remove(usuarioAEliminar);
            Users.SaveChanges();

            if (usuarioAEliminar != null)
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
                // Actualizar el estado del usuario
                usuario.IDEstado = estadoUsuario.IDEstado;
                Users.SaveChanges();
                return Ok();
            }
            else
            {

                return StatusCode(HttpStatusCode.NoContent);
            }
        }
        [HttpPost]
        [Route("api/UsuariosF/ValidarLogin")]
        public IHttpActionResult ValidarLogin(loginUsuario login)
        {
            try
            {
                var usuario = Users.Usuarios.FirstOrDefault(u => u.NombreUsuario == login.NombreUsuario);

                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }

                var contrasenaEncriptada = EncriptarContrasena(login.Contrasena);

                if (usuario.Contrasena == contrasenaEncriptada)
                {
 
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
                        Expires = DateTime.UtcNow.AddHours(1), 
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
                        usuario.TokenExpiracion = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddHours(1), TimeZoneInfo.Local);

                        Users.SaveChanges();

                        var userInfo = new
                        {
                            UsuarioID = usuario.UsuarioID,
                            NombreUsuario = usuario.NombreUsuario,
                            Nombre = usuario.Nombre,
                            Apellidos = usuario.Apellidos,
                            Email = usuario.Email,
                            IDEstado = usuario.IDEstado,
                            Token = usuario.Token,
                            TokenExpiracion = usuario.TokenExpiracion
                        };

                        return Ok(userInfo);
                    }
                    else
                    {
                        return BadRequest("La clave generada excede la longitud permitida.");
                    }
                }
                else
                {
                    return BadRequest("Usuario o Contraseña Incorrecta.");
                }
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(eve => eve.ValidationErrors)
                    .Select(error => error.ErrorMessage);

                var errorMessage = string.Join(Environment.NewLine, errorMessages);

                return BadRequest($"Error de validación: {errorMessage}");
            }
        } 
    }
}
