using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WebApplicationDapper.Models;

namespace WebApplicationDapper.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private string connectionString = WebApplication.CreateBuilder().Configuration.GetConnectionString("DefaultConnection");
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "select * from Regist";
                IEnumerable<Users> users = connection.Query<Users>(query);

                return Ok(users);
            }
        }
        [HttpGet]
        public IActionResult GetAllUsersByProcedure()
        {
            using(var connection = new SqlConnection(connectionString))
            {
                var users = connection.Query(sql:"HammaUser",commandType: CommandType.StoredProcedure);

                return Ok(users);
            }
        }
        [HttpPost]
        public IActionResult InsertAct(UserDto userDto)
        {
            using(var connection = new SqlConnection(connectionString))
            {
                string query = $"insert into Regist Values('{userDto.email}'," +
                    $"'{userDto.firstName}','{userDto.lastName}','{userDto.userName}'" +
                    $",'{userDto.parol}')";


                connection.Execute(query);
                return Ok("Created");
            }
        }
        [HttpGet]
        public async ValueTask<IActionResult> GetMultipleQuery()
        {
            using (var connection = new SqlConnection(connectionString)) {
                
                string query = "select * from Regist; select * from IdIsm";
                
                var multipleTable = await connection.QueryMultipleAsync(query);
                
                var firstTable = multipleTable.ReadAsync<Users>().Result;
                var secondTable = multipleTable.ReadAsync<IdIsm>().Result;
                
                return Ok(secondTable);
            }
        }
        [HttpPost]
        public IActionResult studendsBySurName(string surname)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("surname",surname);
                var users = connection.Query(sql: "getStudentBySurname",param:dynamicParameters, commandType: CommandType.StoredProcedure);

                return Ok(users);
            }
        }
        [HttpPost]
        public IActionResult getStudentsEnterToUniversityBetweenTwoDates(int firsDate , int secondDate )
        {
            using (var connection = new SqlConnection(connectionString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("firstdate", firsDate);
                dynamicParameters.Add("lastdate", secondDate);
                var users = connection.Query(sql: "getStudentsEnterBetweenTwoDates", param: dynamicParameters, commandType: CommandType.StoredProcedure);

                return Ok(users);
            }
        }
        [HttpPost]
        public IActionResult showStudentsWhichHasWord(string Word)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "select * from Students";
                IEnumerable<Students> students = connection.Query<Students>(query);
                IList<Students> stds = new List<Students>();
                foreach(var i in students)
                {
                    Console.WriteLine(i.lastName);
                    bool bor = false;
                    if (i.Id.ToString().Contains(Word)) bor = true; 
                    if (i.firstName.ToLower().Contains(Word.ToLower())) bor = true; 
                    if (i.lastName.ToLower().Contains(Word.ToLower())) bor = true; 
                    if (i.enterDate.ToLower().Contains(Word.ToLower())) bor = true; 
                    if (i.yearOfCourse.ToLower().Contains(Word.ToLower())) bor = true;
                    Console.WriteLine(bor);
                    if (bor == true)
                    {
                        stds.Add(i);
                    }
                }
                return Ok(stds);
            }
        }
    }
}
