using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAM1.Areas.Student.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult Index()
        {
            var businessFacade = new BusinessLayer.BusinessFacade();
            var users = businessFacade.StudentList();

            //var students = from s in users select s;
            //if (!String.IsNullOrEmpty(search))
            //{
            //    users = users.Where(s => s.FirstName.Contains(search));
            //}
            return View(users);

        }
    }
}