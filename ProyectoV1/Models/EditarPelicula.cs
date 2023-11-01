using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoV1.Models
{
    public class EditarPelicula
    {
        public int PeliculaID { get; set; }
        public string CodPelicula { get; set; }
        public string Nombre { get; set; }
        public string Resena { get; set; }
        public int CalificacionGenerQal { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public int PosterID { get; set; }
    }
}