using DataCliente;
using ProyectoV1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web.Http;

namespace ProyectoV1.Controllers
{
    public class PeliculasFController : ApiController
    {
        private tiusr30pl_PeliculasProgra11Entities Pelis = new tiusr30pl_PeliculasProgra11Entities();

        public PeliculasFController()
        {
            Pelis.Configuration.LazyLoadingEnabled = true;
            Pelis.Configuration.ProxyCreationEnabled = true;
        }

        [HttpGet]
        [Route("api/Peliculas/GetPeliculaPorId/{id}")]
        public IHttpActionResult GetPeliculaPorId(int id)
        {
            var pelicula = Pelis.Peliculas
                .Where(p => p.PeliculaID == id)
                .Select(p => new PeliculaDto
                {
                    PeliculaID = p.PeliculaID,
                    Nombre = p.Nombre,
                    Resena = p.Resena,
                    CalificacionGenerQal = p.CalificacionGenerQal,
                    FechaLanzamiento = p.FechaLanzamiento,
                    PosterID = p.PosterID,
                    Involucrados = p.RolesEnPelicula.Select(r => new InvolucradoDto
                    {
                        ActorStaffID = (int)r.ActorStaffID,
                        Nombre = r.ActoresStaff.Nombre,
                        PaginaWeb = r.ActoresStaff.PaginaWeb,
                        Facebook = r.ActoresStaff.Facebook,
                        Instagram = r.ActoresStaff.Instagram,
                        Twitter = r.ActoresStaff.Twitter
                    }).ToList(),
                    Comentarios = p.Comentarios.Select(c => new ComentarioDto
                    {
                        ComentarioID = c.ComentarioID,
                        PeliculaID = c.PeliculaID.Value,
                        UsuarioID = c.UsuarioID.Value,
                        Comentario = c.Comentario,
                        FechaComentario = c.FechaComentario.Value,
                        RespuestaAComentarioID = c.RespuestaAComentarioID.Value
                    }).ToList(),
                    Calificaciones = p.CalificacionesExpertos.Select(cal => new CalificacionDto
                    {
                        PeliculaID = cal.PeliculaID,
                        ExpertoID = cal.ExpertoID,
                        Calificacion = (decimal)cal.Calificacion
                    }).ToList()
                })
                .SingleOrDefault();

            if (pelicula == null)
            {
                return NotFound();
            }

            return Ok(pelicula);
        }

        [HttpGet]
        [Route("api/PeliculasF/GetPeliculas")]
        public IHttpActionResult GetPeliculas()
        {
            var peliculas = Pelis.Peliculas
                .OrderByDescending(p => p.FechaLanzamiento)
                .Take(5)
                .Select(p => new PeliculaDto
                {
                    PeliculaID = p.PeliculaID,
                    Nombre = p.Nombre,
                    Resena = p.Resena,
                    CalificacionGenerQal = p.CalificacionGenerQal,
                    FechaLanzamiento = p.FechaLanzamiento,
                    PosterID = p.PosterID,
                    Involucrados = p.RolesEnPelicula.Select(r => new InvolucradoDto
                    {
                        ActorStaffID = (int)r.ActorStaffID,
                        Nombre = r.ActoresStaff.Nombre,
                        PaginaWeb = r.ActoresStaff.PaginaWeb,
                        Facebook = r.ActoresStaff.Facebook,
                        Instagram = r.ActoresStaff.Instagram,
                        Twitter = r.ActoresStaff.Twitter
                    }).ToList(),
                    Comentarios = p.Comentarios.Select(c => new ComentarioDto
                    {
                        ComentarioID = c.ComentarioID,
                        PeliculaID = c.PeliculaID.Value,
                        UsuarioID = c.UsuarioID.Value,
                        Comentario = c.Comentario,
                        FechaComentario = c.FechaComentario.Value,
                        RespuestaAComentarioID = c.RespuestaAComentarioID.Value
                    }).ToList(),
                    Calificaciones = p.CalificacionesExpertos.Select(cal => new CalificacionDto
                    {
                        PeliculaID = cal.PeliculaID,
                        ExpertoID = cal.ExpertoID,
                        Calificacion = (decimal)cal.Calificacion
                    }).ToList()
                })
                .ToList();

            if (peliculas == null || peliculas.Count == 0)
            {
                return NotFound();
            }

            return Ok(peliculas);
        }

        [HttpPost]
        [Route("api/Peliculas/RegistrarPelicula")]
        public IHttpActionResult RegistrarPelicula(Resgistrarpelicula nuevaPelicula)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar si la película ya existe por su nombre
            var peliculaExistente = Pelis.Peliculas.FirstOrDefault(p => p.Nombre == nuevaPelicula.Nombre);

            if (peliculaExistente != null)
            {
                return Conflict(); // Devolver un código 409 Conflict si la película ya existe
            }

            // Crear una nueva película a partir de los datos proporcionados
            var pelicula = new Peliculas
            {
                Nombre = nuevaPelicula.Nombre,
                Resena = nuevaPelicula.Resena,
                CalificacionGenerQal = nuevaPelicula.CalificacionGenerQal,
                FechaLanzamiento = nuevaPelicula.FechaLanzamiento,
                PosterID = nuevaPelicula.PosterID
            };

            // Agregar la película a la base de datos
            Pelis.Peliculas.Add(pelicula);
            Pelis.SaveChanges();

            // Devolver un código 201 Created y la película recién creada
            return Created(Request.RequestUri + "/" + pelicula.PeliculaID, pelicula);
        }
    
    [HttpPut]
        [Route("api/PeliculasF/EditarPelicula")]
        public IHttpActionResult EditarPelicula(EditarPelicula datosEditados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var peliculaExistente = Pelis.Peliculas.FirstOrDefault(p => p.PeliculaID == datosEditados.PeliculaID);

            if (peliculaExistente == null)
            {
                return NotFound(); // Devolver un código 404 si la película no se encuentra
            }

            peliculaExistente.Nombre = datosEditados.Nombre;
            peliculaExistente.Resena = datosEditados.Resena;
            peliculaExistente.CalificacionGenerQal = datosEditados.CalificacionGenerQal;
            peliculaExistente.FechaLanzamiento = datosEditados.FechaLanzamiento;
            peliculaExistente.PosterID = datosEditados.PosterID;

            Pelis.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("api/PeliculasF/EliminarPeliculaPorCodigo")]
        public IHttpActionResult EliminarPeliculaPorCodigo(int idpeli )
        {
            var peliculaAEliminar = Pelis.Peliculas.FirstOrDefault(p => p.PeliculaID == idpeli);

            if (peliculaAEliminar == null)
            {
                return NotFound();
            }

            try
            {
                // Eliminar los datos relacionados en RolesEnPelicula
                var rolesRelacionados = Pelis.RolesEnPelicula.Where(r => r.IDPelic == peliculaAEliminar.PeliculaID).ToList();
                Pelis.RolesEnPelicula.RemoveRange(rolesRelacionados);

                // Eliminar la película de la base de datos
                Pelis.Peliculas.Remove(peliculaAEliminar);
                Pelis.SaveChanges();

                if (peliculaAEliminar != null)
                {
                    return Ok(peliculaAEliminar); // Devuelve los detalles de la película eliminada si es necesario
                }
                else
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
            }
            catch (DbUpdateException ex)
            {
                // Loguear o imprimir la excepción interna para conocer la causa exacta del error
                Console.WriteLine(ex.InnerException.Message);
                return InternalServerError(ex); // Devolver un error interno del servidor junto con el mensaje de error
            }
        }
    }
}
