using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoV1.Models
{
    public class PeliculaDto
    {
        public int PeliculaID { get; set; }
        public string CodPelicula { get; set; }
        public string Nombre { get; set; }
        public string Resena { get; set; }
        public int? CalificacionGenerQal { get; set; }
        public DateTime? FechaLanzamiento { get; set; }
        public int? PosterID { get; set; }
        public List<InvolucradoDto> Involucrados { get; set; }
        public List<ComentarioDto> Comentarios { get; set; }
        public List<CalificacionDto> Calificaciones { get; set; }
    }
    public class InvolucradoDto
    {
        public int ActorStaffID { get; set; }
        public string Nombre { get; set; }
        public string PaginaWeb { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
    }

    public class ComentarioDto
    {
        public int ComentarioID { get; set; }
        public int PeliculaID { get; set; }
        public int UsuarioID { get; set; }
        public string Comentario { get; set; }
        public DateTime FechaComentario { get; set; }
        public int RespuestaAComentarioID { get; set; }
    }

    public class CalificacionDto
    {
        public int PeliculaID { get; set; }
        public int ExpertoID { get; set; }
        public decimal Calificacion { get; set; }
    }
}