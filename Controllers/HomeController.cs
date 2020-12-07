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
            Session["esPedido"] = 0;
            return View();
        }
    }
}