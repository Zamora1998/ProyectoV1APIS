
namespace DataCliente
{
    using System;
    using System.Collections.Generic;
    
    public partial class Peliculas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Peliculas()
        {
            this.CalificacionesExpertos = new HashSet<CalificacionesExpertos>();
            this.Comentarios = new HashSet<Comentarios>();
            this.RolesEnPelicula = new HashSet<RolesEnPelicula>();
        }
    
        public int PeliculaID { get; set; }
        public string Nombre { get; set; }
        public string Resena { get; set; }
        public Nullable<int> CalificacionGenerQal { get; set; }
        public Nullable<System.DateTime> FechaLanzamiento { get; set; }
        public Nullable<int> PosterID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CalificacionesExpertos> CalificacionesExpertos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comentarios> Comentarios { get; set; }
        public virtual Posters Posters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RolesEnPelicula> RolesEnPelicula { get; set; }
    }
}
