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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    using(ApplicationDbContext db = new ApplicationDbContext())
            //    {
            //        var idUsuarioActual = User.Identity.GetUserId();

            //        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            //        var usuario = userManager.FindById(idUsuarioActual);
            //        var direccion = usuario.direccion;
            //        var telefono = usuario.telefono;
            //    }
            //}
            return View();
        }
    }
}