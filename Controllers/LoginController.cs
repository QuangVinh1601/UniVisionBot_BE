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
            Response.Cookies.Append("JWT", result.AccessToken);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("user")]
        public async Task<IActionResult> User()
        {
            var JwtToken =  Request.Cookies["JWT"];
            if (string.IsNullOrEmpty(JwtToken))
            {
                throw new BadInputException("Token is missing");
            }
            var verifiedToken = await _loginRepository.VerifyToken(JwtToken);
            var userId = verifiedToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
                return Ok(userId);
            
        }

      
    }
}
