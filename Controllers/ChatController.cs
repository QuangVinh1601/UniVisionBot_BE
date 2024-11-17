using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniVisionBot.DTOs.Chat;
using UniVisionBot.Services.Chat;

namespace UniVisionBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepository _chatRepository;
        public ChatController(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        [HttpGet("api/conversation/user/{currentUserId}")]
        public async Task<IActionResult> GetConversationForUser(string currentUserId)
        {
            var conversation = await _chatRepository.GetConversationForCurrentUser(currentUserId);
            return Ok(conversation);
        }

        [HttpGet("api/conversations/{consultantId}")]
        public async Task<IActionResult> GetAllConversationForConsultant(string consultantId)
        {
            var conversation = await _chatRepository.GetAllConversationForConsultant(consultantId);
            return Ok(conversation);
        }
        [HttpGet("api/conversation/history/{conversationId}")]
        public async Task<IActionResult> GetHistoryMessageForParticularUser (string conversationId)
        {
            var historyMessage = await _chatRepository.GetHistoryMessage(conversationId);
            return Ok(historyMessage);
        }
        [HttpPost]
        public async Task<IActionResult> CreateConversation(ConversationRequest request)
        {
            var conversationId = await  _chatRepository.CreateConversation(request);
            return Ok(conversationId);
        }
    }
}
