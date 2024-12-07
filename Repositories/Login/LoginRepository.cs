using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using UniVisionBot.Configurations.Options;
using UniVisionBot.DTOs.Login;
using UniVisionBot.DTOs.Register;
using UniVisionBot.DTOs.Role;
using UniVisionBot.Models;
using UniVisionBot.Services.Login;

namespace UniVisionBot.Repositories.Login
{
    public class LoginRepository : ILoginRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManger;
        private readonly IOptions<AppSettings> _options;
        public LoginRepository(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IOptions<AppSettings> options)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _options = options;
            _signInManger = signInManager;
        }

        public async Task<RegisterResponse> CreateAdminRoleAsync(RegisterRequest request)
        {
            try
            {
                var userExisted = await _userManager.FindByEmailAsync(request.Email);
                if (userExisted != null)
                {
                    return new RegisterResponse { Message = "User is existed", Success = false };
                }

                userExisted = new AppUser()
                {
                    Email = request.Email,
                    FullName = request.FullName,
                    UserName = request.Email,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };
                var resultCreateUser = await _userManager.CreateAsync(userExisted, request.Password);
                if (!resultCreateUser.Succeeded)
                {
                    return new RegisterResponse { Message = "Failed to create user", Success = false };
                }
                var resultAddRole = await _userManager.AddToRoleAsync(userExisted, "ADMIN");
                if (!resultAddRole.Succeeded)
                {
                    return new RegisterResponse { Message = "Fail to add role", Success = false };
                }
                return new RegisterResponse
                {
                    Success = true,
                    Message = "Successfull create user"
                };
            }
            catch (Exception ex)
            {
                {
                    return new RegisterResponse { Message = ex.Message, Success = false };
                }
            }
        }

        public async Task<RegisterResponse> CreateConsultantRoleAsync(RegisterRequest request)
        {
            try
            {
                var userExisted = await _userManager.FindByEmailAsync(request.Email);
                if (userExisted != null)
                {
                    return new RegisterResponse { Message = "User is existed", Success = false };
                }

                userExisted = new AppUser()
                {
                    Email = request.Email,
                    FullName = request.FullName,
                    UserName = request.Email,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };
                var resultCreateUser = await _userManager.CreateAsync(userExisted, request.Password);
                if (!resultCreateUser.Succeeded)
                {
                    return new RegisterResponse { Message = "Failed to create user", Success = false };
                }
                var resultAddRole = await _userManager.AddToRoleAsync(userExisted, "CONSULTANT");
                if (!resultAddRole.Succeeded)
                {
                    return new RegisterResponse { Message = "Fail to add role", Success = false };
                }
                return new RegisterResponse
                {
                    Success = true,
                    Message = "Successfull create user"
                };
            }
            catch (Exception ex)
            {
                {
                    return new RegisterResponse { Message = ex.Message, Success = false };
                }
            }
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new LoginResponse { Message ="Invalid email/password" , Success = false };            
            }

            var signInResult = await _signInManger.CheckPasswordSignInAsync(user, request.Password, false);
            if (!signInResult.Succeeded)
            {
                return new LoginResponse { Message = "Invalid email/password", Success = false };
            }
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name , user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };

            var userRole =  await _userManager.GetRolesAsync(user);

            var listRole = userRole.Select(x => new Claim(ClaimTypes.Role, x));
            claim.AddRange(listRole);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey));
            var signingCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expired = DateTime.Now.AddMinutes(30);

            var token = new JwtSecurityToken(
                  issuer: "https://localhost:7230",
                  audience: "https://localhost:7230",
                  claims: claim,
                  expires: expired,
                  signingCredentials: signingCredential
                  );
            var createToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse
            {
                AccessToken = createToken,
                RoleUser = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "USER",
                Email = user.Email,
                Success = true,
                Message = "Login successfull",
                FullName = user.FullName,
                UserId = user.Id.ToString(),
            }; 

        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var userExisted = await _userManager.FindByEmailAsync(request.Email);
                if (userExisted != null)
                {
                    return new RegisterResponse { Message = "User is existed", Success = false };
                }

                userExisted = new AppUser()
                {
                    Email = request.Email,
                    FullName = request.FullName,
                    UserName = request.Email,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };
                var resultCreateUser = await _userManager.CreateAsync(userExisted, request.Password);
                if (!resultCreateUser.Succeeded)
                {
                    return new RegisterResponse { Message = "Failed to create user", Success = false };
                }
                var resultAddRole = await _userManager.AddToRoleAsync(userExisted, "USER");
                if (!resultAddRole.Succeeded)
                {
                    return new RegisterResponse { Message = "Fail to add role", Success = false };
                }
                return new RegisterResponse
                {
                    Success = true,
                    Message = "Successfull create user"
                };
            }
            catch (Exception ex)
            {
                {
                    return new RegisterResponse { Message = ex.Message, Success = false };
                }
            }
        }

        public async Task<RoleResponse> RoleAsync(RoleRequest request)
        {
            var role = new AppRole
            {
                Name = request.RoleUser
            };
            var resultCreateRole = await _roleManager.CreateAsync(role);
            if (!resultCreateRole.Succeeded)
            {
                return new RoleResponse { Message = "Failed to create role" , Success = false };
            }
            return new RoleResponse { Message = "Create successfull", Success = true };
        }



        public async Task<JwtSecurityToken> VerifyToken(string jwtToken)
        {
            var handlerToken = new JwtSecurityTokenHandler();
            var secretKey = Encoding.UTF8.GetBytes(_options.Value.SecretKey);
            handlerToken.ValidateToken(jwtToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateIssuer = true,
                ValidateAudience = true,
            }, out SecurityToken validatedToken);

            return (JwtSecurityToken)validatedToken;

        }
    }
}
