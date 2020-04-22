using cw5_6.DTOs.Requests;
using cw5_6.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cw5_6.Services
{
    public class SqlServerStudentDbService : IStudentDbService
    {
        public void EnrollStudent(EnrollStudentRequest request)
        {
           

            var st = new Student();
            st.IndexNumber = request.IndexNumber;
            st.FirstName = request.FirstName;
            st.LastName = request.LastName;
            st.BirthDate = request.BirthDate;
            st.Studies = request.Studies;

            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s15157;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                com.Transaction = tran;

                try
                {
                    com.CommandText = "select IdStudy from studies where name=@name";
                    com.Parameters.AddWithValue("name", request.Studies);
                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {

                        //return BadRequest("Studia nie istnieja");

                    }

                    int idstudy = (int)dr["IdStudy"];

                    dr.Close();
                    com.CommandText = "select COUNT(*) from Enrollment where IdStudy=@IdStudy AND Semester = 1";
                    com.Parameters.AddWithValue("IdStudy", idstudy);
                    int exist = (int)com.ExecuteScalar();
                    int idEnrollment;
                    if (exist > 0)
                    {
                        com.CommandText = "select IdEnrollment from Enrollment where IdStudy=@IdStudy AND Semester = 1";
                        dr = com.ExecuteReader();
                        if (dr.Read())
                            idEnrollment = (int)dr["IdEnrollment"];
                        else
                            idEnrollment = 0;

                        dr.Close();
                    }
                    else
                    {

                        DateTime thisDay = DateTime.Today;
                        com.CommandText = "INSERT INTO Enrollment(IdEnrollment,Semester,IdStudy,StartDate) VALUES ((SELECT COUNT(*)+1 FROM Enrollment),1,@IdStudy,@StartDate)";
                        com.Parameters.AddWithValue("StartDate", thisDay);
                        com.ExecuteNonQuery();

                        com.CommandText = "select COUNT(*) from Enrollment";
                        idEnrollment = (int)com.ExecuteScalar();
                    }



                    //x. Dodanie studenta
                    com.CommandText = "select COUNT(*) from Student where IndexNumber=@IndexNumber";
                    com.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                    int uniqueIndex = (int)com.ExecuteScalar();

                    if (uniqueIndex > 0)
                    {
                        tran.Rollback();
                        //return BadRequest("Podany indeks już istnieje");
                    }
                    else
                    {
                        com.CommandText = "INSERT INTO Student(IndexNumber,FirstName,LastName,BirthDate,IdEnrollment) Values (@IndexNumber,@FirstName,@LastName,@BirthDate,@IdEnrollment)";
                        com.Parameters.AddWithValue("FirstName", request.FirstName);
                        com.Parameters.AddWithValue("LastName", request.LastName);
                        com.Parameters.AddWithValue("BirthDate", request.BirthDate);
                        com.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                        com.ExecuteNonQuery();
                    }


                    dr.Close();
                    tran.Commit();
                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                }
            }
            throw new NotImplementedException();
        }

        public void PromoteStudents(int semester, string studies)
        {
            throw new NotImplementedException();
        }
    }
}
