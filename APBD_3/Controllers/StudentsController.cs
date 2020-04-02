using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using APBD_3.DAL;
using APBD_3.Models;
using Microsoft.AspNetCore.Mvc;

namespace APBD_3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        //[HttpGet("{id}")]
        //public IActionResult GetStudents([FromRoute] int id)
        //{
        //    if (id == 1) {
        //        return Ok("Nowak");
        //    }
        //    else if(id == 2)
        //    {
        //        return Ok("Kowalski");
        //    }
        //    return NotFound("Nie znaleziono studenta o id '" + id +"'");
        //}

        [HttpGet]
        public IActionResult GetStudents()
        {
            List<Student> students = new List<Student>();
            //return Ok(_dbService.GetStudents());
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18533;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "SELECT IndexNumber, FirstName, LastName, BirthDate, Semester, Name FROM Student " +
                    "INNER JOIN Enrollment ON Student.IdEnrollment = Enrollment.IdEnrollment " +
                    "INNER JOIN Studies ON Studies.IdStudy = Enrollment.IdStudy";

                client.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.firstName = dr["FirstName"].ToString();
                    st.lastName = dr["LastName"].ToString();
                    st.birthDate = (DateTime)dr["BirthDate"];
                    st.semester = dr["Semester"].ToString();
                    st.nameOfStudies = dr["Name"].ToString();
                    students.Add(st);
                }
            }
            return Ok(students);
        }

        [HttpGet("{id}")]
        public IActionResult GetStudents(string id)
        {
            List<Enrollment> enrollments = new List<Enrollment>();
            //return Ok(_dbService.GetStudents());
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18533;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "SELECT * FROM Enrollment INNER JOIN Student ON Enrollment.IdEnrollment = Student.IdEnrollment WHERE Student.IndexNumber = @id";
                com.Parameters.AddWithValue("id", id);

                client.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var enrollment = new Enrollment();
                    enrollment.idEnrollment = (int)dr["IdEnrollment"];
                    enrollment.semester = (int)dr["Semester"];
                    enrollment.idStudy = dr["IdStudy"].ToString();
                    enrollment.startDate = (DateTime)dr["StartDate"];
                    enrollments.Add(enrollment);
                }
            }
            return Ok(enrollments);
        }

        [HttpPost]
        public IActionResult CreateStudent (Student student)
        {
            student.idStudent = new Random().Next(1, 20000);
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent([FromRoute] int id)
        {
            return Ok("Aktualizacja dokończona");
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveStudent([FromRoute] int id)
        {
            return Ok("Usuwanie ukończone");
        }
    }
}