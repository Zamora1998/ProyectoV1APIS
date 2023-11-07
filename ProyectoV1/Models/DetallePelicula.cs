using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoV1.Models
{
    public class DetallePelicula
    {
        public int PeliculaID { get; set; }
        public string Nombre { get; set; }
        public string Resena { get; set; }
        public double CalificacionGenerQal { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string RutaPoster { get; set; }
        public List<ActorConRol> ActoresStaff { get; set; }
        public int? PosterID { get; internal set; }
    }

    public class ActorConRol
    {
        public string NombreActor { get; set; }
        public string Rol { get; set; }
    }

}