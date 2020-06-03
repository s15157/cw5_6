using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cw5_6.Models;
using cw5_6.DTOs.Requests;
using cw5_6.DTOs.Responses;
using System.Data.SqlClient;
using System.Data;
using cw5_6.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore;

namespace cw5_6.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly s15157Context _dbContext;

        public EnrollmentsController(s15157Context dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            if (!ModelState.IsValid)
            {
                var d = ModelState;
                return BadRequest("!!!");
            }

            var st = new Student1();
            st.IndexNumber = request.IndexNumber;
            st.FirstName = request.FirstName;
            st.LastName = request.LastName;
            st.BirthDate = request.BirthDate;
            st.Studies = request.Studies;

            var response = new EnrollStudentResponse();
            response.IndexNumber = st.IndexNumber;
            response.FirstName = st.FirstName;
            response.LastName = st.LastName;
            response.BirthDate = st.BirthDate;
            response.Studies = st.Studies;


            var idStudy = from _dbContext in _dbContext.Studies
                          where _dbContext.Name == request.Studies
                          select _dbContext.IdStudy;
            if(Convert.ToInt32(idStudy) == 0)
            {
                return BadRequest("Studia nie istnieja!");
            }

            var countEnroll = (from _dbContext in _dbContext.Enrollment
                               where _dbContext.IdStudy == Convert.ToInt32(idStudy) && _dbContext.Semester == 1
                               select _dbContext).Count();

            var idEnrollCount = (from _dbContext in _dbContext.Enrollment
                                   select _dbContext).Count();
            int idEnroll;

            if(Convert.ToInt32(countEnroll) > 0)
            {
                var _idEnroll = from _dbContext in _dbContext.Enrollment
                           where _dbContext.IdStudy == Convert.ToInt32(idStudy) && _dbContext.Semester == 1
                           select _dbContext.IdEnrollment;

                idEnroll = Convert.ToInt32(_idEnroll);
            }else
            {
                DateTime thisDay = DateTime.Today;
                var db = new s15157Context();
                var en = new Enrollment()
                {
                    IdEnrollment = idEnrollCount+1, 
                    Semester = 1,
                    IdStudy = Convert.ToInt32(idStudy),
                    StartDate = thisDay
                };
                db.Enrollment.Add(en);
                db.SaveChanges();

                idEnroll = idEnrollCount + 1;
            }

            var uniqueIndex = (from _dbContext in _dbContext.Student
                              where _dbContext.IndexNumber == request.IndexNumber
                              select _dbContext).Count();

           
            if (uniqueIndex > 0)
            {
                return BadRequest("Podany indeks już istnieje");
            }
            else
            {
                var db = new s15157Context();
                var st1 = new Student()
                {
                    IndexNumber = request.IndexNumber,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    BirthDate = request.BirthDate,
                    IdEnrollment = idEnroll

                };

                db.Student.Add(st1);
                db.SaveChanges();
            }

            return Ok(response);
        }

        [HttpPost("promotions")]
        [Authorize(Roles = "employee")]
        public IActionResult Promotions(EnrollPromotionsRequest request)
        {
            if (!ModelState.IsValid)
            {
                var d = ModelState;
                return BadRequest("!!!");
            }

            var _idStudy = from _dbContext in _dbContext.Studies
                           where _dbContext.Name == request.Studies
                           select _dbContext.IdStudy;

            var _idEnroll = from _dbContext in _dbContext.Enrollment
                            where _dbContext.IdStudy == Convert.ToInt32(_idStudy) && _dbContext.Semester == request.Semester
                            select _dbContext.IdEnrollment;


            var studies = new SqlParameter("studies", request.Studies);
            var semester = new SqlParameter("semester", request.Semester);
            _dbContext.Database.ExecuteSqlRaw("EXEC PromoteStudents @studies, @semester");

            var _startDate = from _dbContext in _dbContext.Enrollment
                             where _dbContext.IdStudy == Convert.ToInt32(_idStudy) && _dbContext.Semester == request.Semester + 1
                             select _dbContext.StartDate;

            var enroll = new Enrollment1();
            enroll.IdEnrollment = Convert.ToInt32(_idEnroll);
            enroll.IdStudy = Convert.ToInt32(_idStudy);
            enroll.Semester = request.Semester + 1;
            enroll.StartDate = Convert.ToDateTime(_startDate);

            return Ok(enroll);

        }

    }
}