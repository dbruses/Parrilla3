using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parrilla3.Models
{
    public class VentaUsuario
    {
        public int idVenta { get; set; }
        public System.DateTime fechaVenta { get; set; }
        public double total { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
    }
}