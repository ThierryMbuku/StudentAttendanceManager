using SAM1.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;


namespace SAM1.Areas.Student.Controllers
{
    public class StudentController : Controller
    {
        private readonly BusinessLayer.BusinessFacade businessFacade = new BusinessLayer.BusinessFacade();

        // GET: Student
        public ActionResult Index()
        {
            var userId = TempData.ContainsKey("UserId") ? TempData["UserId"].ToString() : null;
            if (userId != null)
            {
                var student = businessFacade.FindStudent(Convert.ToInt32(userId));
                return View(student);
            }
                //The ID could be hacked or maliciously sent via a tool like POSTMAN
                return new HttpNotFoundResult("The student does not exist");
        }

        public JsonResult GetStudentAttendanceProgress(int studentId)
        {
            //1) make call to Business facade to calculate the student's progress
            var businessFacade = new BusinessLayer.BusinessFacade();
            var studentProgress = businessFacade.CalculateStudentProgess(studentId);

            return Json(studentProgress.GetProgressList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SignIn()
        {
            return View();
        }

        public ActionResult SignRegister()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authenticate(LogonUserModel user)
        {
            var businessFacade = new BusinessLayer.BusinessFacade();
            var response = businessFacade.AuthenticateUser(user);
            var errorMessage = response.GetErrorMessage();
            TempData.Add("ErrorMessage", errorMessage);
            TempData.Add("UserId", response.GetUserId());
            return RedirectToAction(response.GetRedirectUrl());
        }

        [HttpPost]
        public ActionResult SignOn(LogonUserModel user)
        {
            var businessFacade = new BusinessLayer.BusinessFacade();
            var response = businessFacade.AuthoriseAccessCard();
            //TempData.Add("UserId", response.GetUserId());
            //TempData.Add("IsAuthorised", response.IsAuthorised);

            //return RedirectToAction(response.GetRedirectUrl());
            return null;
        }
    }
}
