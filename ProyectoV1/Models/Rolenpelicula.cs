using DataCliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoV1.Models
{
    public class Rolenpelicula
    {
        
        public int RolEnPeliculaID { get; set; }
        public Nullable<int> RolID { get; set; }
        public Nullable<int> ActorStaffID { get; set; }
        public Nullable<int> IDPelic { get; set; }
        public Nullable<int> OrdenAparicion { get; set; }

        public virtual ActoresStaff ActoresStaff { get; set; }
        public virtual Peliculas Peliculas { get; set; }
        public virtual Rol Rol { get; set; }
    }
}
