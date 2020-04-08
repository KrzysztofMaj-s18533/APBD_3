using APBD_3.DTO;
using APBD_3.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
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

        public IActionResult promoteStudents(PromotionRequest req)
        {
            PromotionResponse rep = new PromotionResponse()
            {
               
            };
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18533;Integrated Security=True"))
            {
                client.Open();
                using(var com = new SqlCommand())
                using (var transaction = client.BeginTransaction())
                {
                    try
                    {
                        com.Connection = client;
                        com.CommandText = "SELECT * FROM Studies WHERE Name = @studyName;";
                        com.Parameters.AddWithValue("studyName", req.studies);
                        com.Transaction = transaction;
                        var dr = com.ExecuteReader();
                        if (!dr.Read())
                        {
                            dr.Close();
                            transaction.Rollback();
                            return BadRequest("No studies found with given Name");
                        }
                        int idStudy = (int)dr["IdStudy"];
                        dr.Close();
                        int sem = req.semester;
                        rep.idStudy = idStudy;
                        com.CommandText = "SELECT * FROM Enrollment WHERE IdStudy = @currId AND Semester = @sem;";
                        com.Parameters.AddWithValue("currId", idStudy);
                        com.Parameters.AddWithValue("sem", sem);
                        dr = com.ExecuteReader();
                        if (!dr.Read())
                        {
                            dr.Close();
                            transaction.Rollback();
                            return NotFound("Enrollment with given IdStudy and Semester was not found");
                        }
                        dr.Close();
                        com.CommandType = CommandType.StoredProcedure;
                        com.CommandText = "promoteStudents";
                        com.Parameters.Clear();
                        com.Parameters.AddWithValue("studyName", req.studies);
                        rep.semester = req.semester + 1;
                        com.Parameters.AddWithValue("semester", req.semester);
                        com.ExecuteNonQuery();
                        //if (!dr.Read())
                        //{
                        //    dr.Close();
                        //    transaction.Rollback();
                        //    return BadRequest("Something went wrong with the stored procedure");
                        //}
                        dr.Close();
                        com.CommandType = CommandType.Text;
                        com.CommandText = "SELECT IdEnrollment AS newId, StartDate FROM Enrollment WHERE IdStudy = @currId AND Semester = @sem";
                        com.Parameters.AddWithValue("currId", idStudy);
                        com.Parameters.AddWithValue("sem", rep.semester);
                        dr = com.ExecuteReader();
                        if (!dr.Read())
                        {
                            dr.Close();
                            transaction.Rollback();
                            return BadRequest("No enrollment with higher semester! Something must have went wrong!");
                        }
                        rep.idEnrollment = (int)dr["newId"];
                        rep.startDate = (DateTime)dr["StartDate"];
                        dr.Close();
                        transaction.Commit();
                        return StatusCode((int)HttpStatusCode.Created, rep);
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ex.Message);
                    }
                }
            }
        }

        public IActionResult registerStudents(RegisterRequest req)
        {
            RegisterResponse rep = new RegisterResponse()
            {
                semester = 1,
                startDate = DateTime.Today
            }; 
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
                        rep.idStudy = idStudies;

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

                        rep.idEnrollment = idEnrollment;

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

        public bool findStud(string index)

        {
            using (var com = new SqlCommand())
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18533;Integrated Security=True"))
            {
                client.Open();
                com.Connection = client;
                com.CommandText = "SELECT * from Student where IndexNumber = @ind";
                com.Parameters.AddWithValue("ind", index);
                var dr = com.ExecuteReader();

                if (dr.Read())
                {
                    dr.Close();
                    return true;
                }
                else
                {
                    dr.Close();
                    return false;
                }
            }

        }
    }
}
