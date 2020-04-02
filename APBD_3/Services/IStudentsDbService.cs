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
    }
}
