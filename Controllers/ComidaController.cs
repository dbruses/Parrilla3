﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Parrilla3.Models;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Parrilla3.Controllers
{
    public class ComidaController : Controller
    {
        // GET: Comida
        private ParrillaEntities pe = new ParrillaEntities();
        public string resp = string.Empty;
        public ActionResult Index()
        {
            if (Session["ingredientes"] == null)
            {
                Session["ingredientes"] = pe.Ingredientes.ToList();
            }
            if (Session["salsas"] == null)
            {
                Session["salsas"] = pe.Salsas.ToList();
            }

            Session["esPedido"] = 1;

            return View(pe.Comidas.ToList());
        }

        public ActionResult ABM()
        {
            if (Session["categorias"] == null)
            {
                Session["categorias"] = pe.Categorias.ToList();
            }

            Session["esPedido"] = 0;
            
            return View(pe.Comidas.ToList());
        }

        public ActionResult VerComida(int id)
        {
            Session["esPedido"] = 0;

            return View(pe.Comidas.Where(x => x.idComida == id));
        }

        public ActionResult VerEdit(int id)
        {
            Session["esPedido"] = 0;

            return View(pe.Comidas.Where(x => x.idComida == id));
        }

        [HttpPost]
        public JsonResult CargaCarrito(int id, int cantidad, string ingredientes)
        {
            Session["esPedido"] = 0;

            if (Session["carrito"] == null)
            {
                //int cant = Convert.ToInt16(Request["cantidad"]);
                List<CarritoItem> compras = new List<CarritoItem>();
                compras.Add(new CarritoItem(pe.Comidas.Find(id), cantidad, ingredientes));
                Session["carrito"] = compras;
            }
            else
            {
                List<CarritoItem> compras = (List<CarritoItem>)Session["carrito"];
                int indexExistente = getIndex(id);
                if (indexExistente == -1)
                {
                    compras.Add(new CarritoItem(pe.Comidas.Find(id), cantidad, ingredientes));
                }
                else
                {
                    compras[indexExistente].Cantidad+=cantidad;
                }
                Session["carrito"] = compras;
            }
            return Json("Comida agregada.", JsonRequestBehavior.AllowGet);
        }

        public ActionResult EliminaCarrito(int id)
        {
            Session["esPedido"] = 0;

            List<CarritoItem> compras = (List<CarritoItem>)Session["carrito"];
            compras.RemoveAt(getIndex(id));
            Session["carrito"] = compras;
            return View("Index", pe.Comidas.ToList());
        }

        [Authorize]
        public ActionResult FinalizarCompra()
        {
            Session["esPedido"] = 0;

            Ventas nuevaVenta = new Ventas();
            List<CarritoItem> compras = (List<CarritoItem>)Session["carrito"];
            if ((compras != null) && (compras.Count > 0))
            {
                int vtaId = 0;
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var user = manager.FindById(User.Identity.GetUserId());

                nuevaVenta.fechaVenta = DateTime.Now;
                nuevaVenta.total = ((double)compras.Sum(x => x.Comidas.precio * x.Cantidad));
                nuevaVenta.idUsuario = user.Id;

                nuevaVenta.ListaVenta = (from item in compras
                                         select new ListaVenta
                                         {
                                             cantidad = item.Cantidad,
                                             comidaId = item.Comidas.idComida,
                                             total = ((double)(item.Comidas.precio * item.Cantidad)),
                                             ingredientes = item.Ingredientes
                                         }).ToList();
                pe.Ventas.Add(nuevaVenta);
                pe.SaveChanges();
                vtaId = nuevaVenta.idVenta;

                Usuario usr = new Usuario();
                usr.direccion = user.direccion;
                usr.telefono = user.telefono;

                Session["usuario"] = usr;
                Session["venta"] = nuevaVenta.idVenta;

                return View(nuevaVenta);
            }

            return View("Index", pe.Comidas.ToList());
        }

        public ActionResult MostrarCarrito()
        {
            Session["esPedido"] = 0;

            return View("CargaCarrito");
        }

        [HttpPost]
        public JsonResult GrabaObsPaga(int idVenta, string observaciones, decimal pagaCon)
        {
            try
            {
                pe.Ventas.Find(idVenta).observaciones = observaciones;
                pe.Ventas.Find(idVenta).pagaCon = pagaCon;
                pe.SaveChanges();

                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ObtenerDatosComercio()
        {
            string result = string.Empty;

            result = "{\"checkoutoption\":\"" + ConfigurationManager.AppSettings["checkoutoption"] + "\",\"sharedsecret\":\"" + ConfigurationManager.AppSettings["FDPassword"] + "\",\"timezone\":\"" + ConfigurationManager.AppSettings["timezone"] + "\",\"storename\":\"" + ConfigurationManager.AppSettings["StoreId"] + "\",\"currency\":\"" + ConfigurationManager.AppSettings["Moneda"] + "\"}";

            //var res = JsonConvert.SerializeObject(result, Formatting.Indented);
            //return Json(res);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Aumento(decimal porcentaje)
        {
            foreach (Comidas comida in pe.Comidas.ToList())
            {
                pe.Comidas.Find(comida.idComida).precio = comida.precio + (comida.precio * porcentaje / 100);
                pe.SaveChanges();
            }
            
            return Json("Los cambios fueron aplicados correctamente.", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GrabaProd(int id, string nombre, string descripcion, int categoria, string precio, bool activo, bool llevaSalsa, bool llevaAderezo, bool llevaGuarnicion, int cantIngredientes)
        {
            decimal precioUnit = Convert.ToDecimal(precio.Replace(".",","));
            pe.Comidas.Find(id).nombre = nombre;
            pe.Comidas.Find(id).descripcion = descripcion;
            pe.Comidas.Find(id).categoria = categoria;
            pe.Comidas.Find(id).precio = precioUnit;
            pe.Comidas.Find(id).activo = activo;
            pe.Comidas.Find(id).llevaSalsa = llevaSalsa;
            pe.Comidas.Find(id).llevaAderezo = llevaAderezo;
            pe.Comidas.Find(id).llevaGuarnicion = llevaGuarnicion;
            pe.Comidas.Find(id).cantIngredientes = cantIngredientes;

            pe.SaveChanges();

            return Json("Los cambios fueron aplicados correctamente.", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Pagar(bool pagoEfectivo, string pagaCon, string observaciones)
        {
            Session["esPedido"] = 0;

            bool pagoOk = false;

            string mensaje = string.Empty;
            Session["observaciones"] = observaciones;
            //Cobrar
            if (!pagoEfectivo)
            {
                pagoOk = pagoTarjeta();
            }
            else
            {
                pagoOk = true;
            }

            if (pagoOk && pagoEfectivo)
            {
                mensaje = "Muchas gracias por su compra.";
                enviaEmail(pagoEfectivo, pagaCon, observaciones);
            }
            else
            {
                //mensaje = "Hemos encontrado problemas con su pago, por favor elija otro medio.";
                mensaje = resp;
            }
            return Json(mensaje, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool pagoTarjeta()
        {
            bool ok = false;

            int ventaId = (int)Session["venta"];
            Ventas venta = pe.Ventas.Find(ventaId);

            string txntype = "sale";
            string timezone = "America/Buenos_Aires";
            string hash_algorithm = "SHA256";
            string currency = ConfigurationManager.AppSettings["Moneda"];
            string storename = ConfigurationManager.AppSettings["StoreId"];
            string txndatetime = DateTime.Now.ToString("yyyy:MM:dd-HH:mm:ss");
            string chargetotal = venta.total.ToString("F2");
            chargetotal = chargetotal.Replace(".", string.Empty);
            chargetotal = chargetotal.Replace(",", ".");
            string sharedsecret = ConfigurationManager.AppSettings["FDPassword"];
            string stringToHash = storename + txndatetime + chargetotal + currency + sharedsecret; 
            string checkoutoption = ConfigurationManager.AppSettings["checkoutoption"];
            string orderid = ventaId.ToString();

            string hexaString = String.Concat(stringToHash.Select(x => ((int)x).ToString("x")));

            string sha256String = ComputeSHA256Hash(hexaString).ToLower();

            var segment = string.Join("&", "txntype="+txntype, "timezone="+timezone, "txndatetime="+txndatetime, "hash_algorithm="+hash_algorithm, "hash="+sha256String, "storename="+storename, "chargetotal="+chargetotal, "currency="+currency, "checkoutoption="+ checkoutoption, "oid="+ orderid,"country=ARG", "language=es_ES");
            var escapedSegment = Uri.EscapeDataString(segment);
            var baseFormat = ConfigurationManager.AppSettings["PayURL"];

            var url = string.Concat(baseFormat, "?" + segment);
            resp = "URL COMPLETA : " + url + "<br /><br /> RESPUESTA <br /><br />";

            HttpWebRequest webReq; // = (HttpWebRequest)WebRequest.Create(string.Format(url));
            webReq = WebRequest.Create(url) as HttpWebRequest;
            webReq.Method = "POST";
            webReq.Accept = "application/json";
            webReq.ContentType = "application/json; charset=utf-8";

            Stream postStream = webReq.GetRequestStream();

            //HttpWebResponse webResponse = (HttpWebResponse)webReq.GetResponse();
            HttpWebResponse webResponse = webReq.GetResponse() as HttpWebResponse;

            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                ok = true;
            }
            StreamReader sr = new StreamReader(webResponse.GetResponseStream());
            //resp = resp + sr.ReadToEnd().Trim();
            
            resp = sr.ReadToEnd().Trim();
            
            //resp = url;

            //Response.Redirect(url,true);
            //string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
            //resp = resp.Replace("/connect/", BaseURL + "/connect/");
            //resp = resp.Replace("resources/js/", BaseURL + "/resources/js/");
            return ok;
        }

        public static string ComputeSHA256Hash(string text)
        {
            using (var sha256 = new SHA256Managed())
            {
                return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "");
            }
        }

        [HttpPost]
        public JsonResult Cancelar()
        {
            Session["esPedido"] = 0;

            Session["venta"] = null;
            Session["carrito"] = null;
            return Json("Compra cancelada.", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CancelaGraba()
        {
            Session["esPedido"] = 0;
            return Json("No se guardaron los datos.", JsonRequestBehavior.AllowGet);
        }

        private void enviaEmail(bool pagoEfectivo, string pagaCon, string observaciones)
        {
            if (Session["venta"] != null)
            {
                string tabla = "<table>";
                int ventaId = (int)Session["venta"];
                Ventas venta = pe.Ventas.Find(ventaId);
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var user = manager.FindById(User.Identity.GetUserId());
                string ingredientes = "";
                List<CarritoItem> listaCarrito = (List<CarritoItem>)Session["carrito"];

                //Mail Parrilla
                foreach (var item in venta.ListaVenta)
                {
                    foreach (var sComida in listaCarrito)
                    {
                        if (item.Comidas.idComida == sComida.Comidas.idComida)
                        {
                            if ((sComida.Ingredientes != null) && (sComida.Ingredientes != ""))
                            {
                                ingredientes = " - " + sComida.Ingredientes;
                            }
                        }
                    }
                    
                    tabla = tabla + "<tr><td><b>";
                    tabla = tabla + item.cantidad.ToString() + "</b></td><td>&nbsp;";
                    tabla = tabla + item.Comidas.nombre + ingredientes + "</td><td>&nbsp;$&nbsp;" + String.Format("{0:C}", (item.cantidad * item.Comidas.precio).ToString()) + "</td></tr>";
                    ingredientes = string.Empty;
                }
                if (pagoEfectivo)
                {
                    double descuento = venta.total * 0.1;
                    double totCdesc = venta.total - descuento;
                    tabla = tabla + "<tr><td colspan=3>&nbsp;</td></tr>";
                    tabla = tabla + "<tr><td colspan=3><b>Total de la compra:&nbsp;&nbsp;" + String.Format("{0:C}", venta.total) + "</b></td></tr>";
                    tabla = tabla + "<tr><td colspan=3><b>Descuento por pago efectivo:&nbsp;&nbsp;" + String.Format("{0:C}", descuento) + "</b></td></tr>";
                    tabla = tabla + "<tr><td colspan=3><b>Total a pagar:&nbsp;&nbsp;" + String.Format("{0:C}", totCdesc) + "</b></td></tr></table>";
                }
                else
                {
                    tabla = tabla + "<tr><td colspan=3><b>Total:&nbsp;&nbsp;" + String.Format("{0:C}", venta.total) + "</b></td></tr></table>";
                }
                string body = "<label>Pedido para " + user.nombre.ToUpper()  + ", " + user.apellido.ToUpper() + " - TE: " + user.telefono + "<label><br /><br/>";
                body = body + "<label><b>Pedido Nº :&nbsp;&nbsp;" + venta.idVenta.ToString() + "&nbsp; &nbsp;Fecha:&nbsp;&nbsp;" + DateTime.Now.ToShortDateString() + "&nbsp;&nbsp;Hora:&nbsp;&nbsp;" + DateTime.Now.ToShortTimeString() + " </b><label><br /><br/>";
                body = body + tabla;
                if ((observaciones != null) && (observaciones != ""))
                {
                    body = body + "<br /><br/><label>Comentarios: " + observaciones + ".<label>";
                }
                if ((pagaCon != null) && (pagaCon != ""))
                {
                    body = body + "<br /><br/><label>Paga con $: " + pagaCon + ".<label>";
                }
                
                body = body + "<br /><br/><label>Enviar pedido a " + user.direccion + ".<label>";

                //Envia mail parrilla
                Email email = new Email();
                email.sendEmailPedido("pedidos@xn--parrillalossalteos-20b.com", body);

                //Mail Usuario
                tabla = "<table>";
                foreach (var item in venta.ListaVenta)
                {
                    foreach (var sComida in listaCarrito)
                    {
                        if (item.Comidas.idComida == sComida.Comidas.idComida)
                        {
                            if ((sComida.Ingredientes != null) && (sComida.Ingredientes != ""))
                            {
                                ingredientes = " - " + sComida.Ingredientes;
                            }
                        }
                    }
                    tabla = tabla + "<tr><td><b>";
                    tabla = tabla + item.cantidad.ToString() + "</b></td><td>&nbsp;";
                    tabla = tabla + item.Comidas.nombre + ingredientes + "</td><td>&nbsp;$&nbsp;" + String.Format("{0:C}", (item.cantidad * item.Comidas.precio).ToString()) + "</td></tr>";
                    ingredientes = string.Empty;
                }
                if (pagoEfectivo)
                {
                    double descuento = venta.total * 0.1;
                    double totCdesc = venta.total - descuento;
                    tabla = tabla + "<tr><td colspan=3>&nbsp;</td></tr>";
                    tabla = tabla + "<tr><td colspan=3><b>Total de la compra:&nbsp;&nbsp;" + String.Format("{0:C}", venta.total) + "</b></td></tr>";
                    tabla = tabla + "<tr><td colspan=3><b>Descuento por pago efectivo:&nbsp;&nbsp;" + String.Format("{0:C}", descuento) + "</b></td></tr>";
                    tabla = tabla + "<tr><td colspan=3><b>Total a pagar:&nbsp;&nbsp;" + String.Format("{0:C}", totCdesc) + "</b></td></tr></table>";
                }
                else
                {
                    tabla = tabla + "<tr><td colspan=3><b>Total:&nbsp;&nbsp;" + String.Format("{0:C}", venta.total) + "</b></td></tr></table>";
                }

                body = "<label>Hola " + user.nombre.Substring(0, 1).ToUpper() + user.nombre.Substring(1).ToLower() + ", hemos recibido tu pedido, pronto podrás disfrutarlo!!<label><br /><br/>";
                body = body + "<label><b>Pedido Nº :&nbsp;&nbsp;" + venta.idVenta.ToString() + "&nbsp; &nbsp;Fecha:&nbsp;&nbsp;" + DateTime.Now.ToShortDateString() + "&nbsp;&nbsp;Hora:&nbsp;&nbsp;" + DateTime.Now.ToShortTimeString() + " </b><label><br /><br/>";
                body = body + tabla;
                if ((observaciones != null) && (observaciones != ""))
                {
                    body = body + "<br /><br/><label>Comentarios: " + observaciones + ".<label>";
                }
                if ((pagaCon != null) && (pagaCon != ""))
                {
                    body = body + "<br /><br/><label>Paga con $: " + pagaCon + ".<label>";
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
                email.sendEmailPedido(User.Identity.Name, body);

                Session["venta"] = null;
                Session["carrito"] = null;
            }
            
        }

        private int getIndex(int id)
        {
            List<CarritoItem> compras = (List<CarritoItem>)Session["carrito"];
            for (int i = 0; i < compras.Count; i++)
            {
                if (compras[i].Comidas.idComida == id)
                {
                    return i;
                }
            }

            return -1;
        }

        private bool UploadFileValido(string extension)
        {
            bool valido = false;
            string[] formatosValidos = { "PNG", "JPG", "JPEG", "BMP", "GIF" };
            int i = 0;

            if (!string.IsNullOrEmpty(extension))
            {
                while ((!valido) && (i < formatosValidos.Length))
                {
                    if (extension.ToUpper() == formatosValidos[i])
                    {
                        valido = true;
                    }
                    i++;
                }
            }

            return valido;
        }

        [HttpPost]
        public JsonResult UploadFile()
        {
            string msjRetorno = string.Empty;
            string bits = string.Empty;
            string nombre = string.Empty;
            string ext = string.Empty;
            string nomArch = string.Empty;
            byte[] arreglo = null;
            byte[] result = null;
            string post = string.Empty;
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        bits = stream.Length.ToString();

                        var fileName = fileContent.FileName;
                        nombre = file; // fileName.ToString();

                        string[] fn = fileName.Split('.');
                        nomArch = fn[0].ToString();
                        ext = fn[1].ToString();

                        // valido que sea un formato de archivo válido
                        if (UploadFileValido(ext))
                        {
                            var streamMem = new MemoryStream();
                            arreglo = new byte[fileContent.ContentLength];

                            int bytesRead;
                            while ((bytesRead = stream.Read(arreglo, 0, arreglo.Length)) > 0)
                            {
                                streamMem.Write(arreglo, 0, bytesRead);
                            }
                            result = streamMem.ToArray();
                            //string archBytes = Convert.ToBase64String(result);

                            //redimensiono la imagen
                            Image imagenOriginal;
                            byte[] nuevaImg;
                            using (var ms = new MemoryStream(result))
                            {
                                imagenOriginal = Image.FromStream(ms);
                                imagenOriginal = ResizeImage(imagenOriginal, 500, 300);
                            }
                            using (var ms = new MemoryStream())
                            {
                                imagenOriginal.Save(ms, ImageFormat.Jpeg);
                                nuevaImg = ms.ToArray();
                            }

                            string archBytes = Convert.ToBase64String(nuevaImg);
                            int cId = Convert.ToInt32(Request.Form[0]);

                            //Actualizo la comida
                            pe.Comidas.Find(cId).imagen = archBytes;
                            pe.SaveChanges();

                            msjRetorno = "Archivo subido correctamente";
                        }
                        else
                        {
                            msjRetorno = "El formato de archivo no es válido, debe ser del tipo imagen (PNG, JPEG, JPG, BMP o GIF)";
                        }
                        // FIN SISDEV-5286
                    }
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("1^Fallo en la subida del archivo " + e.Message);
            }

            return Json(msjRetorno);
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.Default;
                graphics.InterpolationMode = InterpolationMode.Default;
                graphics.SmoothingMode = SmoothingMode.Default;
                graphics.PixelOffsetMode = PixelOffsetMode.Default;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

    }
}