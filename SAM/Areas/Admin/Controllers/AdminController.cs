using SAM1.Models;
using System.IO.Ports;
using System.Linq;
using System.Web.Mvc;

namespace SAM1.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        private readonly BusinessLayer.BusinessFacade businessFacade = new BusinessLayer.BusinessFacade();

        #region Authorise and Authentication

        public ActionResult Landing()
        {
            return View();
        }

        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorise()
        {
            var response = businessFacade.AuthoriseAccessCard();
            TempData["IsAuthorised"] = response.IsAuthorised;
            TempData["AuthorisationMessage"] = response.GetErrorMessage();
            TempData["UserId"] = response.GetUserId();

            return Redirect(response.GetRedirectUrl());
        }

        [HttpPost]
        public ActionResult Authenticate(LogonUserModel user)
        {
            var response = businessFacade.AuthenticateUser(user);
            var errorMessage = response.GetErrorMessage();
            TempData.Add("ErrorMessage", errorMessage);
            Session.Add("AdminUserId", response.GetUserId());

            return Redirect(response.GetRedirectUrl());
        }

        [HttpPost]
        public ActionResult SignOn(LogonUserModel user)
        {
            var response = businessFacade.AuthoriseAccessCard();
            TempData.Add("UserId", response.GetUserId());
            TempData.Add("IsAuthorised", response.IsAuthorised);

            return RedirectToAction(response.GetRedirectUrl());
        }

        //Linking students to access cards
        public ActionResult StudentCards()
        {
            var response = businessFacade.GetStudentAccessCards();
            return View(response);
        }

        [HttpPost]
        public ActionResult UpdateStudentAccessCardLink(int userId, string operationType, int? cardId)
        {
            var response = businessFacade.PerformStudentAccessCardLink(userId, operationType, cardId);
            TempData["ErrorMessage"] = response.GetErrorMessage();
            TempData["UserId"] = response.GetUserId();
            TempData["WasOperationSuccessful"] = response.WasOperationSuccessful;
            return RedirectToAction("StudentCards");
        }

        #endregion

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