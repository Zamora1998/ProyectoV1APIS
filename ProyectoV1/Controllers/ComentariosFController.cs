using DataCliente;
using ProyectoV1.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProyectoV1.Controllers
{
    public class ComentariosFController : ApiController
    {
        private tiusr30pl_PeliculasProgra11Entities Coment = new tiusr30pl_PeliculasProgra11Entities();

        public ComentariosFController()
        {
            Coment.Configuration.LazyLoadingEnabled = true;
            Coment.Configuration.ProxyCreationEnabled = true;
        }

        [HttpPost]
        [Route("api/Comentarios/AgregarComentario")]
        public IHttpActionResult AgregarComentario(AgregarComentario comentario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            { 
                // Verificar si el usuario existe
                var usuarioExistente = Coment.Usuarios.FirstOrDefault(u => u.UsuarioID == comentario.UsuarioID);
                if (usuarioExistente == null)
                {
                    return BadRequest("El usuario especificado no existe.");
                } 
                // Crear una nueva instancia de Comentario utilizando los datos proporcionados
                var nuevoComentario = new Comentarios
                {
                    PeliculaID = comentario.PeliculaID,
                    UsuarioID = comentario.UsuarioID,
                    Comentario = comentario.Comentario,
                    FechaComentario = comentario.FechaComentario
                }; 
                // Agregar el comentario a la base de datos
                Coment.Comentarios.Add(nuevoComentario);
                Coment.SaveChanges(); 
                return Ok("Comentario agregado con éxito.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/Comentarios/EditarComentario")]
        public IHttpActionResult EditarComentario(EditarComentario datosEditados)
        {
            // Buscar el comentario por su identificador
            var comentarioExistente = Coment.Comentarios.FirstOrDefault(c => c.ComentarioID == datosEditados.ComentarioID);

            // Verificar si el comentario NO existe
            if (comentarioExistente == null)
            {
                return NotFound(); // Devolver un código 404 si el comentario no se encuentra
            }

            // Actualizar el campo de comentario con el nuevo valor
            comentarioExistente.PeliculaID = datosEditados.PeliculaID;
            comentarioExistente.Comentario = datosEditados.Comentario;
            comentarioExistente.FechaComentario = datosEditados.FechaComentario;
            comentarioExistente.RespuestaAComentarioID = datosEditados.RespuestaAComentarioID;

            // Guardar los cambios en la base de datos
            Coment.SaveChanges();

            return Ok("Comentario actualizado con éxito.");
        }

        [HttpDelete]
        [Route("api/Comentarios/EliminarComentario")]
        public IHttpActionResult EliminarComentario(int comentarioID)
        {
            var comentarioAEliminar = Coment.Comentarios.FirstOrDefault(c => c.ComentarioID == comentarioID);

            if (comentarioAEliminar == null)
            {
                return NotFound();
            } 
            // Eliminar el comentario de la base de datos
            Coment.Comentarios.Remove(comentarioAEliminar);
            Coment.SaveChanges();

            if (comentarioAEliminar != null)
            {
                return Ok(comentarioAEliminar); // Devuelve los detalles del comentario eliminado si es necesario
            }
            else
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
        }
    }
}