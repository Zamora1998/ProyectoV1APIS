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
                .Take(5) 
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

            var actoresYRoles = Pelis.RolesEnPelicula
               .Select(r => new PeliculaDetalle
               {
                   NombrePelicula = Pelis.Peliculas
                       .FirstOrDefault(p => p.PeliculaID == r.IDPelic)
                       .Nombre,
                   NombreActor = Pelis.ActoresStaff
                       .FirstOrDefault(a => a.ActorStaffID == r.ActorStaffID)
                       .Nombre,
                   Rol = Pelis.Rol
                       .FirstOrDefault(rol => rol.RolID == r.RolID)
                       .Nombre
               })
               .ToList();

            if (peliculas == null || peliculas.Count == 0)
            {
                return NotFound();
            }

            var peliculasConRutaPoster = new List<GetPeliculas>();
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
                        RutaPoster = baseUrl + "/APIV5" + poster.RutaArchivo // Construir la URL completa
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

        [HttpGet]
        [Route("api/PeliculasF/ObtenerDetalles")]
        public IHttpActionResult ObtenerDetalles()
        {
            var peliculas = Pelis.Peliculas
                .OrderByDescending(p => p.FechaLanzamiento)
                .Take(5)
                .Select(p => new DetallePelicula
                {
                    PeliculaID = p.PeliculaID,
                    Nombre = p.Nombre,
                    Resena = p.Resena,
                    CalificacionGenerQal = (double)p.CalificacionGenerQal,
                    FechaLanzamiento = (DateTime)p.FechaLanzamiento,
                    PosterID = p.PosterID
                })
                .ToList();

            var detallesPeliculas = new List<DetallePelicula>();
            var request = HttpContext.Current.Request;
            var baseUrl = request.Url.GetLeftPart(UriPartial.Authority);

            foreach (var pelicula in peliculas)
            {
                var actoresConRoles = Pelis.RolesEnPelicula
                    .Where(r => r.IDPelic == pelicula.PeliculaID)
                    .Take(3)
                    .Select(r => new ActorConRol
                    {
                        NombreActor = Pelis.ActoresStaff
                            .FirstOrDefault(a => a.ActorStaffID == r.ActorStaffID)
                            .Nombre,
                        Rol = Pelis.Rol
                            .FirstOrDefault(rol => rol.RolID == r.RolID)
                            .Nombre
                    })
                    .ToList();

                var poster = Pelis.Posters.FirstOrDefault(p => p.PosterID == pelicula.PosterID);
                var rutaPoster = poster != null ? baseUrl + "/APIV5" + poster.RutaArchivo : null;

                detallesPeliculas.Add(new DetallePelicula
                {
                    PeliculaID = pelicula.PeliculaID,
                    Nombre = pelicula.Nombre,
                    Resena = pelicula.Resena,
                    CalificacionGenerQal = pelicula.CalificacionGenerQal,
                    FechaLanzamiento = pelicula.FechaLanzamiento,
                    RutaPoster = rutaPoster,
                    ActoresStaff = actoresConRoles
                });
            }

            return Ok(detallesPeliculas);
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
                    Comentario = c.Comentario,
                    FechaComentario = c.FechaComentario,
                }).ToList();

            // Obtener actores y sus roles en la película con descripción de rol
            // Obtener actores y sus roles en la película con descripción de rol
            var actoresConRoles = Pelis.RolesEnPelicula
                .Where(r => r.IDPelic == pelicula.PeliculaID)
                .Select(r => new
                {
                    NombreActor = Pelis.ActoresStaff
                        .FirstOrDefault(a => a.ActorStaffID == r.ActorStaffID)
                        .Nombre,
                    Rol = Pelis.Rol
                        .FirstOrDefault(rol => rol.RolID == r.RolID)
                        .Nombre,
                }).ToList();
            var request = HttpContext.Current.Request;
            var baseUrl = request.Url.GetLeftPart(UriPartial.Authority);
            var rutaImagenCompleta = baseUrl + "/APIV5" + rutaPoster;

            var peliculaDetalle = new
            {
                Nombre = pelicula.Nombre,
                Resena = pelicula.Resena,
                CalificacionGenerQal = pelicula.CalificacionGenerQal,
                FechaLanzamiento = pelicula.FechaLanzamiento,
                RutaPoster = rutaImagenCompleta, // Usar la URL completa
                Comentarios = comentarios,
                ActoresStaff = actoresConRoles
            };

            return Ok(peliculaDetalle);
        }

    }

}

