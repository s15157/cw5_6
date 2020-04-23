using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cw5_6.Models
{
    public class Student
    {
        public string IndexNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Studies { get; set; }

        public void AddStudent()
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s15157;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.CommandText = "SELECT IdStudy FROM Studies WHERE Name=@Name";
                com.Parameters.AddWithValue("Name", Studies);
                var IdStudy = (int)com.ExecuteScalar();

                com.CommandText = "SELECT IdEnrollment FROM Enrollment WHERE IdStudy=@IdStudy AND Semester = 1";
                com.Parameters.AddWithValue("IdStudy",IdStudy);
                var IdEnrollment = (int)com.ExecuteScalar();
    
            com.CommandText = "INERT INTO Students (IndexNumber, FirstName, LastName, BirthDate, IdEnrollment ) values (@IndexNumber, @FirstName, @LastName, @BirthDate, @IdEnrollment)";
                com.Parameters.AddWithValue("IndexNumber", IndexNumber);
                com.Parameters.AddWithValue("FirstName", FirstName);
                com.Parameters.AddWithValue("LastName", LastName);
                com.Parameters.AddWithValue("BirthDate", BirthDate);
                com.Parameters.AddWithValue("IdEnrollment", IdEnrollment);
                com.ExecuteNonQuery();
            }
        }
    }
}

    

