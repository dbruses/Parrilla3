//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Parrilla3.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Comidas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Comidas()
        {
            this.ListaVenta = new HashSet<ListaVenta>();
        }
    
        public int idComida { get; set; }
        public string nombre { get; set; }
        public Nullable<decimal> precio { get; set; }
        public string descripcion { get; set; }
        public int categoria { get; set; }
        public Nullable<bool> activo { get; set; }
        public string foto { get; set; }
        public Nullable<bool> llevaSalsa { get; set; }
        public Nullable<bool> llevaAderezo { get; set; }
        public Nullable<bool> llevaGuarnicion { get; set; }
        public Nullable<int> cantIngredientes { get; set; }
    
        public virtual Categorias Categorias { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ListaVenta> ListaVenta { get; set; }
    }
}
