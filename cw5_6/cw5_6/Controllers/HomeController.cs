using cw5_6.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw5_6.Controllers
{
    [Route("api/students1")]
    public class HomeController : ControllerBase
    {
        private readonly s15157Context _dbContext;

        public HomeController(s15157Context dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetStudent()
        {
            List<Student> students = new List<Student>();

            var stdLsts = from _dbContext in _dbContext.Student
                      select _dbContext;

            var _studentList = (from _dbContext in _dbContext.Student
                              select new StudentViewModel
                              {
                                  IndexNumber = _dbContext.IndexNumber,
                                  FirstName = _dbContext.FirstName,
                                  LastName = _dbContext.LastName,
                                  BirthDate = _dbContext.BirthDate,
                                  IdEnrollment = _dbContext.IdEnrollment
                              }).ToList();

            IList<StudentViewModel> studentList = _studentList;


            return Ok(studentList);
        }

        [HttpPut("update")]
        public IActionResult UpdateStudent(StudentViewModel request)
        {
            var db = new s15157Context();

            var st = new StudentViewModel
            {
                IndexNumber = request.IndexNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate,
                IdEnrollment = request.IdEnrollment
            };

            db.Attach(st);
            db.SaveChanges();

            return Ok(st);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(string id)
        {
            var db = new s15157Context();
            var st = new Student
            {
                IndexNumber = id
            };
            db.Attach(st);
            db.Remove(st);
            db.SaveChanges();

                return Ok("Usuwanie ukończone");
        }
    }
}
