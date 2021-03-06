﻿using cw5_6.DTOs.Requests;
using cw5_6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw5_6.Services
{
    public interface IStudentDbService
    {
        void EnrollStudent(EnrollStudentRequest request);
        void PromoteStudents(int semester, string studies);
        Student GetStudent(string IndexNumber);
    }
}
