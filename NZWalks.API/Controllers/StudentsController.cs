using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
         private string[] _students = new string[] { "John", "Jane", "Bob" };

        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(_students);
        }
    }
}
