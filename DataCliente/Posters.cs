//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataCliente
{
    using System;
    using System.Collections.Generic;
    
    public partial class Posters
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Posters()
        {
            this.Peliculas = new HashSet<Peliculas>();
        }
    
        public int PosterID { get; set; }
        public string NombreArchivo { get; set; }
        public string RutaArchivo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Peliculas> Peliculas { get; set; }
    }
}
