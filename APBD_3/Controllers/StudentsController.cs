using System;
using System.Collections.Generic;
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
        public IActionResult GetStudents(string orderBy)
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpPost]
        public IActionResult CreateStudent (Student student)
        {
            student.indexNumber = $"s{new Random().Next(1, 20000)}";
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