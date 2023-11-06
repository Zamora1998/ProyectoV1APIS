using DataCliente;
using ProyectoV1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
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
        [Route("api/PeliculasF/GetPeliculas")]
        public IHttpActionResult GetPeliculas()
        {
            var peliculas = Pelis.Peliculas
                .OrderByDescending(p => p.FechaLanzamiento) // Ordena por fecha de lanzamiento en orden descendente (las más recientes primero)
                .Take(5) // Limita a las 5 películas más recientes
                .Select(p => new
                {
                    PeliculaID = p.PeliculaID,
                    Nombre = p.Nombre,
                    Resena = p.Resena,
                    CalificacionGenerQal = p.CalificacionGenerQal,
                    FechaLanzamiento = p.FechaLanzamiento,
                    PosterID = p.PosterID
                })
                .ToList();

            if (peliculas == null || peliculas.Count == 0)
            {
                return NotFound();
            }

            var peliculasConRutaPoster = new List<GetPeliculas>();

            // Obtener la URL base de la solicitud actual
            var request = HttpContext.Current.Request;
            var baseUrl = request.Url.GetLeftPart(UriPartial.Authority);

            foreach (var pelicula in peliculas)
            {
                var poster = Pelis.Posters.FirstOrDefault(p => p.PosterID == pelicula.PosterID);

                if (poster != null)
                {
                    var peliculaConRuta = new GetPeliculas
                    {
                        PeliculaID = pelicula.PeliculaID,
                        Nombre = pelicula.Nombre,
                        Resena = pelicula.Resena,
                        CalificacionGenerQal = pelicula.CalificacionGenerQal,
                        FechaLanzamiento = pelicula.FechaLanzamiento,
                        PosterID = pelicula.PosterID,
                        RutaPoster = baseUrl + "/APIV3" + poster.RutaArchivo // Construir la URL completa
                    };

                    peliculasConRutaPoster.Add(peliculaConRuta);
                }
            }

            return Ok(peliculasConRutaPoster);
        }


        [HttpGet]
        [Route("api/PeliculasF/GetPeliculasNombre")]
        public IHttpActionResult GetPeliculas([FromUri] string nombrePelicula)
        {
            // Realiza una búsqueda insensible a mayúsculas y minúsculas
            var peliculasEncontradas = Pelis.Peliculas
                .Where(p => p.Nombre.ToLower().Contains(nombrePelicula.ToLower()))
                .Select(p => p.Nombre)
                .ToList();

            if (peliculasEncontradas.Count == 0)
            {
                return NotFound();
            }

            return Ok(peliculasEncontradas);
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
        [HttpGet]
        [Route("api/PeliculaDetalle/ObtenerDetallesPorNombre")]
        public IHttpActionResult ObtenerDetallesPorNombre(string nombrePelicula)
        {
            // Buscar la película por su nombre
            var pelicula = Pelis.Peliculas
                .FirstOrDefault(p => p.Nombre.ToLower() == nombrePelicula.ToLower());

            if (pelicula == null)
            {
                return NotFound(); // Devolver 404 si la película no se encuentra
            }
            var poster = Pelis.Posters.FirstOrDefault(p => p.PosterID == pelicula.PosterID);
            var rutaPoster = poster != null ? poster.RutaArchivo : null;

            // Obtener comentarios de la película
            var comentarios = Pelis.Comentarios
                .Where(c => c.PeliculaID == pelicula.PeliculaID)
                .Select(c => new ComentarioDto
                {
                    //ComentarioID = c.ComentarioID,
                    //PeliculaID = c.PeliculaID,
                    UsuarioID = c.UsuarioID,
                    Comentario = c.Comentario,
                    FechaComentario = c.FechaComentario,
        })
                .ToList();
            var request = HttpContext.Current.Request;
            var baseUrl = request.Url.GetLeftPart(UriPartial.Authority);
            var rutaImagenCompleta = baseUrl + "/APIV3" + rutaPoster;

            var peliculaDetalle = new
            {
                PeliculaID = pelicula.PeliculaID,
                Nombre = pelicula.Nombre,
                Resena = pelicula.Resena,
                CalificacionGenerQal = pelicula.CalificacionGenerQal,
                FechaLanzamiento = pelicula.FechaLanzamiento,
                RutaPoster = rutaImagenCompleta, // Usar la URL completa
                Comentarios = comentarios
            };

            return Ok(peliculaDetalle);
        }

    }
}

