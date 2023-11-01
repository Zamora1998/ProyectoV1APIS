using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoV1.Models
{
    public class Resgistrarpelicula
    {
        public int PeliculaID { get; set; }
        public string Nombre { get; set; }
        public string Resena { get; set; }
        public Nullable<int> CalificacionGenerQal { get; set; }
        public Nullable<System.DateTime> FechaLanzamiento { get; set; }
        public Nullable<int> PosterID { get; set; }
    }
}