using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoV1.Models
{
    public class PeliculaDetalle
    {
        public int PeliculaID { get; set; }
        public string CodPelicula { get; set; }
        public string Nombre { get; set; }
        public string Resena { get; set; }
        public int? CalificacionGenerQal { get; set; }
        public DateTime? FechaLanzamiento { get; set; }
        public int? PosterID { get; set; }
        public string RutaPoster { get; internal set; }
        public string Actores { get; set; }
        public string NombrePelicula { get; internal set; }
        public string NombreActor { get; internal set; }
        public string Rol { get; internal set; }
    }

}