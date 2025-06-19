using Microsoft.AspNetCore.Mvc;

namespace CourseraWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserRepository userRepository) : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<User>> Get()
        {
            var users = userRepository.GetAll();
            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }
            return Ok(users);
        }

        [HttpGet]
        [Route("{id:int}")]
        public ActionResult<User> GetById(int id)
        {
            var user = userRepository.GetById(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return Ok(user);
        }

        [HttpPost]
        public ActionResult<string> Post([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User cannot be null.");
            }
            userRepository.Post(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut]
        public ActionResult<string> Put(int id, [FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User cannot be null.");
            }
            try
            {
                userRepository.Put(id, user);
                return Ok($"Updated user {id} to: {user}");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete]
        public ActionResult<string> Delete(int id)
        {
            try
            {
                userRepository.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}