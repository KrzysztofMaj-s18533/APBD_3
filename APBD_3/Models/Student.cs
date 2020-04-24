using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_3.Models
{
    public class Student
    {
        public int idStudent { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime birthDate { get; set; }
        public string semester { get; set; }
        public string nameOfStudies { get; set; }
        public string idEnrollment { get; set; }
        public string password { get; set; }
        public string salt { get; set; }
    }
}
