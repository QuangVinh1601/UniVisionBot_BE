using UniVisionBot.DTOs.Chat;

namespace UniVisionBot.Services.ChatHub
{
    public interface IChatHubRepository
    {
        Task<MessageResponse> SaveMessage(MessageRequest request);
    }
}
