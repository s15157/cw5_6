using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using cw5_6.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using cw5_6.DTOs.Requests;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace cw5_6.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : Controller
    {
        /*
         * HttpGet, -> Pobierz z 80
         * HttpPost, -> Dodaj zasób do BD
         * HttpPut, -> Zaktualizuj zasób
         * HttpPatch,-> Załataj (częsciowa aktualizacja)
         * HttpDelete -> Usuń zasób
         */
        public IConfiguration Configuration { get; set; }

        public StudentsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet]
        [Authorize]
        public List<Student1> GetStudents()
        {
            return SelectAllStudents();
        }


        [HttpPost("{addStudent}")]
        public IActionResult AddStudent(Student1 student)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s15157;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT IdStudy FROM Studies WHERE Name=@name;";
                com.Parameters.AddWithValue("name", student.Studies);
                con.Open();

                var dr = com.ExecuteReader();
                var idStudy = (int)com.ExecuteScalar();
                dr.Close();

                com.CommandText = "SELECT IdEnrollment FROM Enrollment WHERE IdStudy=@idStudy";
                com.Parameters.AddWithValue("idStudy", idStudy);

                var idEnrollment = (int)com.ExecuteScalar();
                dr.Close();

                com.CommandText = "INSERT INTO Student(IndexNumber,FirstName,LastName,BirthDate,IdEnrollment) Values (@IndexNumber,@FirstName,@LastName,@BirthDate,@IdEnrollment)";
                com.Parameters.AddWithValue("IndexNumber", student.IndexNumber);
                com.Parameters.AddWithValue("FirstName", student.FirstName);
                com.Parameters.AddWithValue("LastName", student.LastName);
                com.Parameters.AddWithValue("BirthDate", student.BirthDate);
                com.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                com.ExecuteNonQuery();


            }
            
            return Ok("Dodano studenta do bazy!");
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentById(int id)
        {

            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s15157;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT * FROM Student Where IndexNumber=@id";
                com.Parameters.AddWithValue("id", id);
                con.Open();

                var dr = com.ExecuteReader();

                if (dr.Read())
                {
                    var st = new Student1();

                    st.IndexNumber = dr.GetString(dr.GetOrdinal("IndexNumber"));
                    st.FirstName = dr.GetString(dr.GetOrdinal("FirstName"));
                    st.LastName = dr.GetString(dr.GetOrdinal("LastName"));
                    st.BirthDate = dr.GetDateTime(dr.GetOrdinal("BirthDate"));

                    com.CommandText = "SELECT IdStudy FROM Enrollment WHERE IdEnrollment=@IdEnrollment";
                    com.Parameters.AddWithValue("IdEnrollment", dr.GetInt32(dr.GetOrdinal("IdEnrollment")));
                    dr.Close();
                    var IdStudy = (int)com.ExecuteScalar();

                    com.CommandText = "SELECT Name FROM Studies WHERE IdStudy=@IdStudy";
                    com.Parameters.AddWithValue("IdStudy", IdStudy);
                    dr.Close();
                    dr = com.ExecuteReader();
                    dr.Close();
                    if (dr.Read())
                    {
                        st.Studies = dr.GetString(dr.GetOrdinal("Name"));
                    }
                    return Ok(st);
                }
            }

                return NotFound("Nie znaleziono studenta");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id)
        {

            return Ok("Aktualizacja dokończona");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s15157;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "DELETE FROM Student Where IndexNumber=@id";
                com.Parameters.AddWithValue("id", id);
                con.Open();

                com.ExecuteNonQuery();
            }
                return Ok("Usuwanie ukończone");
        }

        public List<Student1> SelectAllStudents()
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s15157;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT * FROM Student";
                con.Open();
                var dr = com.ExecuteReader();

                List<Student1> students = new List<Student1>();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                            var st = new Student1();

                            st.IndexNumber = Convert.ToString(dr["IndexNumber"]);
                            st.FirstName = Convert.ToString(dr["FirstName"]);
                            st.LastName = Convert.ToString(dr["LastName"]);
                            st.BirthDate = Convert.ToDateTime(dr["BirthDate"]);
                            st.Studies = GetStudies(st.IndexNumber, Convert.ToInt32(dr["IdEnrollment"]));

                            students.Add(st);
                    }
                }
                dr.Close();

                return students;
            }
        }

        public String GetStudies(String indexNumber, int idEnrollment)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s15157;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT IdStudy FROM Enrollment WHERE IdEnrollment=@IdEnrollment";
                com.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                con.Open();
                var dr = com.ExecuteReader();
                
                dr.Close();
                var IdStudy = (int)com.ExecuteScalar();

                dr.Close();
                com.CommandText = "SELECT Name FROM Studies WHERE IdStudy=@IdStudy";
                com.Parameters.AddWithValue("IdStudy", IdStudy);

                dr = com.ExecuteReader();
                if (dr.Read())
                {
                    return Convert.ToString(dr["Name"]);
                }
            }
            return "Brak";
        }

        [HttpPost]
        public IActionResult Login(LoginRequestDto request)
        {
            
            bool login = false;
            bool passw = false;
            string name = "Brak";

            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s15157;Integrated Security=True"))
            using (var com = new SqlCommand())
            {   
                com.Connection = con;
                com.CommandText = "SELECT IndexNumber, FirstName, LastName, Password, Salt FROM Student WHERE IndexNumber=@IndexNumber";
                com.Parameters.AddWithValue("IndexNumber", request.Login);
                con.Open();
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    if(Convert.ToString(dr["IndexNumber"]).Equals(request.Login))
                    { 
                        login = true;
                        passw = Validate(request.Password, Convert.ToString(dr["Salt"]), Convert.ToString(dr["Password"]));
                        name = Convert.ToString(dr["FirstName"]) + " " + Convert.ToString(dr["LastName"]);
                    }
               
                }
            }

                var claims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier, request.Login),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, "employee")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds

            );

            if(login == true && passw == true)
            {
                var tokens = new JwtSecurityTokenHandler().WriteToken(token);
                var refreshtoken = Guid.NewGuid();
                using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s15157;Integrated Security=True"))
                using (var com = new SqlCommand())
                {
                    com.Connection = con;
                    com.CommandText = "SELECT RefToken FROM Student WHERE IndexNumber=@IndexNumber";
                    com.Parameters.AddWithValue("IndexNumber", request.Login);
                    con.Open();

                    var dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                            return Ok(RefreshToken(Convert.ToString(dr["RefToken"])));
                        
                    }
                    
                }
            }
            
            return Unauthorized();
        }

        [HttpPost("refresh-token/{token}")]
        public IActionResult RefreshToken(string refToken)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s15157;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT IndexNumber, FirstName, LastName FROM Student WHERE RefToken=@RefToken";
                com.Parameters.AddWithValue("RefToken", refToken);
                con.Open();
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, Convert.ToString(dr["IndexNumber"])),
                            new Claim(ClaimTypes.Name, Convert.ToString(dr["FirstName"] + " " + Convert.ToString(dr["LastName"]))),
                            new Claim(ClaimTypes.Role, "employee")
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken
                        (
                            issuer: "Gakko",
                            audience: "Students",
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(10),
                            signingCredentials: creds

                        );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token)
                        });
                }
            }
            return NotFound();
                 
        }

        public static string Create(string value, string salt)
        {

            var valueBytes = KeyDerivation.Pbkdf2(
                            password: value,
                            salt: Encoding.UTF8.GetBytes(salt),
                            prf: KeyDerivationPrf.HMACSHA512,
                            iterationCount: 10000,
                            numBytesRequested: 256 / 8
                            );
            return Convert.ToBase64String(valueBytes);
        }

        public static bool Validate(string value, string salt, string hash) => Create(value, salt) == hash;

        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public static void CreatePassword(string login, string passw)
        {
            var salt = CreateSalt();
            var password = Create(passw, salt);

            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s15157;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "UPDATE Student SET Password=@Password, Salt=@Salt WHERE IndexNumber=@IndexNumber";
                com.Parameters.AddWithValue("Password", password);
                com.Parameters.AddWithValue("Salt", salt);
                com.Parameters.AddWithValue("IndexNumber", login);
                con.Open();
                com.ExecuteNonQuery();

            }
        }

    }
}