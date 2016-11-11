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
    }
}