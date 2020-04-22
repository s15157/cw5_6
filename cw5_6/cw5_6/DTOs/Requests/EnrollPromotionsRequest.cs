using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cw5_6.DTOs.Requests
{
    public class EnrollPromotionsRequest
    {
       
        [Required(ErrorMessage = "Musisz podać studia")]
        public string Studies { get; set; }
        [Required(ErrorMessage = "Musisz podać semestr")]
        public int Semester { get; set; }
    }
}
