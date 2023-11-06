using System;

namespace ProyectoV1.Controllers
{
    internal class ComentarioDto
    {
        public int ComentarioID { get; set; }
        public int? PeliculaID { get; set; }
        public int? UsuarioID { get; set; }
        public string Comentario { get; set; }
        public DateTime? FechaComentario { get; set; }
    }
}