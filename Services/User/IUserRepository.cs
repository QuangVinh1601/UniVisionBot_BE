using UniVisionBot.DTOs.User;

namespace UniVisionBot.Services.User
{
    public interface IUserRepository
    {
        Task<List<UserResponse>> GetAllUser();
        Task DeleteUser(string userId);
        Task UpdateUser(string userId, UserRequest request);
        Task<UserResponse> GetUserById(string id);
    }
}
