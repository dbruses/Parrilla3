using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parrilla3.Models
{
    public class CarritoItem
    {
        private Comidas _comidas;
        private int _cantidad;
        private string _ingredientes;

        public CarritoItem()
        {

        }

        public CarritoItem(Comidas comida, int cantidad, string ingredientes)
        {
            this.Comidas = comida;
            this.Cantidad = cantidad;
            this.Ingredientes = ingredientes;
        }

        public Comidas Comidas { get => _comidas; set => _comidas = value; }
        public int Cantidad { get => _cantidad; set => _cantidad = value; }
        public string Ingredientes { get => _ingredientes; set => _ingredientes = value; }
    }
}