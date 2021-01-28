using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Parrilla3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parrilla3.Controllers
{
    public class NotificacionController : Controller
    {
        private ParrillaEntities pe = new ParrillaEntities();

        // GET: Notificacion
        public ActionResult Index(string oid)
        {
            string pedido = oid;
            return View();
        }

        public ActionResult PagoOk(string oid)
        {
            enviaEmail(oid);
            ViewData["oid"] = oid;
            return View();
        }

        public ActionResult PagoRechazado(string oid)
        {
            ViewData["oid"] = oid;
            return View();
        }

        private void enviaEmail(string oid)
        {
            string observaciones = string.Empty; 
            string pagaCon = string.Empty;

            string tabla = "<table>";
            int ventaId = Convert.ToInt32(oid);
            Ventas venta = pe.Ventas.Find(ventaId);
            observaciones = venta.observaciones;
            pagaCon = venta.pagaCon.ToString();
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = manager.FindById(venta.idUsuario);
            string ingredientes = string.Empty;

            //Mail Parrilla
            foreach (var item in venta.ListaVenta)
            {
                if ((item.ingredientes != null) && (item.ingredientes != ""))
                {
                    ingredientes = item.ingredientes;
                }
                
                tabla = tabla + "<tr><td><b>";
                tabla = tabla + item.cantidad.ToString() + "</b></td><td>&nbsp;";
                tabla = tabla + item.Comidas.nombre + " " + ingredientes + "</td><td>&nbsp;$&nbsp;" + String.Format("{0:C}", (item.cantidad * item.Comidas.precio).ToString()) + "</td></tr>";
                ingredientes = string.Empty;
            }

            tabla = tabla + "<tr><td colspan=3><b>Total:&nbsp;&nbsp;" + String.Format("{0:C}", venta.total) + "</b></td></tr></table>";
            string body = "<label>Pedido para " + user.nombre.ToUpper() + ", " + user.apellido.ToUpper() + " - TE: " + user.telefono + "<label><br /><br/>";
            body = body + "<label><b>Pedido Nº :&nbsp;&nbsp;" + venta.idVenta.ToString() + "&nbsp; &nbsp;Fecha:&nbsp;&nbsp;" + DateTime.Now.ToShortDateString() + "&nbsp;&nbsp;Hora:&nbsp;&nbsp;" + DateTime.Now.ToShortTimeString() + " </b><label><br /><br/>";
            body = body + tabla;
            if ((observaciones != null) && (observaciones != string.Empty))
            {
                body = body + "<br /><br/><label>Comentarios: " + observaciones + ".<label>";
            }
            if ((pagaCon != null) && (pagaCon != string.Empty))
            {
                body = body + "<br /><br/><label>Paga con: $ " + pagaCon + ".<label>";
            }
            body = body + "<br /><br/><label>Enviar pedido a " + user.direccion + ".<label>";

            //Envia mail parrilla
            Email email = new Email();
            email.sendEmailPedido("pedidos@xn--parrillalossalteos-20b.com", body);

            //Mail Usuario
            tabla = "<table>";
            foreach (var item in venta.ListaVenta)
            {
                if ((item.ingredientes != null) && (item.ingredientes != ""))
                {
                    ingredientes = item.ingredientes;
                }
                tabla = tabla + "<tr><td><b>";
                tabla = tabla + item.cantidad.ToString() + "</b></td><td>&nbsp;";
                tabla = tabla + item.Comidas.nombre + " " + ingredientes + "</td><td>&nbsp;$&nbsp;" + String.Format("{0:C}", (item.cantidad * item.Comidas.precio).ToString()) + "</td></tr>";
                ingredientes = string.Empty;
            }

            tabla = tabla + "<tr><td colspan=3><b>Total:&nbsp;&nbsp;" + String.Format("{0:C}", venta.total) + "</b></td></tr></table>";

            body = "<label>Hola " + user.nombre.Substring(0, 1).ToUpper() + user.nombre.Substring(1).ToLower() + ", hemos recibido tu pedido, pronto podrás disfrutarlo!!<label><br /><br/>";
            body = body + "<label><b>Pedido Nº :&nbsp;&nbsp;" + venta.idVenta.ToString() + "&nbsp; &nbsp;Fecha:&nbsp;&nbsp;" + DateTime.Now.ToShortDateString() + "&nbsp;&nbsp;Hora:&nbsp;&nbsp;" + DateTime.Now.ToShortTimeString() + " </b><label><br /><br/>";
            body = body + tabla;
            if ((observaciones != null) && (observaciones != string.Empty))
            {
                body = body + "<br /><br/><label>Comentarios: " + observaciones + ".<label>";
            }
            if ((pagaCon != null) && (pagaCon != string.Empty))
            {
                body = body + "<br /><br/><label>Paga con: $ " + pagaCon + ".<label>";
            }
            body = body + "<br /><br/><label>Enviaremos tu pedido a " + user.direccion + ".<label>";
            body = body + "<br /><br/><label><b>Por consultas o comentarios, mandanos un Whatsapp o comunicate por teléfono</b><label>";
            body = body + "<br /><br/><label>Muchas gracias por tu compra.<label><br /><br/><label>PARRILLA LOS SALTEÑOS<label>";
            body = body + "<br /><br/>";
            body = body + "<div class='text-content container'><div class='col-md-6'><span class='social_heading'>SEGUINOS EN</span>";
            body = body + "<p>Facebook: <b>Parrilla Resto Los Salteños (facebook.com/resto.lossaltenos)</b></p>";
            body = body + "<p>Email: <b>pedidos@parrillalossalteños.com</b></p>";
            body = body + "<p>Instagram: <b>instagram.com/resto.parrilla.los.saltenos/</b></p>";
            body = body + "<p>WhatsApp: <b><a href = 'https://wa.me/541158530973?text=Quiero%20hacer%20un%20pedido' target='_blank'>11 5853 - 0973</a></p>";
            body = body + "</div><div class='col-md-4'><span class='social_heading'>O LLAME AL </span><span class='social_info'>011 4957-0049</span>";
            body = body + "</div><div class='col-md-10'><span class='social_heading'>DELIVERY SIN CARGO</span></div></div>";

            //Envia mail usuario
            email = new Email();
            email.sendEmailPedido(user.Email, body);

            Session["venta"] = null;
            Session["carrito"] = null;
            //}

        }
    }
}