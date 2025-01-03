﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UniVisionBot.DTOs.Chat;
using UniVisionBot.Hubs;
using UniVisionBot.Services.Chat;

namespace UniVisionBot.Controllers
{
    [Route("api/conversations")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepository _chatRepository;
        private readonly IHubContext<ChatHub> _hubContext;
        public ChatController(IChatRepository chatRepository, IHubContext<ChatHub> context)
        {
            _chatRepository = chatRepository;
            _hubContext = context;
        }


        [HttpGet("current/{currentUserId}/{consultantId}")]
        public async Task<IActionResult> GetConversationForUser(string currentUserId, string consultantId)
        {
            var conversation = await _chatRepository.GetConversationForCurrentUser(currentUserId, consultantId);
            return Ok(conversation);
        }

        [HttpGet("{consultantId}")] //consultant
        public async Task<IActionResult> GetAllConversationForConsultant(string consultantId) // consultantId
        {
            var conversation = await _chatRepository.GetAllConversationForConsultant(consultantId);
            return Ok(conversation);
        }
        [HttpGet("history/{conversationId}")]
        public async Task<IActionResult> GetHistoryMessageForParticularUser(string conversationId)
        {
            var historyMessage = await _chatRepository.GetHistoryMessage(conversationId);
            return Ok(historyMessage);
        }
        [HttpPost]
        public async Task<IActionResult> CreateConversation(ConversationRequest request)
        {
            var conversationId = await _chatRepository.CreateConversation(request);
            return Ok(conversationId);
        }
        [HttpGet("pending")]
        public IActionResult GetAllPendingConversation()
        {
            var result = _chatRepository.GetAllPendingConversation();
            return Ok(result);
        }
        
        [HttpDelete("{conversationId}")]
        public async Task<IActionResult> DeleteConversation(string conversationId)
        { 
            await _chatRepository.DeleteConversation(conversationId);
            return Ok();
        }
        [HttpPost("notify")]
        public async Task<IActionResult> NotifyNewPendingConversation([FromBody] PendingConversationRequest request)
        {
            try
            {
                var consultantConnectionId = ChatHub.GetConsultantConnection(request.ConsultantId);
                if (string.IsNullOrEmpty(consultantConnectionId))
                {
                    return BadRequest("Consultant not connected");
                }
                var response = _chatRepository.GetPendingConversation(request);
                await _hubContext.Clients.Client(consultantConnectionId).SendAsync("NotifyPendingConversation", response);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request");
            }
        }
    }
}
