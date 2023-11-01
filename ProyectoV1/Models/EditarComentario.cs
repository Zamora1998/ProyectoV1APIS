using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoV1.Models
{
    public class EditarComentario
    {
        public int ComentarioID { get; set; }
        public int PeliculaID { get; set; }
        public int UsuarioID { get; set; }
        public string Comentario { get; set; }
        public DateTime FechaComentario { get; set; }
        public Nullable<int> RespuestaAComentarioID { get; set; }
    }
}