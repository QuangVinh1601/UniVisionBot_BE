using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniVisionBot.DTOs.Chat;
using UniVisionBot.DTOs.User;
using UniVisionBot.Services.User;

namespace UniVisionBot.Area.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _userRepository.GetAllUser();
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var result = await _userRepository.GetUserById(id);
            return Ok(result);
        }
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _userRepository.DeleteUser(userId);
            return Ok();
        }
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, UserRequest request)
        {
            await _userRepository.UpdateUser(userId, request);
            return Ok();
        }
    }
}
