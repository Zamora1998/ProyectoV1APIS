using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoV1.Models
{
    public class AgregarCalificacionExperto
    {
        public int PeliculaID { get; set; }
        public int ExpertoID { get; set; }
        public decimal Calificacion { get; set; }
    }
}