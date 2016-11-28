using SAM1.Models;
using System.IO.Ports;
using System.Linq;
using System.Web.Mvc;

namespace SAM1.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        private readonly BusinessLayer.BusinessFacade businessFacade = new BusinessLayer.BusinessFacade();

        // GET: Account
        public ActionResult Index()
        {
            using (var db = new SAMEntities())
            {
                return View(db.Users.ToList());
            }
        }
        public ActionResult Register()
        {
            var availableCards = businessFacade.GetAvailableCards();
            ViewBag.AvailableCards = availableCards;
            return View();
        }
        [HttpPost]
        public ActionResult Register(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                var message = businessFacade.RegisterStudent(userModel);
                ModelState.Clear();
                ViewBag.Message = message;
            }
            return View();
        }

        public ActionResult LoggedIn()
        {
            if (Session["Username"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult StudentViewList()
        {
            var businessFacade = new BusinessLayer.BusinessFacade();
            var users = businessFacade.StudentList();

            return View(users);

        }
        public ActionResult Attendance()
        {
            var businessFacade = new BusinessLayer.BusinessFacade();
            var users = businessFacade.StudentList();
            return View(users);
        }
        //public ActionResult SignRegister()
        //{
        //    var businessFacade = new BusinessLayer.BusinessFacade();
        //    var users = businessFacade.StudentList();

        //    return View(users);

        //}

    }
}