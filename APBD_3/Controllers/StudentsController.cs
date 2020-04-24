using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using APBD_3.DAL;
using APBD_3.DTO;
using APBD_3.Models;
using APBD_3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace APBD_3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsDbService _dbService;
        public IConfiguration Configuration { get; set; }

        public StudentsController(IStudentsDbService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            Configuration = configuration;
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
        [Authorize]
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
        [Authorize]
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

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {


            if (!SqlDbService.validReq(request)) {
                return Unauthorized();
            }

            var claims = new[]
               {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "login"),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "student"),
                new Claim(ClaimTypes.Role, "employee")

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                    issuer: "s18533",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );

            var refToken = Guid.NewGuid();
            _dbService.AddRefreshToken(refToken, request.indexNum);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = refToken
            });
        }

        [HttpPost("refresh-token/{token}")]
        public IActionResult RefreshToken(string tokenString)
        {
            string indexNum = _dbService.CheckRefreshToken(tokenString);

            if (indexNum == null)
            {
                return Unauthorized();
            }

            var claims = new[]
               {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "login"),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "student"),
                new Claim(ClaimTypes.Role, "employee")

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                    issuer: "s18533",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );

            var refToken = Guid.NewGuid();
            _dbService.AddRefreshToken(refToken, indexNum);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = refToken
            });
        }

        [HttpPost("register")]
        public IActionResult RegisterAccount(Student student)
        {

            var salt = _dbService.CreateSalt();

            var hash = _dbService.CreateHash(student.password, salt);


            if (!_dbService.TryHash(student, salt, hash).GetType().Equals("BadRequestObjectResult"))
            {
                return Ok();
            }

            return Unauthorized();
        }
    }
}