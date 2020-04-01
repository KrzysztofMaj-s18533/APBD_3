﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APBD_3.DTO;
using APBD_3.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : Controller
    {
        IStudentsDbService dbService;

        public EnrollmentsController(IStudentsDbService service)
        {
            this.dbService = service;
        }

        [HttpPost]
        public IActionResult RegisterStudents(RegisterRequest req)
        {
            //return Ok();
            var response = dbService.registerStudents(req);
            return response;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}