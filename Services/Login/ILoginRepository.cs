using System.IdentityModel.Tokens.Jwt;
using UniVisionBot.DTOs.Login;
using UniVisionBot.DTOs.Register;
using UniVisionBot.DTOs.Role;

namespace UniVisionBot.Services.Login
{
    public interface ILoginRepository
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync (LoginRequest request);
        Task<RoleResponse> RoleAsync (RoleRequest request);
        
        Task<JwtSecurityToken> VerifyToken(string jwtToken);

        Task<RegisterResponse> CreateConsultantRoleAsync (RegisterRequest request);
        Task<RegisterResponse> CreateAdminRoleAsync (RegisterRequest request);
    }
}
