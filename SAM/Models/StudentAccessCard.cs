
using System;

namespace SAM1.Models
{
    public class StudentAccessCard
    {
        public StudentModel Student { get; set; }
        public string CardId { get; set; }
        public DateTime LinkDate { get; set; }
        public int Id { get; internal set; }
    }
}