using APBD_3.DTO;
using APBD_3.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_3.Services
{
    public interface IStudentsDbService
    {
        [HttpGet]
        public IEnumerable<Student> GetStudents();
        public IActionResult registerStudents(RegisterRequest req);
        public IActionResult promoteStudents(PromotionRequest req);
        public bool findStud(string index);
        public void AddRefreshToken(Guid refToken, string indexNum);
        public string CheckRefreshToken(string refToken);
        public string CreateSalt();
        public string CreateHash(string password, string salt);
        public IActionResult TryHash(Student student, string salt, string hash);
    }
}
