using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Parrilla3.Models
{
    public class Email
    {
        //Envia mail
        
        public Email()
        {

        }
        
        const string fromPassword = "Roman2018";
        const string subject = "Su pedido de Parrilla Los Salteños fue recibido";
        const string from = "pedidos@xn--parrillalossalteos-20b.com";

        public bool sendEmailPedido(string toAddress, string body)
        {
            MailAddress ma = new MailAddress(toAddress, "");
            MailAddress fromAddress = new MailAddress(from, "Parrilla Los Salteños");

            var smtp = new SmtpClient
            {
                Host = "mail.xn--parrillalossalteos-20b.com", //"dtc007.ferozo.com",
                Port = 587, //465,
                EnableSsl = false, // true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
            };
            try
            {
                using (var message = new MailMessage(fromAddress, ma)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    message.IsBodyHtml = true;
                    smtp.Send(message);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}