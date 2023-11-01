using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoV1.Models
{
    public class RegistrarPelicula
    { 
        public string CodPelicula { get; set; }
        public string Nombre { get; set; }
        public string Resena { get; set; }
        public int CalificacionGenerQal { get; set; }
        public DateTime FechaLanzamiento { get; set; } 
        public virtual ICollection<InvolucradosPelis> Involucrados { get; set; }

        //Poster 
        public string NombreArchivo { get; set; }
        public string RutaArchivo { get; set; }
    }
}