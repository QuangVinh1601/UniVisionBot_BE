using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UniVisionBot.Configurations.DbConfig;
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
        private readonly IOptions<MyDatabase> _optionsDatabase;
        private readonly IMongoCollection<RefreshToken> _refreshTokenCollection;
        private readonly IMongoCollection<AppUser> _appUserCollection;
        public LoginRepository(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IOptions<MyDatabase> optionsDatabase, IOptions<AppSettings> options)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _options = options;
            _signInManger = signInManager;
            _optionsDatabase = optionsDatabase;
            var connectionString = new MongoClient(_optionsDatabase.Value.ConnectionString);
            var database = connectionString.GetDatabase(_optionsDatabase.Value.DatabaseName);
            _refreshTokenCollection = database.GetCollection<RefreshToken>(_optionsDatabase.Value.RefreshTokenCollectionName);
            _appUserCollection = database.GetCollection<AppUser>(_optionsDatabase.Value.AppUsersCollectionName);
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
                return new LoginResponse { Message = "Invalid email/password", Success = false };
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

            var userRole = await _userManager.GetRolesAsync(user);

            var listRole = userRole.Select(x => new Claim(ClaimTypes.Role, x));
            claim.AddRange(listRole);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey));
            var signingCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expired = DateTime.Now.AddHours(5);

            var token = new JwtSecurityToken(
                  issuer: "https://localhost:7230",
                  audience: "https://localhost:7230",
                  claims: claim,
                  expires: expired,
                  signingCredentials: signingCredential
                  );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            //var refreshToken = new RefreshToken
            //{
            //    Token = Guid.NewGuid(),
            //    UserId = user.Id,
            //    Expired = DateTime.Now.AddDays(30),
            //    isRevoked = false
            //};

            //await _refreshTokenCollection.InsertOneAsync(refreshToken);
            return new LoginResponse
            {
                Token = accessToken,
                //RefreshToken = refreshToken.Token.ToString(),
                RoleUser = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "USER",
                Email = user.Email,
                Success = true,
                Message = "Login successfull",
                FullName = user.FullName,
                UserId = user.Id.ToString(),
            };

        }
        //private static string GetRefreshToken()
        //{
        //    var tokenByteArray = new byte[32];
        //    using (var ramdom = RandomNumberGenerator.Create())
        //    {
        //        ramdom.GetBytes(tokenByteArray);
        //    }
        //    return Convert.ToBase64String(tokenByteArray);
        //}

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
                return new RoleResponse { Message = "Failed to create role", Success = false };
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

        //public async Task<Token> ResetAccessToken(string refreshToken)
        //{
        //    var token = await _refreshTokenCollection.Find(rt => rt.Token == Guid.Parse(refreshToken)).FirstOrDefaultAsync();
        //    if (token == null)
        //    {
        //        throw new Exception("Cannot find the refreshToken");
        //    }
        //    if (token.Expired < DateTime.UtcNow)
        //    {
        //        throw new SecurityTokenExpiredException("Expired token");
        //    }
        //    token.isRevoked = true;
        //    var filter = Builders<RefreshToken>.Filter.Eq(rt => rt.Id, token.Id);
        //    await _refreshTokenCollection.ReplaceOneAsync(filter, token);

        //    var user = await _appUserCollection.Find(u => u.Id == token.UserId).FirstOrDefaultAsync();

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey));

        //    var signingCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        //    var claim = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //        new Claim(ClaimTypes.Name , user.Email),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
        //    };
        //    var roles = await _userManager.GetRolesAsync(user);
        //    var roleUser = roles.Select(r => new Claim(ClaimTypes.Role, r.ToString())).FirstOrDefault();
        //    claim.Add(roleUser);
        //    var accessToken = new JwtSecurityToken(claims: claim,
        //                                           expires: DateTime.UtcNow.AddMinutes(30),
        //                                           issuer: "https://localhost:7230",
        //                                           audience: "https://localhost:7230",
        //                                           signingCredentials: signingCredential);
        //    var newAccessToken = tokenHandler.WriteToken(accessToken);
        //    var newRefreshToken = Guid.NewGuid();
        //    var refreshTokenDocument = new RefreshToken
        //    {
        //        Expired = DateTime.UtcNow.AddDays(30),
        //        UserId = user.Id,
        //        Token = newRefreshToken,
        //        isRevoked = false,
        //    };
        //    await _refreshTokenCollection.InsertOneAsync(refreshTokenDocument);

        //    return new Token
        //    {
        //        AccessToken = newAccessToken,
        //        RefreshToken = newRefreshToken.ToString(),
        //    };
        //}
    }
}
