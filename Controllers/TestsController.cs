using Microsoft.AspNetCore.Mvc;

namespace CourseraWebApi.Controllers
{

    [Route("[controller]")]
    public class TestsController : ControllerBase
    {
        [HttpGet("throw-error")]
        public IActionResult ThrowError()
        {
            throw new InvalidOperationException("This is a test error from the TestController.");
        }
    }
}