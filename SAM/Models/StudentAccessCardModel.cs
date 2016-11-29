using System.Collections.Generic;

namespace SAM1.Models
{
    public class StudentAccessCardModel
    {
        public List<StudentModel> AvailableStudents { get; set; }
        public List<StudentAccessCard> LinkedStudentAccessCards { get; set; }
    }
}