using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoV1.Models
{
    public class InvolucradosPelis
    {
        public string Nombre { get; set; }
        public string PaginaWeb { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public int RolID { get; set; }
        public int ActorStaffID { get; set; }
        public int IDPelic { get; set; }
        public int OrdenAparicion { get; set; }
    }
}