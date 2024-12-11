using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using UniVisionBot.DTOs.Login;
using UniVisionBot.DTOs.Register;
using UniVisionBot.DTOs.Role;
using UniVisionBot.Exceptions;
using UniVisionBot.Models;
using UniVisionBot.Services.Login;

namespace UniVisionBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ILoginRepository _loginRepository;


        public LoginController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, ILoginRepository loginRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _loginRepository = loginRepository;
        }

        [HttpPost]
        [Route("roles/add")]
        public async Task<IActionResult> CreateRole([FromBody] RoleRequest request)
        {
            var result = await _loginRepository.RoleAsync(request);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost]
        [Route("register/user")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _loginRepository.RegisterAsync(request);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost]
        [Route("register/consultant")] 
        public async Task<IActionResult> RegisterConsultant ([FromBody] RegisterRequest request)
        {
            var result = await _loginRepository.CreateConsultantRoleAsync(request);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost]
        [Route("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request)
        {
            var result = await _loginRepository.CreateAdminRoleAsync(request);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {   
            var result = await _loginRepository.LoginAsync(request);
            if (result.Success)
            {
                Response.Cookies.Append("AccessToken", result.AccessToken.ToString());
                Response.Cookies.Append("RefreshToken", result.RefreshToken.ToString());
            }
            return result.Success ? Ok(result) : Unauthorized(result.Message);
        }
        [HttpPost("{refreshToken}")]
        public async Task<IActionResult> ResetAccessToken(string refreshToken)
        {
            try
            {
                var result = await _loginRepository.ResetAccessToken(refreshToken);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (SecurityTokenExpiredException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

      
    }
}
