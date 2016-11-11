using SAM1.Models;
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

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogonUserModel user)
        {
            var response = businessFacade.LogOn(user);
            TempData["IsAuthorised"] = response.IsAuthorised;
            return Redirect(response.GetRedirectUrl());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}