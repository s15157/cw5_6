using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw5_6.DTOs.Responses
{
    public class EnrollStudentResponse
    {
        public string IndexNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Studies { get; set; }
    }
}
