using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoV1.Models
{
    public class AgregarComentario
    {
        public int PeliculaID { get; set; }
        public int UsuarioID { get; set; }
        public string Comentario { get; set; }
        public DateTime FechaComentario { get; set; }
        public int RespuestaAComentarioID { get; set; }
    }
}