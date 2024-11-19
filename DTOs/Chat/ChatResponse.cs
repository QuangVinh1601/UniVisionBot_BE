using UniVisionBot.Enum;
using UniVisionBot.Models;

namespace UniVisionBot.DTOs.Chat
{
    public class ConversationResponse
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ConsultantId { get; set; }
        public DateTime Created_At { get; set; }
        public List<MessageResponse> Messages { get; set; }
        public UserResponse User { get; set; }
        public string? LastMessage { get; set; }
    }
    public class UserResponse
    {
        public string FullName { get; set; }
        public string? UrlImage { get; set; }

    }
    public class MessageResponse
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public StatusChatEnum Status { get; set; }
        public DateTime Created_At { get; set; }
    }
    public class MessageRequest
    {
        public string Content { get; set; }
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public StatusChatEnum Status { get; set; }
    }
    public class ConversationRequest
    {
        public string UserId { get; set; }
        public string ConsultantId { get; set; }
    }
}
