using UniVisionBot.DTOs.Chat;

namespace UniVisionBot.Services.Chat
{
    public interface IChatRepository
    {
        Task<List<ConversationResponse>> GetAllConversationForConsultant(string consultantId);
        Task<ConversationResponse> GetConversationForCurrentUser(string currentUserId);
        Task<ConversationResponse> GetHistoryMessage(string conversationId);
        Task<string> CreateConversation(ConversationRequest request);
    }
}
