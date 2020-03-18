using APBD_3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_3.DAL
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;

        static MockDbService()
        {
            _students = new List<Student>
            {
                new Student{idStudent=1, firstName="Maciej", lastName="Kowalski" },
                new Student{idStudent=2, firstName="Anna", lastName="Rzeczna" },
                new Student{idStudent=1, firstName="Piotr", lastName="Siódmiak" }
            };
        }
        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }
    }
}
