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
    
    public partial class ListaVenta
    {
        public int idListaVenta { get; set; }
        public int ventaId { get; set; }
        public int comidaId { get; set; }
        public int cantidad { get; set; }
        public double total { get; set; }
        public string ingredientes { get; set; }
    
        public virtual Comidas Comidas { get; set; }
        public virtual Ventas Ventas { get; set; }
    }
}
