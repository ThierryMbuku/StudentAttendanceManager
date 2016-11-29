using SAM1.Models;
using System;
using System.Web.Mvc;

namespace SAM1.Controllers
{
    public class HomeController : Controller
    {
        private readonly BusinessLayer.BusinessFacade businessFacade = new BusinessLayer.BusinessFacade();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
      
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            HttpContext.Response.Cache.SetExpires(DateTime.Now.AddMinutes(-1));
            Response.Cache.SetValidUntilExpires(true);
            return RedirectToAction("Login");
        }
    }
}