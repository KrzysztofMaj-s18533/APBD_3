using APBD_3.DTO;
using APBD_3.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace APBD_3.Services
{
    public class SqlDbService : Controller, IStudentsDbService
    {
        public IEnumerable<Student> GetStudents()
        {
            throw new NotImplementedException();
        }

        public IActionResult promoteStudents()
        {
            throw new NotImplementedException();
        }

        public IActionResult registerStudents(RegisterRequest req)
        {
            RegisterResponse rep = new RegisterResponse()
            {
                lastName = req.lastName,
                semester = 1,
                startDate = DateTime.Today
            }; 
            Student studentToRegister = new Student(){idStudent = req.idStudent, firstName = req.firstName, lastName = req.lastName, birthDate = req.birthDate, nameOfStudies = req.nameOfStudies};
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18533;Integrated Security=True"))
            {
                client.Open();
                using (var com = new SqlCommand())
                using (var transaction = client.BeginTransaction())
                {
                    try
                    {
                        com.Parameters.AddWithValue("name", req.nameOfStudies);
                        com.Connection = client;
                        com.CommandText = "SELECT * from Studies where Name=@name";
                        com.Transaction = transaction;
                        var dr = com.ExecuteReader();
                        if (!dr.Read()) { dr.Close(); transaction.Rollback(); return BadRequest("No studies found with given Name"); }
                        int idStudies = (int)dr["IdStudy"];
                        dr.Close();

                        com.CommandText = "Select IdEnrollment, Semester, Enrollment.IdStudy, StartDate, Studies.IdStudy From Enrollment JOIN Studies ON Studies.IdStudy = Enrollment.IdStudy WHERE Semester = 1 AND Studies.Name = @name";
                        dr = com.ExecuteReader();
                        bool readResult;
                        readResult = dr.Read();
                        int idEnrollment = (int)dr["IdEnrollment"];
                        if (!readResult)
                        {
                            dr.Close();
                            com.CommandText = "select MAX(IdEnrollment)+1 as id from Enrollment;";
                            dr = com.ExecuteReader();
                            if (!dr.Read()) { idEnrollment = 1; dr.Close(); }
                            else { idEnrollment = (int)dr["id"]; dr.Close(); }
                            dr.Close();
                            com.CommandText = "INSERT INTO Enrollment VALUES(@id, @semester, 1, GETDATE());";
                            com.Parameters.AddWithValue("id", idEnrollment);
                            com.Parameters.AddWithValue("idStudy", idStudies);
                            com.ExecuteNonQuery();
                        }
                        else
                        {
                            idEnrollment = (int)dr["idEnrollment"];
                            dr.Close();
                        }
                        dr.Close();

                        com.CommandText = "SELECT IndexNumber from Student where IndexNumber=@ind;";
                        com.Parameters.AddWithValue("ind", req.idStudent);
                        dr = com.ExecuteReader();
                        if (dr.Read()) { dr.Close(); transaction.Rollback(); return BadRequest("Student already exists"); }
                        dr.Close();
                        com.CommandText = "INSERT into Student values(@ind, @firstName, @lastName, @birthDate, @idEnrollment);";
                        com.Parameters.AddWithValue("firstName", req.firstName);
                        com.Parameters.AddWithValue("lastName", req.lastName);
                        com.Parameters.AddWithValue("birthDate", req.birthDate);
                        com.Parameters.AddWithValue("idEnrollment", idEnrollment);
                        com.ExecuteNonQuery();
                        transaction.Commit();
                        dr.Close();
                        return StatusCode((int)HttpStatusCode.Created, rep);
                    }catch(SqlException ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ex.Message);
                    }
                }
            }
        }
    }
}
