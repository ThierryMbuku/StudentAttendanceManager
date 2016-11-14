using System.Collections.Generic;
using System.Web.Mvc;

namespace SAM1.Areas.Student.Controllers
{
    public class StudentController : Controller
    {
        private readonly BusinessLayer.BusinessFacade businessFacade = new BusinessLayer.BusinessFacade();
        
        // GET: Student
        public ActionResult Index(int studentId)
        {
            var student = businessFacade.FindStudent(studentId);
            if (student == null)
            {
                //The ID could be hacked or maliciously sent via a tool like POSTMAN
                return new HttpNotFoundResult("The student does not exist");
            }

            return View(student);
        }

        public JsonResult GetStudentAttendanceProgress(int studentId)
        {
            //1) make call to Business facade to calculate the student's progress

            var list = new List<Result>();
            list.Add(new Result
            {
                SeriesTitle = "Days Attended",
                SeriesValue = 2
            });
            list.Add(new Result
            {
                SeriesTitle = "Days Not Attended",
                SeriesValue = 3
            });

            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }

    public class Result
    {
        public string SeriesTitle { get; set; }
        public int SeriesValue { get; set; }
    }
}