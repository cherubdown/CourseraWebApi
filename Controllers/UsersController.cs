using Microsoft.AspNetCore.Mvc;

namespace CourseraWebApi.Controllers
{
    public class UsersController(IUserRepository userRepository) : ControllerBase
    {
        [HttpGet]
        [Route("api/users")]
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
        [Route("api/users/{id:int}")]
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
        [Route("api/users")]
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
        [Route("api/users/{id:int}")]
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
        [Route("api/users/{id:int}")]
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